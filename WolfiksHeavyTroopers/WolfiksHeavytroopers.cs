using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Common;
using JetBrains.Annotations;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Runtime.InteropServices;
using SPTarkov.Server.Core.Models.Eft.Inventory;

namespace WolfiksHeavyTroopers;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; init; } = "Wolfiks Heavy Troopers";
    public override string Author { get; init; } = "Hood";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.1.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.3");


    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://forge.sp-tarkov.com/mod/1569/wolfiks-heavy-trooper-masks-reupload";
    public override bool? IsBundleMod { get; init; } = true;
    public override string? License { get; init; } = "Creative Commons BY-NC-SA 3.0 ";
    public override string ModGuid { get; init; } = "com.hood.wolfiksheavytroopers";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class WolfiksHeavyTroopers(
    ISptLogger<WolfiksHeavyTroopers> logger,
    ConfigServer configServer,
    CustomItemService customItemService,
    ModHelper modHelper,
    DatabaseService databaseService,
    DatabaseServer db
    )
    : IOnLoad
{

    public Task OnLoad()
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(pathToMod, "config"));
        var itemPropsPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(pathToMod, "locales"));
        var modConfig = modHelper.GetJsonDataFromFile<ModConfig>(configPath, "config.jsonc");
        var ragfairConfig = configServer.GetConfig<RagfairConfig>();
        var masks = modHelper.GetJsonDataFromFile<Masks>(itemPropsPath, "en.json");
        var tables = db.GetTables();
        var ItemCreator = new ItemCreator(logger, modConfig, masks);
        var traderHelper = new TraderHelper(db, databaseService, logger, modConfig, masks);
        var botHelper = new BotHelper(db, databaseService, logger, modConfig, masks);
        var maskUtil = new MaskUtil();
        ItemCreator.BuildItems(customItemService);
        traderHelper.addMasksToTrader("5935c25fb3acc3127c3d8cd9");
        traderHelper.addMasksToQuests();
        botHelper.addCultistMaskToCultistLoadout();


        foreach (var (maskName, maskProps) in masks.Items)
        {
            if (tables?.Templates?.Items == null) continue;

            // Add masks to every helmet filter
            foreach (var helmet in maskUtil.defaultHelmets)
            {
                if (tables.Templates.Items.TryGetValue(helmet, out var currentHelmet))
                {
                    currentHelmet.Properties?.Slots?.ElementAt(1).Properties?.Filters?.ElementAt(0).Filter?.Add(maskProps.Id);
                }
            }
            foreach (var currentFaceConvering in maskUtil.conflictingFaceCoverings)
            {
                if (tables.Templates.Items.TryGetValue(maskProps.Id, out var currentMask))
                {
                    currentMask.Properties?.ConflictingItems?.Remove(currentFaceConvering);
                }
            }
        }

        foreach (var (maskConfigName, maskConfigProps) in modConfig.Config)
        {
            // flea ban masks
            if (maskConfigProps.flea_banned)
            {
                ragfairConfig.Dynamic.Blacklist.Custom.Add(masks.Items[maskConfigName].Id);
            }
            // Static Loot Injection
            if (masks.Items.TryGetValue(maskConfigName, out var maskProps))
            {
                foreach (var map in maskUtil.maps)
                {
                    string mapName = tables.Locations.GetMappedKey(map);
                    var location = tables.Locations.GetDictionary()[mapName];
                    var mapStaticLoot = location?.StaticLoot?.Value;
                    var staticLooProbabilities = maskConfigProps.static_loot_container_probabilities;

                    foreach (var (lootContainerString, probability) in staticLooProbabilities)
                    {
                        var lootContainer = maskUtil.lootContainerMap[lootContainerString];
                        try
                        {
                            var newProbability = new ItemDistribution
                            {
                                Tpl = maskProps.Id,
                                RelativeProbability = probability
                            };

                            var list = mapStaticLoot[lootContainer].ItemDistribution?.ToList() ?? new List<ItemDistribution>();
                            list.Add(newProbability);
                            mapStaticLoot[lootContainer].ItemDistribution = list;

                            location.StaticLoot.AddTransformer(lazyLoadedStaticLoot =>
                            {
                                if (lazyLoadedStaticLoot == null) return lazyLoadedStaticLoot;
                                if (!lazyLoadedStaticLoot.TryGetValue(lootContainer, out StaticLootDetails? details)) return lazyLoadedStaticLoot;

                                var updatedItemDistribution = details.ItemDistribution?.ToList() ?? new List<ItemDistribution>();
                                updatedItemDistribution.Add(newProbability);
                                lazyLoadedStaticLoot[lootContainer].ItemDistribution = updatedItemDistribution;
                                return lazyLoadedStaticLoot;
                            });
                        }
                        catch
                        {
                            logger.Debug($"[Wolfiks Heavy Troopers] Could not add {maskConfigName} to container {lootContainerString} on map {map}");
                        }
                    }
                }
            }
        }
        logger.Success("[Wolfiks Heavy Troopers] Successfully added to server!");
        return Task.CompletedTask;
    }










}