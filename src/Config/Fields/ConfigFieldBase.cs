using MelonLoader;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// Represents the base typeless structure of a config field, which is extended by
/// <see cref="ConfigField{T,TSelf}"/> to provide type-specific functionality.
/// </summary>
public abstract class ConfigFieldBase(string identifier, string displayName)
{
    /// <summary>
    /// The internal identifier of this config field that is used for saving to MelonPreferences.
    /// </summary>
    public string Identifier { get; } = identifier.Trim().Replace(" ", "");
    
    /// <summary>
    /// The display name shown for this config field in the config panel.
    /// </summary>
    public string DisplayName { get; set; } = displayName;

    /// <summary>
    /// The description shown for this config field in the config panel.
    /// </summary>
    public string? Description { get; protected set; } 

    /// <summary>
    /// Creates the correct UI input object for this config field.
    /// </summary>
    /// <param name="parent">The parent under which this UI object should be instantiated.</param>
    /// <param name="name">The string to use as the name of the UI object.</param>
    /// <returns>The created input <see cref="GameObject"/>.</returns>
    protected internal abstract GameObject CreateInputObject(RectTransform parent, string name);

    /// <summary>
    /// Created the <see cref="MelonPreferences"/> entry to which the value of this config field will be saved.
    /// </summary>
    /// <param name="melonCategory">The category to save the melon entry to.</param>
    internal abstract void CreateMelonEntry(MelonPreferences_Category melonCategory);

    /// <summary>
    /// Sets the current value shown in the UI input object to the default value without updating the stored value.
    /// </summary>
    internal abstract void ResetInput();

    /// <summary>
    /// Updates the stored value of this config field to contain the current value in the UI input object.
    /// </summary>
    protected internal abstract void ApplyInput();

    /// <summary>
    /// Updates the UI input object with the current value stored by this config field.
    /// </summary>
    internal abstract void RefreshInput();
}