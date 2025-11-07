using System.Runtime.InteropServices;
using SPTarkov.Server.Core.Models.Common;

namespace WolfiksHeavyTroopers;

public class ModConfig
{
    public required Dictionary<string, ItemProps> Items { get; set; }
}

public class ItemProps
{
    public bool enable { get; set; }
    public bool sold_by_trader { get; set; }
    public bool flea_banned { get; set; }
    public int trader_price { get; set; }
    public int flea_price { get; set; }
    public int handbook_price { get; set; }
    public int trader_stock { get; set; }
    public int loyalty_level { get; set; }
    public int armor_class_level { get; set; }
    public int durability { get; set; }
    public int max_durability { get; set; }
    public required Dictionary<MongoId, int> static_loot_probabilities { get; set; }
}