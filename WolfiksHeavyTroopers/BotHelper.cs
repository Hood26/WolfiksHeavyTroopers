using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Models.Common;
using System.ComponentModel;

namespace WolfiksHeavyTroopers;

class BotHelper
{
    private readonly DatabaseServer db;
    private readonly DatabaseService databaseService;
    private readonly ISptLogger<WolfiksHeavyTroopers> logger;
    private readonly ModConfig modConfig;
    private readonly Masks masks;

    public BotHelper(DatabaseServer db, DatabaseService databaseService, ISptLogger<WolfiksHeavyTroopers> logger, ModConfig modConfig, Masks masks)
    {
        this.db = db;
        this.databaseService = databaseService;
        this.logger = logger;
        this.modConfig = modConfig;
        this.masks = masks;
    }

    public void addCultistMaskToCultistLoadout()
    {
        var tables = db.GetTables();

        if (tables.Bots.Types.TryGetValue("sectantwarrior", out var cultist))
        {
            logger.Success("Adding Cultist Mask to Cultists");
            // 15% chance to spawn with helmet - the chance face covering stops it??
            cultist.BotChances.EquipmentChances["Headwear"] = 15;
            // Always spawns with a mask
            // Might present a compatibility issue in the future. maybe. idk.
            cultist.BotChances.EquipmentModsChances["mod_nvg"] = 100; // Mask is considered an nvg for some reason 

            var headwearInventory = cultist.BotInventory.Equipment[EquipmentSlots.Headwear];
            headwearInventory.Add("5a154d5cfcdbcb001a3b00da", 999999); // FAST MT Black Helmet

            var mods = cultist.BotInventory.Mods;

            Dictionary<string, HashSet<MongoId>> modsToAdd = new()
            {
                ["Helmet_top"] = new HashSet<MongoId>
                    {
                        "657f8ec5f4c82973640b234c"
                    },
                ["Helmet_back"] = new HashSet<MongoId>
                    {
                        {
                            "657f8ec5f4c82973640b234c"
                        }
                    },
                ["mod_nvg"] = new HashSet<MongoId>
                    {
                        {
                            "690be27992b93c91540ad4e3"
                        }
                    }
            };
            cultist.BotInventory.Mods.Add("5a154d5cfcdbcb001a3b00da", modsToAdd);
        }
    }
}