using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Modules.Config.Inputs;

public class StringInputField(string name, string value, Action<string> onValueChanged, Action onInputChanged, Func<string, string> transformValue, Func<string, bool> validateValue) : InputFieldBase<string>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public override Type InputObjectType => typeof(ReloadedInputField);
    public ReloadedInputField Textbox => ((GameObject)InputObject).GetComponent<ReloadedInputField>();

    public override void UpdateFromUI() => Value = Textbox.text;
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value);
}