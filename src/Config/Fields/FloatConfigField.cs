using BloomEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="float"/> value using a slider.
/// </summary>
public sealed class FloatConfigField(string identifier, string displayName, float defaultValue)
    : ConfigField<float, FloatConfigField>(identifier, displayName, defaultValue)
{
    /// <summary>
    /// The minimum value constraint of this <see cref="float"/> input slider.
    /// This is <c>0f</c> by default, but can be changed using <see cref="WithRange"/>.
    /// </summary>
    public float MinValue { get; private set; }

    /// <summary>
    /// The maximum value constraint of this <see cref="float"/> input slider.
    /// This is <c>1f</c> by default, but can be changed using <see cref="WithRange"/>.
    /// </summary>
    public float MaxValue { get; private set; } = 1f;

    /// <summary>
    /// The UI slider which corresponds to this config field in the config panel.
    /// </summary>
    public Slider Slider { get; private set; } = null!;

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        Slider = UIHelper.CreateSlider(name, parent, Value, MinValue, MaxValue, onValueChanged: _ => HandleInputChanged());
        return Slider.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = Slider.value;

    /// <inheritdoc/>
    protected override void SetDisplayedValue(float value) => Slider.SetValueWithoutNotify(value);

    /// <summary>
    /// Defines a custom <see langword="float"/> range for this config field. The default is 0f - 1f.
    /// </summary>
    /// <param name="minValue">The lowest possible value that this config field should allow.</param>
    /// <param name="maxValue">The highest possible value that this config field should allow.</param>
    /// <returns>This config field, with its new range set.</returns>
    public FloatConfigField WithRange(float minValue, float maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        return this;
    }
}