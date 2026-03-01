using BloomEngine.Helpers;
using BloomEngine.ModMenu.Services;
using MelonLoader;

namespace BloomEngine;

internal sealed class BloomEngineMod : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "0.3.1-beta";
    public const string Author = "PalmForest";

    internal static MelonLogger.Instance Logger { get; private set; }

    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;
        Logger.Msg($"Successfully loaded version {Version} of {Name}.");

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Mod menu and config manager library for PvZ Replanted.")
            .AddIcon(AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.BloomEngineIcon.png"))
            .Register();
    }
}