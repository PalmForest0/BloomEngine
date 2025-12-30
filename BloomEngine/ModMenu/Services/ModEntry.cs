using BloomEngine.Modules.Config.Inputs;
using BloomEngine.Utilities;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.ModMenu.Services;

/// <summary>
/// A mod entry that shows up in the mod menu if <see cref="Register"/> is called.
/// </summary>
public class ModEntry(MelonMod mod)
{
    /// <summary>
    /// The MelonLoader mod this entry belongs to.
    /// </summary>
    public MelonMod Mod { get; } = mod;

    /// <summary>
    /// The display name that shows up in the mod menu for this entry.
    /// </summary>
    public string DisplayName { get; private set; } = GetDefaultModName(mod);

    /// <summary>
    /// The description that shows up under the mod name in the mod menu.
    /// </summary>
    public string Description { get; private set; } = GetDefaultModDescription(mod);

    /// <summary>
    /// Sprite that shows up as the icon for this mod in the mod menu.
    /// </summary>
    public Sprite Icon { get; private set; }

    /// <summary>
    /// A list of all the registered config input fields for this mod.
    /// </summary>
    /// <remarks>
    /// It is not recommended to modify this list directly, use <see cref="AddConfig(Type)"/> instead.
    /// </remarks>
    public List<IConfigInput> ConfigInputFields { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this mod has a registered config.
    /// </summary>
    public bool HasConfig { get; private set; }

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

    /// <summary>
    /// Adds a custom icon to this entry in the mod menu.
    /// </summary>
    /// <param name="iconSprite">
    /// The iconSprite to replace the default icon with.<br/>
    /// To load a iconSprite, you can add it to your mod as an embedded resource and load it with <see cref="AssetHelper.LoadSprite(string, float)"/>.
    /// </param>
    /// <returns>This mod entry with the new icon.</returns>
    public ModEntry AddIcon(Sprite iconSprite)
    {
        Icon = iconSprite;
        return this;
    }

    /// <summary>
    /// Adds a config to this mod using a static config class.
    /// To add input fields, use the static methods provided by <see cref="Config.ConfigService"/> and make the result publicly accessible.
    /// </summary>
    /// <param name="staticConfig">The static class type containing public input fields to be registered in the config menu.</param>
    /// <returns>This mod entry with the config added.</returns>
    public ModEntry AddConfig(Type staticConfig)
    {
        ConfigInputFields = new List<IConfigInput>();

        // Use reflection to find all fields and properties that define input fields
        var fields = staticConfig.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            // Null instance for static class
            if (field.GetValue(null) is IConfigInput inputField)
                ConfigInputFields.Add(inputField);
        }

        HasConfig = ConfigInputFields.Count > 0;
        return this;
    }

    /// <summary>
    /// Registers this <see cref="ModEntry"/> and adds it to the mod menu with the provided information.
    /// </summary>
    /// <remarks>
    /// Calling this method invokes the <see cref="ModMenuService.OnModRegistered"/> event.
    /// </remarks>
    /// <returns>The registered entry.</returns>
    public ModEntry Register()
    {
        ModMenuService.RegisterModEntry(this);
        return this;
    }

    internal static string GetDefaultModName(MelonMod mod) => mod?.Info?.Name ?? "???";
    internal static string GetDefaultModDescription(MelonMod mod) => $"By {mod?.Info?.Author ?? "???"}\nVersion {mod?.Info?.Version ?? "???"}";
}