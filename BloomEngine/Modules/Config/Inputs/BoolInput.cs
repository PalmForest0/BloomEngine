using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Modules.Config.Inputs;

public sealed class BoolInput(string name, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null, Func<bool, bool> transformValue = null, Func<bool, bool> validateValue = null)
    : BaseInput<bool>(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue)
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