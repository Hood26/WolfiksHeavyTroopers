using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;

namespace WolfiksHeavyTroopers;

class TraderHelper
{
    private readonly DatabaseService databaseService;
    private readonly ISptLogger<WolfiksHeavyTroopers> logger;
    private readonly ModConfig config;
    private readonly Masks masks;

    public TraderHelper(DatabaseService databaseService, ISptLogger<WolfiksHeavyTroopers> logger, ModConfig config, Masks masks)
    {
        this.databaseService = databaseService;
        this.logger = logger;
        this.config = config;
        this.masks = masks;
    }
    public void addMasksToTrader(
        string traderId)
    {
        var assortCreator = new FluentTraderAssortCreator(databaseService, logger);

        foreach (var (name, props) in config.Items)
        {
            if (props.sold_by_trader)
            {
                assortCreator.CreateSingleAssortItem(masks.Items[name].Id)
                    .AddUnlimitedStackCount()
                    .AddBuyRestriction(props.trader_stock)
                    .AddMoneyCost(Money.DOLLARS, props.trader_price)
                    .AddLoyaltyLevel(props.loyalty_level)
                    .Export(traderId);
            }
        }
    }
}