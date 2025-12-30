using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Internal;

public sealed class StringConfigInput(string name, string value, Action<string> onValueChanged, Action onInputChanged, Func<string, string> transformValue, Func<string, bool> validateValue)
    : BaseConfigInputT<string>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedInputField Textbox { get; private set; }

    internal override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        inputObject.GetComponent<ReloadedInputField>();
    }

    internal override void UpdateFromUI() => Value = Textbox.text;
    internal override void RefreshUI() => Textbox.SetTextWithoutNotify(Value);
}