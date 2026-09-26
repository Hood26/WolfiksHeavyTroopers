using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;

namespace WolfiksHeavyTroopers;

class MaskUtil(
    DatabaseServer db,
    DatabaseService ds,
    ISptLogger<WolfiksHeavyTroopers> logger,
    ModConfig config,
    Masks masks,
    Locales locales
    )
{

    public readonly DatabaseServer db = db;
    public readonly DatabaseService ds = ds;
    public readonly ISptLogger<WolfiksHeavyTroopers> logger = logger;
    public readonly ModConfig config = config;
    public readonly Masks masks = masks;
    public readonly Locales locales = locales;
    public readonly string[] helmets =
    [
        // Defaults
        "5a154d5cfcdbcb001a3b00da", // Ops-Core FAST MT Super High Cut helmet (Black)
        "5ac8d6885acfc400180ae7b0", // Ops-Core FAST MT Super High Cut helmet (Urban Tan)
        "5b432d215acfc4771e1c6624", // LShZ lightweight helmet (Olive Drab)
        "5ea05cf85ad9772e6624305d", // Tac-Kek FAST MT helmet (Replica)
        "5e01ef6886f77445f643baa4", // Team Wendy EXFIL Ballistic Helmet (Coyote Brown)
        "5e00c1ad86f774747333222c", // Team Wendy EXFIL Ballistic Helmet (Black)
        // Couturier
        "68af53260c10f1000000018c", // LShZ lightweight helmet (Dusk)
        "68af53260c10f10000000191", // LShZ lightweight helmet (EMR Summer)
        "68af53260c10f1000000019b", // LShZ lightweight helmet (Yagel)
        "68af53260c10f100000001a0", // LShZ lightweight helmet (SURPAT)
        "68835fc00c10f100000000b0", // Ops-Core FAST MT Super High Cut helmet (MM14)
        "68835fc00c10f100000000b5", // Ops-Core FAST MT Super High Cut helmet (Multicam)
        "68835fc00c10f100000000ba", // Ops-Core FAST MT Super High Cut helmet (A-TACS FG)
        "68835fc00c10f100000000bf", // Ops-Core FAST MT Super High Cut helmet (UCP)
        "6883724d0c10f100000000b8", // Ops-Core FAST MT Super High Cut helmet (Multicam Tropic)
    ];

    public readonly string[] tcgHelmets = [
        // Tactical Gear Component / Painter
        "672e2e75b14ae1b5c91474b4"  // Ops-Core FAST MT MODXII (M90)
    ];

    public readonly string[] artemHelmets = 
    [
        "66326bfd46817c660d015126", // Ops-Core FAST Carbon High Cut Helmet (Dark Blue)
        "66326bfd46817c660d015128", // Ops-Core FAST Carbon High Cut Helmet
        "66bf757f27d0b097db0ace44", // Ops-Core SF High Cut Helmet (Multicam)
        "66bf757f27d0b097db0ace58", // Ops-Core SF High Cut Helmet (OD)
        "66bf757f27d0b097db0ace61", // Ops-Core SF High Cut Helmet (Black)
    ];

    public readonly string[] conflictingFaceCoverings =
    [
        "5e71f6be86f77429f2683c44", // Twitch Rivals 2020 mask
        "5b4325355acfc40019478126", // Shemagh (Tan)
        "5e54f76986f7740366043752", // Shroud half-mask
        "5e71fad086f77422443d4604", // Twitch Rivals 2020 half-mask
        "572b7fa524597762b747ce82", // Lower half-mask
        "5ab8f85d86f7745cd93a1cf5", // Shemagh (Green)
    ];

    public readonly string[] maps =
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

    public readonly Dictionary<string, string> traderMap = new()
    {
        {"peacekeeper", "5935c25fb3acc3127c3d8cd9"},
        {"skier", "58330581ace78e27b8b10cee"},
        {"mechanic", "5a7c2eca46aef81a7ca2145d"},
        {"ragman", "5ac3b934156ae10c4430e83c"},
        {"jaeger", "5c0647fdd443bc2504c2d371"},
        {"prapor", "54cb50c76803fa8b248b4571"},
        {"therapist", "54cb57776803fa99248b456e"}
    };

    public readonly Dictionary<string, string> lootContainerMap = new()
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
}