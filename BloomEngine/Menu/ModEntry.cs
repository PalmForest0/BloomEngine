using BloomEngine.Config.Inputs;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.Menu;

/// <summary>
/// A mod entry that shows up in the mod menu if <see cref="Register"/> is called.
/// </summary>
public class ModEntry
{
    public MelonMod Mod { get; private set; }

    public string DisplayName { get; private set; }
    public string Description { get; private set; }

    //public Texture2D Image { get; private set; }

    public List<IInputField> ConfigInputFields { get; private set; }
    public bool HasConfig { get; private set; }


    internal ModEntry(MelonMod mod)
    {
        Mod = mod;
    }

    /// <summary>
    /// Adds a custom display name that will replace this entry's mod name in the mod menu.
    /// </summary>
    /// <param name="displayName">The string containing the custom display name.</param>
    /// <returns>This mod entry with the custom display name.</returns>
    public ModEntry AddDisplayName(string displayName)
    {
        DisplayName = displayName;
        return this;
    }

    /// <summary>
    /// Adds a description to this mod entry in the mod menu, displaying it under the mod's name.
    /// If no description is provided, it will be replaced by the author and version number of the mod.
    /// </summary>
    /// <param name="description">A string containing the description of this mod.</param>
    /// <returns>This mod entry with the new description.</returns>
    public ModEntry AddDescription(string description)
    {
        Description = description;
        return this;
    }

    // TODO: Add the required functionality and uncomment this method
    //public ModEntry AddImage(Texture2D image)
    //{
    //    Image = image;
    //    return this;
    //}

    /// <summary>
    /// Adds a config to this mod using a static config class.
    /// To add input fields, use the static methods provided by <see cref="BloomEngine.Config.ConfigMenu"/> and make the result publicly accessible.
    /// </summary>
    /// <param name="staticConfig">The static class type containing public input fields to be registered in the config menu.</param>
    /// <returns>This mod entry with the config added.</returns>
    public ModEntry AddConfig(Type staticConfig)
    {
        ConfigInputFields = new List<IInputField>();

        // Use reflection to find all fields and properties that define input fields
        var fields = staticConfig.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            // Null instance for static class
            if (field.GetValue(null) is IInputField inputField)
                ConfigInputFields.Add(inputField);
        }

        HasConfig = ConfigInputFields.Count > 0;
        return this;
    }

    /// <summary>
    /// Registers this <see cref="ModEntry"/> and adds it to the mod menu with the provided information.
    /// </summary>
    /// <remarks>
    /// Calling this method invokes the <see cref="ModMenu.OnModRegistered"/> event.
    /// </remarks>
    /// <returns>The registered entry.</returns>
    public ModEntry Register()
    {
        ModMenu.RegisterModEntry(this);
        return this;
    }
}