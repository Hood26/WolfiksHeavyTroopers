namespace WolfiksHeavyTroopers;

public class Masks
{
    public required Dictionary<string, MaskProps> Items { get; set; }
}

public class MaskProps
{
    public required string Id { get; set; }
    public required string QuestId { get; set; }
    public required string ItemAssortId { get; set; }
    public required string QuestAssortId { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required string Description { get; set; }

}