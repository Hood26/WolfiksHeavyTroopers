using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Modding.Custom;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Spt.Tables;
namespace WolfiksHeavyTroopers;

[Injectable(TypePriority = OnLoadOrder.Preload + 5)]
public class WolfiksHeavyTroopers(
    ISptLogger<WolfiksHeavyTroopers> logger,
    RagfairConfig ragfairConfig,
    CustomItemService customItemService,
    ModHelper modHelper,
    TemplateTable templateTable,
    LocationTable locationTable,
    BotTable botTable,
    TradersTable traderTable
    )
    : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(pathToMod, "config"));
        var maskPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(pathToMod, "db"));
        var itemPropsPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(pathToMod, "locales"));
        var config = modHelper.GetJsonDataFromFile<ModConfig>(configPath, "config.jsonc");
        var masks = modHelper.GetJsonDataFromFile<Masks>(maskPath, "MaskProps.json");
        var locales = modHelper.GetJsonDataFromFile<Locales>(itemPropsPath, "locales.json");
        var maskUtil = new MaskUtil(templateTable, locationTable, traderTable, botTable, logger, config, masks, locales);
        var ItemCreator = new ItemCreator(maskUtil);
        var traderHelper = new TraderHelper(maskUtil);
        var botHelper = new BotHelper(maskUtil);
        ItemCreator.BuildItems(customItemService);
        traderHelper.addMasksToTrader();
        traderHelper.addMasksToQuests();
        botHelper.addCultistMaskToCultistLoadout();

        foreach (var (maskName, maskProps) in masks.Items)
        {
            if (!config.Items[maskName].enable) continue;
            if (templateTable.Items == null) continue;

            // Add masks to every helmet filter
            foreach (var helmet in maskUtil.helmets)
            {
                if (templateTable.Items.TryGetValue(helmet, out var currentHelmet))
                {
                    currentHelmet.Properties?.Slots?.ElementAt(1).Properties?.Filters?.ElementAt(0).Filter?.Add(maskProps.Id);
                }
            }
            foreach (var helmet in maskUtil.artemHelmets)
            {
                if (templateTable.Items.TryGetValue(helmet, out var currentHelmet))
                {
                    currentHelmet.Properties?.Slots?.ElementAt(0).Properties?.Filters?.ElementAt(0).Filter?.Add(maskProps.Id);
                }
            }
            foreach (var currentFaceConvering in maskUtil.conflictingFaceCoverings)
            {
                if (templateTable.Items.TryGetValue(maskProps.Id, out var currentMask))
                {
                    currentMask.Properties?.ConflictingItems?.Remove(currentFaceConvering);
                }
            }
        }

        foreach (var (maskConfigName, maskConfigProps) in config.Items)
        {
            if (!maskConfigProps.enable) continue;

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
                    string mapName = locationTable.GetMappedKey(map);
                    var location = locationTable.GetDictionary()[mapName];
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
        logger.Success("[Wolfiks Heavy Trooper Masks] Successfully added to server!");
        return Task.CompletedTask;
    }










}