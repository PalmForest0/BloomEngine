using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Internal;

public sealed class BoolConfigInput(string name, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null, Func<bool, bool> transformValue = null, Func<bool, bool> validateValue = null)
    : BaseConfigInputT<bool>(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public Toggle Toggle { get; private set; }

    internal override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Toggle = inputObject.GetComponent<Toggle>();
    }

    internal override void UpdateFromUI() => Value = Toggle.isOn;
    internal override void RefreshUI() => Toggle.SetIsOnWithoutNotify(Value);   
}