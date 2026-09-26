using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services.Modding.Custom;

namespace WolfiksHeavyTroopers;

class ItemCreator(MaskUtil maskUtil)
{
    private readonly MaskUtil maskUtil = maskUtil;

    public void BuildItems(CustomItemService customItemService)
    {
        foreach (var (name, props) in maskUtil.config.Items)
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
                NewId = maskUtil.masks.Items[name].Id,
                NewItemName = name,
                FleaPriceRoubles = props.flea_price,
                HandbookPriceRoubles = props.handbook_price,
                HandbookParentId = "5b5f704686f77447ec5d76d7",
                Locales = new Dictionary<string, LocaleDetails>
                {
                    {
                        "en",
                        new LocaleDetails
                        {
                            Name = maskUtil.locales.Items["en"][name].Name,
                            ShortName = maskUtil.locales.Items["en"][name].ShortName,
                            Description = maskUtil.locales.Items["en"][name].Description,
                        }
                    },
                    {
                       "ru",
                        new LocaleDetails
                        {
                            Name = maskUtil.locales.Items["ru"][name].Name,
                            ShortName = maskUtil.locales.Items["ru"][name].ShortName,
                            Description = maskUtil.locales.Items["ru"][name].Description,
                        } 
                    },
                    {
                       "ch",
                        new LocaleDetails
                        {
                            Name = maskUtil.locales.Items["ch"][name].Name,
                            ShortName = maskUtil.locales.Items["ch"][name].ShortName,
                            Description = maskUtil.locales.Items["ch"][name].Description,
                        } 
                    }
                }

            };
            customItemService.CreateItemFromClone(newItem);
        }
    }
}