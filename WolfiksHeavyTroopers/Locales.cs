namespace WolfiksHeavyTroopers;

public class Locales
{
    public required Dictionary<string, Dictionary<string, LocaleProps>> Items { get; set; }
}

public class LocaleProps
{
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required string Description { get; set; }

}