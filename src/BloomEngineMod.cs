using BloomEngine.Core;
using BloomEngine.Helpers;
using BloomEngine.ModList;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;

namespace BloomEngine;

internal sealed class BloomEngineMod : MelonMod
{
    public const string UnknownString = "???";
    public const string Name = "BloomEngine";
    private const string Description = "Robust mod list and config manager for PvZ Replanted.";
    public const string Author = "PalmForest";
    public const string Version = "0.3.0-beta";

    public override void OnInitializeMelon()
    {
        ClassInjector.RegisterTypeInIl2Cpp<UI.CustomPopup>();
        
        BloomLogger.Logger = LoggerInstance;
        BloomLogger.Info($"Successfully loaded {Name} v{Version} by {Author}.");

        ModListService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription(Description)
            .AddIcon(ResourceHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.BloomEngineIcon.png"))
            .AddConfigClass(typeof(BloomConfig))
            .Register();
    }
}