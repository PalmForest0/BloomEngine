using BloomEngine.Core;
using BloomEngine.Helpers;
using BloomEngine.ModMenu;
using MelonLoader;

namespace BloomEngine;

internal sealed class BloomEngineMod : MelonMod
{
    public const string UnknownString = "???";
    public const string Name = "BloomEngine";
    public const string Description = "Robust mod menu and config manager for PvZ Replanted.";
    public const string Author = "PalmForest";
    public const string Version = "0.3.0-beta";

    public override void OnInitializeMelon()
    {
        BloomLogger.Logger = LoggerInstance;
        BloomLogger.Info($"Successfully loaded {Name} v{Version} by {Author}.");

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription(Description)
            .AddIcon(AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.BloomEngineIcon.png"))
            .Register();
    }
}