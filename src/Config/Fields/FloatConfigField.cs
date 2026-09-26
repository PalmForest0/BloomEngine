using BloomEngine.Config.Fields.Base;
using BloomEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="float"/> value using a slider.
/// To create a <see cref="FloatConfigField"/>, use <see cref="ConfigService.CreateFloat(string, string, float, float, float)"/>
/// </summary>
public sealed class FloatConfigField : TypedConfigField<float, FloatConfigField>
{
    /// <summary>
    /// The minimum value constraint of this <see cref="float"/> input slider.
    /// </summary>
    public float MinValue { get; private init; }

    /// <summary>
    /// The maximum value constraint of this <see cref="float"/> input slider.
    /// </summary>
    public float MaxValue { get; private init; }

    /// <summary>
    /// The UI slider which corresponds to this config field in the config panel.
    /// </summary>
    public Slider Slider { get; private set; } = null!;

    internal FloatConfigField(string name, string description, float defaultValue, float minValue, float maxValue) : base(name, description, defaultValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

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
}