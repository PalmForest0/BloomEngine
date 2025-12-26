using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Modules.Config.Inputs;

public class BoolInputField(string name, bool value, Action<bool> onValueChanged, Action onInputChanged, Func<bool, bool> transformValue, Func<bool, bool> validateValue) : InputFieldBase<bool>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public override Type InputObjectType => typeof(Toggle);
    public Toggle Toggle => ((GameObject)InputObject).GetComponent<Toggle>();

    public override void UpdateFromUI() => Value = Toggle.isOn;
    public override void RefreshUI() => Toggle.SetIsOnWithoutNotify(Value);
}