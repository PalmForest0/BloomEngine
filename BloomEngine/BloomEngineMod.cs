using BloomEngine;
using BloomEngine.Utilities;
using MelonLoader;
using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(BloomEngineMod), BloomEngineMod.Name, BloomEngineMod.Version, BloomEngineMod.Author)]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: ComVisible(false)]

namespace BloomEngine;

internal class BloomEngineMod : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "0.1.0-alpha";
    public const string Author = "PalmForest";
    public const string Id = "com.palmforest.bloomengine";

    public override void OnInitializeMelon()
    {
        Il2CppHelper.RegisterAllMonoBehaviours(MelonAssembly.Assembly);
        LoggerInstance.Msg($"Successfully loaded version {Version} of {nameof(BloomEngine)}.");
    }
}