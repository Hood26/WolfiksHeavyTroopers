using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services.Mod;

namespace WolfiksHeavyTroopers;

class ItemCreator
{
    private readonly ISptLogger<WolfiksHeavyTroopers> logger;
    private readonly ModConfig config;
    private readonly Masks masks;
    public ItemCreator(ISptLogger<WolfiksHeavyTroopers> logger, ModConfig config, Masks masks)
    {
        this.logger = logger;
        this.config = config;
        this.masks = masks;
    }

    public void BuildItems(CustomItemService customItemService)
    {
        foreach (var (name, props) in config.Config)
        {
            if (!props.enable) continue;

            var newItem = new NewItemFromCloneDetails
            {
                ItemTplToClone = "5ea058e01dbce517f324b3e2",
                OverrideProperties = new TemplateItemProperties
                {
                    ArmorClass = props.armor_class_level,
                    Durability = props.durability,
                    MaxDurability = props.max_durability,
                    Prefab = new Prefab
                    {
                        Path = $"assets/{name}.bundle",
                        Rcid = ""
                    },
                },
                ParentId = "57bef4c42459772e8d35a53b",
                NewId = masks.Items[name].Id,
                FleaPriceRoubles = props.flea_price,
                HandbookPriceRoubles = props.handbook_price,
                HandbookParentId = "5b5f704686f77447ec5d76d7",
                Locales = new Dictionary<string, LocaleDetails>
                {
                    {
                        "en",
                        new LocaleDetails
                        {
                            Name = masks.Items[name].Name,
                            ShortName = masks.Items[name].ShortName,
                            Description = masks.Items[name].Description,
                        }
                    }
                }

            };
            customItemService.CreateItemFromClone(newItem);
        }
    }
}