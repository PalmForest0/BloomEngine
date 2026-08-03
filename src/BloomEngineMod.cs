using BloomEngine.Core;
using BloomEngine.Helpers;
using BloomEngine.ModMenu;
using MelonLoader;

namespace BloomEngine;

internal sealed class BloomEngineMod : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "0.3.2-beta";
    public const string Author = "PalmForest";

    public override void OnInitializeMelon()
    {
        BloomLogger.Logger = LoggerInstance;
        BloomLogger.Info($"Successfully loaded version {Version} of {Name}.");

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Mod menu and config manager library for PvZ Replanted.")
            .AddIcon(AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.BloomEngineIcon.png"))
            .Register();
    }
}