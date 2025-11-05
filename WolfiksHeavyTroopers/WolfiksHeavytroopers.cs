using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Common;
using Microsoft.VisualBasic;
using SPTarkov.Server.Core.Models.Common;
using System.ComponentModel.Design;
using JetBrains.Annotations;

namespace WolfiksHeavyTroopers;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; init; } = "SerWolfikHeavyTroopers";
    public override string Author { get; init; } = "Hood";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.1.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.3");


    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://forge.sp-tarkov.com/mod/1569/wolfiks-heavy-trooper-masks-reupload";
    public override bool? IsBundleMod { get; init; } = true;
    public override string? License { get; init; } = "Creative Commons BY-NC-SA 3.0 ";
    public override string ModGuid { get; init; } = "com.hood.wolfiksheavytroopers";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class SerWolfikHeavyTroopers(
    ISptLogger<SerWolfikHeavyTroopers> logger,
    ConfigServer configServer,
    CustomItemService customItemService,
    ModHelper modHelper,
    DatabaseService databaseService,
    DatabaseServer db
    )
    : IOnLoad
{

    public Task OnLoad()
    {










        return Task.CompletedTask;
    }










}