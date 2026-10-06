using MelonLoader;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// Represents the base typeless structure of a config field, which is extended by
/// <see cref="ConfigField{T,TSelf}"/> to provide type-specific functionality.
/// </summary>
public abstract class ConfigFieldBase(string name)
{
    /// <summary>
    /// The internal identifier of this config field that is used for saving to MelonPreferences.
    /// </summary>
    public string Identifier { get; } = GetIdentifierFromName(name);
    
    /// <summary>
    /// The display name shown for this config field in the config panel.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The description popup shown for this config field in the config panel.
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Returns whether the UI input object for this config field exists.
    /// </summary>
    public bool InputObjectCreated { get; internal set; }
    
    /// <summary>
    /// Creates the UI input object for this config field.
    /// </summary>
    /// <param name="parent">The parent under which this UI object should be instantiated.</param>
    /// <param name="name">The string to use as the name of the UI object.</param>
    /// <returns>The created UI input GameObject.</returns>
    internal abstract GameObject CreateInput(RectTransform parent, string name);

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
    internal abstract void ApplyInput();

    /// <summary>
    /// Updates the UI input object with the current value stored by this config field.
    /// </summary>
    internal abstract void UpdateInput();
    
    /// <summary>
    /// Converts a config field's display name into an identifier string that is suitable for saving to MelonPreferences.
    /// </summary>
    /// <param name="name">The display name to convert to an identifier</param>
    /// <returns>The identifier string created from the provided display name.</returns>
    protected static string GetIdentifierFromName(string name) => string.Join("_", name.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}