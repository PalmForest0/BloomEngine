using BloomEngine.Attributes;
using BloomEngine.Config.Inputs.Base;
using BloomEngine.Config.Services;
using BloomEngine.Utilities;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.ModMenu.Services;

/// <summary>
/// A mod entry that shows up in the mod menu if <see cref="Register"/> is called.
/// </summary>
public sealed class ModMenuEntry(MelonMod mod)
{
    /// <summary>
    /// The MelonLoader mod this entry belongs to.
    /// </summary>
    public MelonMod Mod { get; } = mod;

    /// <summary>
    /// A string that represents the unique identifier for this mod entry.
    /// </summary>
    public string Identifier { get; private set; } = mod.Info.Name.Trim().Replace(" ", "");

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
    /// This mod's config that will be available in-game. To add a config, use <see cref="AddConfigInputs(BaseConfigInput[])"/> or <see cref="AddConfigClass(Type)"/>.
    /// </summary>
    public ModConfig Config { get; private set; }

    /// <summary>
    /// Adds a custom display name that will replace this entry's mod name in the mod menu.
    /// </summary>
    /// <param name="displayName">The string containing the custom display name.</param>
    /// <returns>This mod entry with the custom display name.</returns>
    public ModMenuEntry AddDisplayName(string displayName)
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
    public ModMenuEntry AddDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    /// Adds a custom icon to this entry in the mod menu.
    /// </summary>
    /// <param name="iconSprite">
    /// The <see cref="Sprite"/> to replace the default icon with. To load a <see cref="Sprite"/>,
    /// you can add it to your mod as an embedded resource and load it with <see cref="AssetHelper.LoadSprite{TMarker}(string, float)"/>.
    /// </param>
    /// <returns>This mod entry with the new icon.</returns>
    public ModMenuEntry AddIcon(Sprite iconSprite)
    {
        Icon = iconSprite;
        return this;
    }

    /// <summary>
    /// Creates a config for this mod and adds all the provided inputs, adding them to the existing ones if a config alredy exists.<br/>
    /// To create a config input, use the static methods provided by <see cref="ConfigService"/>.
    /// </summary>
    /// <param name="inputs">An array of inputs to create the config with.</param>
    /// <returns>This mod entry with the added config inputs.</returns>
    public ModMenuEntry AddConfigInputs(params BaseConfigInput[] inputs)
    {
        if (Config is null)
            Config = new ModConfig(this, inputs);
        else Config.ConfigInputs.AddRange(inputs);

        return this;
    }

    /// <summary>
    /// Adds a config to this mod using a static config class. To add input fields,
    /// use the static methods provided by <see cref="ConfigService"/> and make the config inputs publicly accessible.
    /// </summary>
    /// <param name="staticConfig">The static class type containing public input fields to be registered in the config menu.</param>
    /// <returns>This mod entry with the config added.</returns>
    public ModMenuEntry AddConfigClass(Type staticConfig)
    {
        Config = new ModConfig(this, staticConfig);
        return this;
    }

    /// <summary>
    /// Registers this <see cref="ModMenuEntry"/> and adds it to the mod menu with the provided information.<br/>
    /// Calling this method invokes the <see cref="ModMenuService.OnModRegistered"/> event.
    /// </summary>
    /// <returns>The registered mod menu entry.</returns>
    public ModMenuEntry Register()
    {
        ModMenuService.RegisterModEntry(this);
        Config?.Save(false);

        // Register all classes with a custom attribute
        foreach(var type in Mod.MelonAssembly.Assembly.GetTypes())
        {
            if(type.IsSubclassOf(typeof(UnityEngine.Object)) && type.GetCustomAttribute<RegisterInIl2CppAttribute>() is not null)
                ClassInjector.RegisterTypeInIl2Cpp(type);
        }

        return this;
    }

    internal static string GetDefaultModName(MelonMod mod) => mod?.Info?.Name ?? "???";
    internal static string GetDefaultModDescription(MelonMod mod) => $"By {mod?.Info?.Author ?? "???"}\nVersion {mod?.Info?.Version ?? "???"}";
}