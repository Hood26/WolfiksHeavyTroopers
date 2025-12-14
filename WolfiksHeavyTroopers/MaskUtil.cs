namespace WolfiksHeavyTroopers;

class MaskUtil
{
    public readonly string[] helmets =
    [
        // Defaults
        "5a154d5cfcdbcb001a3b00da",
        "5ac8d6885acfc400180ae7b0",
        "5b432d215acfc4771e1c6624",
        "5ea05cf85ad9772e6624305d",
        "5e01ef6886f77445f643baa4",
        "5e00c1ad86f774747333222c",
        // Couturier
        "68af53260c10f1000000018c",
        "68af53260c10f10000000191",
        "68af53260c10f1000000019b",
        "68af53260c10f100000001a0",
        "68835fc00c10f100000000b0",
        "68835fc00c10f100000000b5",
        "68835fc00c10f100000000ba",
        "68835fc00c10f100000000bf",
        "6883724d0c10f100000000b8",
    ];

    public readonly string[] artemHelmets = 
    [
        "66326bfd46817c660d015125", // DLP Tactical Extreme Helmet
        "66326bfd46817c660d015126", // Ops-Core FAST Carbon High Cut Helmet (Dark Blue)
        "66326bfd46817c660d015127", // Legacy Safety Special Ops Ballistic Helmet FAST
        "66326bfd46817c660d015128", // Ops-Core FAST Carbon High Cut Helmet
        "66326bfd46817c660d015129", // DLP Tactical Extreme Helmet
        "66326bfd46817c660d01512d", // Tactical Bump Helmet (Black)
        "6673b1ac5cae0610f1079d7e", // Tactical Bump Helmet (Alpine)
        "669819683571cb050b0b6393", // ACH High Cut Tactical Helmet (Alpine)
        "669819683571cb050b0b6394", // ACH High Cut Tactical Helmet (Black)
        "66bf757f27d0b097db0ace44", // Ops-Core SF High Cut Helmet (Multicam)
        "66bf757f27d0b097db0ace58", // Ops-Core SF High Cut Helmet (OD)
        "66bf757f27d0b097db0ace61", // Ops-Core SF High Cut Helmet (Black)
        "676a1476242dea0ba69ebbd8", // Ops-Core SF Warrior Helmet (Black)
        "66326bfd46817c660d01512a", // Ops-Core FAST Carbon High Cut Helmet (Tan)
    ];

    public readonly string[] conflictingFaceCoverings =
    [
        "5e71f6be86f77429f2683c44",
        "5b4325355acfc40019478126",
        "5e54f76986f7740366043752",
        "5e71fad086f77422443d4604",
        "572b7fa524597762b747ce82",
        "5ab8f85d86f7745cd93a1cf5",
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