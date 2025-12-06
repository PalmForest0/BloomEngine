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


    /// <summary>
    /// Adds a <see cref="ModConfigBase"/> instance to this mod entry, making all of its properties appear in the in-game config.
    /// Alternatively, you can use <see cref="AddConfigProperty{T}(string, T, Action{T}, Func{T, bool}, Func{T, T})"/> to manually add individual properties.
    /// </summary>
    /// <param name="configInstance"></param>
    /// <returns></returns>
    public ModEntry AddConfig(ModConfigBase configInstance)
    {
        // Assign the config or merge with existing properties
        if(Config is null) Config = configInstance;
        else Config.InputFields.AddRange(configInstance.InputFields);

        return this;
    }

    /// <summary>
    /// Adds a property to the in-game config of this mod.
    /// </summary>
    /// <typeparam name="T">The value type of this property.</typeparam>
    /// <param name="name">The text that will appear next to the property input field in the config panel.</param>
    /// <param name="defaultValue">Default value of this property.</param>
    /// <param name="onValueUpdated">A callback that provides the new value when it is updated.</param>
    /// <param name="validateFunc">A function that validates the value before the setter is called.</param>
    /// <param name="transformFunc">A function that allows any transformation to be made to the value before the setter is called.</param>
    /// <returns>The modified mod entry with this config property.</returns>
    //public ModEntry AddConfigProperty<T>(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    //{
    //    Config ??= new ModConfigBase();
    //    Config.AddProperty(name, defaultValue, onValueUpdated, validateFunc, transformFunc);

    //    return this;
    //}

    /// <summary>
    /// Registers this <see cref="ModEntry"/>, making it appear in the mod menu with the provided information.
    /// </summary>
    public ModEntry Register()
    {
        ModMenu.Register(this);
        return this;
    }
}