using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using MelonLoader;

namespace BloomEngine;

internal class BloomEngineMod : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "0.2.0-beta";
    public const string Author = "PalmForest";

    internal static MelonLogger.Instance Logger { get; private set; }

    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;

        Il2CppHelper.RegisterAllMonoBehaviours(MelonAssembly.Assembly);
        LoggerInstance.Msg($"Successfully loaded version {Version} of {Name}.");

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Mod menu and config manager library for PvZ Replanted.")
            .AddIcon(AssetHelper.LoadSprite("BloomEngine.Resources.BloomEngineIcon.png"))
            .Register();
    }
}