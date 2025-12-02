using BloomEngine.Config;
using MelonLoader;
using UnityEngine;

namespace BloomEngine.Menu;

public class ModEntry
{
    public string DisplayName { get; private set; }
    public string Description { get; private set; }

    public Texture2D Image { get; private set; }
    public MelonMod Mod { get; private set; }

    public ModConfigBase Config { get; private set; }

    internal ModEntry(MelonMod mod, string displayName)
    {
        Mod = mod;
        DisplayName = displayName;
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

    public ModEntry AddConfigProperty<T>(ConfigPropertyData<T> data)
    {
        Config ??= new ModConfigBase();
        Config.AddProperty(data);

        return this;
    }

    public ModEntry Register()
    {
        ModMenu.Register(this);
        return this;
    }
}