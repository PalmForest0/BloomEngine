using UnityEngine.UI;

namespace BloomEngine.Inputs;

public class BoolInputField : InputFieldBase<bool>
{
    public Toggle Toggle { get; set; }

    public override void UpdateValue() => Value = Toggle.isOn;
    public override void RefreshUI() => Toggle.SetIsOnWithoutNotify(Value);
}