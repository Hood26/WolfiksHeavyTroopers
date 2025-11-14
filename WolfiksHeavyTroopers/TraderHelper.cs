using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Models.Common;

namespace WolfiksHeavyTroopers;

class TraderHelper
{
    private readonly DatabaseServer db;
    private readonly DatabaseService databaseService;
    private readonly ISptLogger<WolfiksHeavyTroopers> logger;
    private readonly ModConfig modConfig;
    private readonly Masks masks;

    public TraderHelper(DatabaseServer db, DatabaseService databaseService, ISptLogger<WolfiksHeavyTroopers> logger, ModConfig modConfig, Masks masks)
    {
        this.db = db;
        this.databaseService = databaseService;
        this.logger = logger;
        this.modConfig = modConfig;
        this.masks = masks;
    }
    public void addMasksToTrader(
        string traderId)
    {
        var assortCreator = new FluentTraderAssortCreator(databaseService, logger);

        foreach (var (name, props) in modConfig.Config)
        {
            if (props.sold_by_trader)
            {
                assortCreator.CreateSingleAssortItem(masks.Items[name].Id, masks.Items[name].ItemAssortId)
                    .AddUnlimitedStackCount()
                    .AddBuyRestriction(props.trader_stock)
                    .AddMoneyCost(Money.DOLLARS, props.trader_price)
                    .AddLoyaltyLevel(props.loyalty_level)
                    .Export(traderId);
            }
        }
    }

    public void addMasksToQuests()
    {
        var tables = db.GetTables();
        var quests = tables.Templates.Quests;
        MongoId peacekeeper = "5935c25fb3acc3127c3d8cd9";

        foreach (var (maskName, maskProps) in masks.Items)
        {
            if (!modConfig.Config[maskName].quest_required) continue;

            // Add masks to Peacekeeper QuestAssort
            if (tables.Traders.TryGetValue(peacekeeper, out var trader)) {
                //logger.Success($"Adding {maskName} to Peacekeeper QuestAssort");
                trader.QuestAssort["success"].Add(masks.Items[maskName].ItemAssortId, masks.Items[maskName].QuestId);
            }

            // Add mask to quest reward
            if (quests.TryGetValue(masks.Items[maskName].QuestId, out var quest) && quest is not null)
            {
                //logger.Success($"Creating {maskName} Reward");
                var reward = new Reward
                {
                    AvailableInGameEditions = [],
                    GameMode = [
                        "regular",
                        "pve"
                    ],
                    Id = maskProps.QuestAssortId,
                    IsHidden = false,
                    Items = [
                        new Item {
                            Id = maskProps.ItemAssortId,
                            Template = maskProps.Id
                        }
                    ],
                    LoyaltyLevel = 4,
                    Target = maskProps.ItemAssortId,
                    TraderId = peacekeeper,
                    Type = RewardType.AssortmentUnlock,
                    Unknown = false
                };

                if (quest.Rewards!.TryGetValue("Success", out var rewards))
                {
                    //logger.Success($"Adding {maskName} to Reward");
                    rewards.Add(reward);
                }
            }
        }
    }
}