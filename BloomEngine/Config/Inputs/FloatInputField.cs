using Il2CppReloaded.Input;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Inputs;

public class FloatInputField(string name, float value, float minValue, float maxValue, Action<float> onValueChanged, Action onInputChanged, Func<float, float> transformValue, Func<float, bool> validateValue) : InputFieldBase<float>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public override Type InputObjectType => typeof(Slider);
    public Slider Slider => ((GameObject)InputObject).GetComponent<Slider>();

    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;

    public override void UpdateFromUI() => Value = Slider.value;
    public override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}