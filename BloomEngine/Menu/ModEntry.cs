using BloomEngine.Config.Inputs;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.Menu;

public class ModEntry
{
    public MelonMod Mod { get; private set; }

    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public Texture2D Image { get; private set; }

    public Type ConfigClassType { get; private set; }
    public List<IInputField> ConfigInputFields { get; private set; }
    public bool HasConfig { get; private set; }

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

    public ModEntry AddConfig<T>()
    {
        ConfigClassType = typeof(T);
        ConfigInputFields = new List<IInputField>();

        // Get all input field fields
        var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            // Null instance for static class
            if (field.GetValue(null) is IInputField inputField)
                ConfigInputFields.Add(inputField);
        }

        // Get all input field properties
        var properties = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var prop in properties)
        {
            // Must be readable or skip
            if (!prop.CanRead)
                continue;

            // Protect from throwing in getter
            try
            {
                // Null instance for static class
                if (prop.GetValue(null) is IInputField inputField)
                    ConfigInputFields.Add(inputField);
            }
            catch { continue; }
        }

        HasConfig = true;
        return this;
    }

    /// <summary>
    /// Registers this <see cref="ModEntry"/>, making it appear in the mod menu with the provided information.
    /// </summary>
    public ModEntry Register()
    {
        ModMenu.Register(this);
        return this;
    }
}