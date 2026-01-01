using BloomEngine.Config.Services;
using BloomEngine.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="float"/> input.<br/>
/// To create a <see cref="FloatConfigInput"/>, use <see cref="ConfigService.CreateFloat(string, string, float, float, float, ConfigInputOptions{float})"/>
/// </summary>
public sealed class FloatConfigInput : TypedConfigInput<float>
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
    /// The UI slider which corresponds to this config input in the config panel.
    /// </summary>
    public Slider Slider { get; private set; }

    internal FloatConfigInput(string name, string description, float defaultValue, float minValue, float maxValue, ConfigInputOptions<float> options) : base(name, description, defaultValue, options)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Slider = UIHelper.CreateSlider(InputObjectName, parent, Value, MinValue, MaxValue, onValueChanged: _ => OnUIChanged());
        return Slider.gameObject;
    }

    internal override void UpdateFromUI() => Value = Slider.value;
    internal override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}