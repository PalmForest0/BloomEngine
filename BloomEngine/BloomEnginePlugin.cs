using BloomEngine;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using System.Runtime.InteropServices;
using UnityEngine;

[assembly: MelonInfo(typeof(BloomEnginePlugin), BloomEnginePlugin.Name, BloomEnginePlugin.Version, BloomEnginePlugin.Author)]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: ComVisible(false)]

namespace BloomEngine;

public class BloomEnginePlugin : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "1.0.0";
    public const string Author = "PalmForest";
    public const string Id = "com.palmforest.bloomengine";

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg($"Successfully initialised {nameof(BloomEngine)}.");
        RegisterAllMonoBehaviours();
    }

    private void RegisterAllMonoBehaviours()
    {
        var monoBehaviourTypes = MelonAssembly.Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsAbstract)
            .OrderBy(type => type.Name);

        foreach (var type in monoBehaviourTypes)
        {
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Failed to register MonoBehaviour: {type.FullName}\n{e}");
            }
        }
    }
}