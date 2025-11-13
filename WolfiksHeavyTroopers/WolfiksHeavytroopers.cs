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
    public override string Name { get; init; } = "SerWolfikHeavyTroopers";
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
        var masks = modHelper.GetJsonDataFromFile<Masks>(itemPropsPath, "en.json");
        var tables = db.GetTables();
        var ItemCreator = new ItemCreator(logger, modConfig, masks);
        var traderHelper = new TraderHelper(db, databaseService, logger, modConfig, masks);
        var botHelper = new BotHelper(db, databaseService, logger, modConfig, masks);
        ItemCreator.BuildItems(customItemService);
        traderHelper.addMasksToTrader("5935c25fb3acc3127c3d8cd9");
        traderHelper.addMasksToQuests();
        botHelper.addCultistMaskToCultistLoadout();

        string[] defaultHelmets =
        [
            "5a154d5cfcdbcb001a3b00da",
            "5ac8d6885acfc400180ae7b0",
            "5b432d215acfc4771e1c6624",
            "5ea05cf85ad9772e6624305d",
            "5e01ef6886f77445f643baa4",
            "5e00c1ad86f774747333222c"
        ];

        string[] conflictingFaceCoverings =
        [
            "5e71f6be86f77429f2683c44",
            "5b4325355acfc40019478126",
            "5e54f76986f7740366043752",
            "5e71fad086f77422443d4604",
            "572b7fa524597762b747ce82",
            "5ab8f85d86f7745cd93a1cf5",
        ];

        string[] maps =
        [
            "bigmap",      // customs
            "factory4_day",
            "factory4_night",
            "woods",
            "rezervbase",
            "shoreline",
            "interchange",
            "tarkovstreets",
            "lighthouse",
            "laboratory",
            "sandbox",     // groundzero
            "sandbox_high" // groundzero_lvl_20+
        ];

        Dictionary<string, string> lootContainerMap = new()
        {
            { "weapon_box_5x5", "5909d89086f77472591234a0" },
            { "weapon_box_4x4", "5909d7cf86f77470ee57d75a" },
            { "weapon_box_6x3", "5909d76c86f77471e53d2adf" },
            { "weapon_box_5x2", "5909d5ef86f77467974efbd8" },
            { "ground_cache_4x4", "5d6d2b5486f774785c2ba8ea" },
            { "wooden_crate_5x2", "578f87ad245977356274f2cc" },
            { "duffle_bag_4x3", "578f87a3245977356274f2cb" },
            { "dead_scav_4x4", "5909e4b686f7747f5b744fa4" },
        };

        foreach (var (maskName, maskProps) in masks.Items)
        {
            if (tables?.Templates?.Items == null) continue;

            // Add masks to every helmet filter
            foreach (var helmet in defaultHelmets)
            {
                if (tables.Templates.Items.TryGetValue(helmet, out var currentHelmet))
                {
                    currentHelmet.Properties?.Slots?.ElementAt(1).Properties?.Filters?.ElementAt(0).Filter?.Add(maskProps.Id);
                }
            }
            foreach (var currentFaceConvering in conflictingFaceCoverings)
            {
                if (tables.Templates.Items.TryGetValue(maskProps.Id, out var currentMask))
                {
                    currentMask.Properties?.ConflictingItems?.Remove(currentFaceConvering);
                }
            }
        }

        foreach (var (maskConfigName, maskConfigProps) in modConfig.Config)
        {
            if (masks.Items.TryGetValue(maskConfigName, out var maskProps))
            {
                foreach (var map in maps)
                {
                    string mapName = tables.Locations.GetMappedKey(map);
                    var location = tables.Locations.GetDictionary()[mapName];
                    var mapStaticLoot = location?.StaticLoot?.Value;
                    var staticLooProbabilities = maskConfigProps.static_loot_container_probabilities;

                    foreach (var (lootContainerString, probability) in staticLooProbabilities)
                    {
                        var lootContainer = lootContainerMap[lootContainerString];
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
                            logger.Error($"[Wolfiks Heavy Troopers] Could not add {maskConfigName} to container {lootContainerString} on map {map}");
                        }
                    }
                }
            }
        }
        logger.Success("Wolfiks Heavy Troopers successfully add to server!");
        return Task.CompletedTask;
    }










}