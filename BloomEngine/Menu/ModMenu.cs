using MelonLoader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomEngine.Menu;

public static class ModMenu
{
    private static ConcurrentDictionary<string, ModInfo> mods = new ConcurrentDictionary<string, ModInfo>();

    /// <summary>
    /// Event invoked when a mod is registered to the Mod Menu
    /// </summary>
    public static event Action<MelonMod> OnModRegistered;

    /// <summary>
    /// Registers a mod with the mod menu and displays it in the mod list. Mods that are not registered will
    /// still be displayed, however their name will be yellow and they will not have available configuration options.
    /// </summary>
    /// <remarks>
    /// This method ensures that the provided mod is added to the registry under its uniqueidentifier.
    /// </remarks>
    /// <param name="mod">The mod to register. Must not be <see langword="null"/> and must have a valid, non-empty <see cref="MelonMod.ID"/>.</param>
    public static void Register(MelonMod mod, ModConfig config = null)
    {
        mods[mod.Info.Name] = new ModInfo(mod, config);
        OnModRegistered?.Invoke(mod);

        Melon<BloomEnginePlugin>.Logger.Msg($"[ModMenu] Successfully registered {mod.Info.Name} from {mod.MelonAssembly.Assembly.FullName}");

        foreach(var property in config?.Properties)
        {
            Melon<BloomEnginePlugin>.Logger.Msg($"[ModMenu]   - Registered config property: \"{property.Name}\"");
            Melon<BloomEnginePlugin>.Logger.Msg($"[ModMenu]       - Before: \"{property.Getter()}\"");
            property.Setter?.Invoke("new yippee");
            Melon<BloomEnginePlugin>.Logger.Msg($"[ModMenu]       - After: \"{property.Getter()}\"");
        }
    }
}
