using BloomEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="float"/> value using a slider.
/// </summary>
public sealed class FloatConfigField(string name, float defaultValue)
    : ConfigField<float, FloatConfigField>(name, defaultValue)
{
    /// <inheritdoc/>
    protected override string Comment => $"Float Range: {MinValue}f - {MaxValue}f";
    
    /// <summary>
    /// The minimum value constraint of this <see cref="float"/> input slider, which is <c>0f</c> by default.
    /// </summary>
    public float MinValue { get; init; } = 0f;

    /// <summary>
    /// The maximum value constraint of this <see cref="float"/> input slider, which is <c>1f</c> by default.
    /// </summary>
    public float MaxValue { get; init; } = 1f;

    private Slider slider = null!;

    /// <inheritdoc/>
    protected override GameObject CreateInputObject(RectTransform parent, string name, Action<float> onInputChanged)
    {
        slider = UIHelper.CreateSlider(name, parent, Value, MinValue, MaxValue, onInputChanged);
        return slider.gameObject;
    }
    
    /// <inheritdoc/>
    protected override float GetInputValue() => slider.value;

    /// <inheritdoc/>
    protected override void SetInputValue(float inputValue) => slider.SetValueWithoutNotify(inputValue);
}