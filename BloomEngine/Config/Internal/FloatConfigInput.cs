using Il2CppReloaded.Input;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Internal;

public sealed class FloatConfigInput(string name, float value, float minValue, float maxValue, Action<float> onValueChanged, Action onInputChanged, Func<float, float> transformValue, Func<float, bool> validateValue) : BaseConfigInput<float>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;

    public Slider Slider { get; private set; }

    public override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Slider = inputObject.GetComponent<Slider>();
    }

    public override void UpdateFromUI() => Value = Slider.value;
    public override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}