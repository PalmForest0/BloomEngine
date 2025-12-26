using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Modules.Config.Inputs;

public sealed class BoolInputField(string name, bool value, Action<bool> onValueChanged, Action onInputChanged, Func<bool, bool> transformValue, Func<bool, bool> validateValue) : InputFieldBase<bool>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public Toggle Toggle { get; private set; }

    public override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Toggle = inputObject.GetComponent<Toggle>();
    }

    public override void UpdateFromUI() => Value = Toggle.isOn;
    public override void RefreshUI() => Toggle.SetIsOnWithoutNotify(Value);   
}