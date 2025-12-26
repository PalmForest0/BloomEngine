using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Modules.Config.Inputs;

public sealed class StringInputField(string name, string value, Action<string> onValueChanged, Action onInputChanged, Func<string, string> transformValue, Func<string, bool> validateValue) : InputFieldBase<string>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedInputField Textbox { get; private set; }

    public override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        inputObject.GetComponent<ReloadedInputField>();
    }

    public override void UpdateFromUI() => Value = Textbox.text;
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value);
}