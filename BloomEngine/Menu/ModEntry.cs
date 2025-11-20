using BloomEngine.Config;
using MelonLoader;
using UnityEngine;

namespace BloomEngine.Menu;

public class ModEntry
{
    public string Id { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }

    public Texture2D Image { get; private set; }
    public MelonMod Mod { get; private set; }

    public ModConfigBase Config { get; private set; }

    public ModEntry(MelonMod mod, string id, string displayName = null)
    {
        Mod = mod;
        Id = id;
        DisplayName = displayName ?? mod.Info.Name;
    }

    public ModEntry AddDescription(string description)
    {
        Description = description;
        return this;
    }

    public ModEntry AddImage(Texture2D image)
    {
        Image = image;
        return this;
    }

    public ModEntry AddConfig(ModConfigBase configInstance)
    {
        Config = configInstance;
        return this;
    }

    public ModEntry AddConfigProperty<T>(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    {
        Config ??= new ModConfigBase();
        Config.AddProperty(name, defaultValue, onValueUpdated, validateFunc, transformFunc);

        return this;
    }

    public ModEntry Register()
    {
        ModMenu.Register(this);
        return this;
    }
}