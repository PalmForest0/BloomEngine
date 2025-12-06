using UnityEngine.UI;

namespace BloomEngine.Inputs;

public class FloatInputField : InputFieldBase<float>
{
    public Slider Slider { get; set; }

    public override void UpdateValue() => Value = Slider.value;
    public override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}