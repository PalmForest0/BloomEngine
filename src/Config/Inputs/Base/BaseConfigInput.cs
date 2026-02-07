using MelonLoader;
using UnityEngine;

namespace BloomEngine.Config.Inputs.Base;

/// <summary>
/// Represents the base typeless structure of a config input, which is extended by
/// <see cref="TypedConfigInput{T}"/> to provide type-specific functionality.
/// </summary>
public abstract class BaseConfigInput
{
    /// <summary>
    /// The display name of this config input, which is displayed in the config menu.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The description of this input field, which is displayed in the config menu.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The type of value stored within this config input.
    /// </summary>
    public Type ValueType { get; protected set; }

    /// <summary>
    /// Gets the name of the UI element that corresponds to this config input.
    /// </summary>
    protected string InputObjectName => $"ConfigInput_{Name.Trim().Replace(" ", "")}";

    /// <summary>
    /// Creates the appropriate UI object for this config input.
    /// </summary>
    /// <param name="parent">The parent under which this UI object should be instantiated.</param>
    /// <returns>The created <see cref="GameObject"/>.</returns>
    internal abstract GameObject CreateInputObject(RectTransform parent);

    /// <summary>
    /// Created the <see cref="MelonPreferences"/> entry for this config input to which the value will be saved.
    /// </summary>
    /// <param name="melonCategory">The category to save the melon entry to.</param>
    internal abstract void CreateMelonEntry(MelonPreferences_Category melonCategory);

    /// <summary>
    /// Sets the current value shown in the UI to the default value without updating the actual value.
    /// </summary>
    internal abstract void ResetValueUI();

    /// <summary>
    /// Updates the value of this config input to the current value stored in the UI input.
    /// </summary>
    internal abstract void UpdateFromUI();

    /// <summary>
    /// Updates the UI with the current value saved in this config input.
    /// </summary>
    internal abstract void RefreshUI();

    /// <summary>
    /// Invokes the <see cref="TypedConfigInput{T}.OnInputChanged"/> action and performs any type-specific logic.
    /// </summary>
    internal abstract void OnUIChanged();
}