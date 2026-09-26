using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Utils.Json;

namespace WolfiksHeavyTroopers;

class TraderHelper(MaskUtil maskUtil)
{
    private readonly MaskUtil maskUtil = maskUtil;

    public void addMasksToTrader()
    {
        var assortCreator = new FluentTraderAssortCreator(maskUtil.traderTable, maskUtil.logger);

        foreach (var (name, props) in maskUtil.config.Items)
        {
            if(!props.enable) continue;
            if (props.sold_by_trader)
            {
                MongoId traderId = maskUtil.traderMap[maskUtil.config.Items[name].trader];
                var currencyType = getCurrencyType(maskUtil.config.Items[name].trader_currency_type);
                assortCreator.CreateSingleAssortItem(maskUtil.masks.Items[name].Id, maskUtil.masks.Items[name].ItemAssortId)
                    .AddUnlimitedStackCount()
                    .AddBuyRestriction(props.trader_stock)
                    .AddMoneyCost(currencyType, props.trader_price)
                    .AddLoyaltyLevel(props.loyalty_level)
                    .Export(traderId);
            }
        }
    }

    public MongoId getCurrencyType(string currencyType)
    {
        Dictionary<string, MongoId> currencyTypes = new()
        {
            {"roubles", Money.ROUBLES},
            {"dollars", Money.DOLLARS},
            {"euros", Money.EUROS}
        };

        if (currencyTypes.TryGetValue(currencyType, out var result))
        {
            return result;
        }

        return Money.ROUBLES;
    }

    public void addMasksToQuests()
    {
        var quests = maskUtil.templateTable.Quests;
        //MongoId peacekeeper = "5935c25fb3acc3127c3d8cd9";

        foreach (var (maskName, maskProps) in maskUtil.masks.Items)
        {
            if (!maskUtil.config.Items[maskName].enable) continue;
            if (!maskUtil.config.Items[maskName].quest_required) continue;

            string traderId = maskUtil.traderMap[maskUtil.config.Items[maskName].trader];
            StringOrInt _traderId = new(traderId, null); // wtf is this
            // Add masks to Peacekeeper QuestAssort
            if (maskUtil.traderTable.TryGetValue(traderId, out var trader)) {
                //logger.Success($"Adding {maskName} to Peacekeeper QuestAssort");
                trader.QuestAssort["success"].Add(maskUtil.masks.Items[maskName].ItemAssortId, maskUtil.masks.Items[maskName].QuestId);
            }

            // Add mask to quest reward
            if (quests.TryGetValue(maskUtil.masks.Items[maskName].QuestId, out var quest) && quest is not null)
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
                    TraderId = _traderId,
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