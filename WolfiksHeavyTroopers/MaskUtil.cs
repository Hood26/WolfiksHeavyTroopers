namespace WolfiksHeavyTroopers;

class MaskUtil
{
    public readonly string[] defaultHelmets =
    [
        "5a154d5cfcdbcb001a3b00da",
        "5ac8d6885acfc400180ae7b0",
        "5b432d215acfc4771e1c6624",
        "5ea05cf85ad9772e6624305d",
        "5e01ef6886f77445f643baa4",
        "5e00c1ad86f774747333222c"
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