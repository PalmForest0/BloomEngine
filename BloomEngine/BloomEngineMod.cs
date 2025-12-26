using BloomEngine.Modules.Menu;
using BloomEngine.Utilities;
using MelonLoader;

namespace BloomEngine;

internal class BloomEngineMod : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "0.2.0-beta";
    public const string Author = "PalmForest";

    public override void OnInitializeMelon()
    {
        Il2CppHelper.RegisterAllMonoBehaviours(MelonAssembly.Assembly);
        LoggerInstance.Msg($"Successfully loaded version {Version} of {Name}.");

        ModMenu.CreateEntry(this)
            .AddDescription($"Mod menu and config manager library for PvZ Replanted.")
            .AddIcon(AssetHelper.LoadSprite("BloomEngine.Resources.ModIcon.png"))
            .Register();
    }
}