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
    
    private Slider slider = null!;

    /// <inheritdoc/>
    protected override GameObject CreateInputObject(RectTransform parent, string name, Action<float> onInputChanged)
    {
        slider = UIHelper.CreateSlider(name, parent, Value, MinValue, MaxValue, onInputChanged);
        return slider.gameObject;
    }

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
    
    /// <inheritdoc/>
    protected override float GetInputValue() => slider.value;

    /// <inheritdoc/>
    protected override void SetInputValue(float inputValue) => slider.SetValueWithoutNotify(inputValue);
}