using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Internal;

public sealed class FloatConfigInput(string name, float value, float minValue, float maxValue, Action<float> onValueChanged, Action onInputChanged, Func<float, float> transformValue, Func<float, bool> validateValue) : BaseConfigInputT<float>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;

    public Slider Slider { get; private set; }

    internal override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Slider = inputObject.GetComponent<Slider>();
    }

    internal override void UpdateFromUI() => Value = Slider.value;
    internal override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}