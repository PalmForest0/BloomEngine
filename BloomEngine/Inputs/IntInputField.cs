using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Inputs;

public class IntInputField(string name, int value, Action<int> onValueChanged, Action onInputChanged, Func<int, int> transformValue, Func<int, bool> validateValue) : InputFieldBase<int>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public override Type InputObjectType => typeof(ReloadedInputField);
    public ReloadedInputField Textbox => ((GameObject)InputObject).GetComponent<ReloadedInputField>();

    public override void UpdateFromUI() => Value = (int)TextHelper.ValidateNumericInput(Textbox.text, typeof(int));
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value.ToString());
    public override void OnUIChanged()
    {
        Textbox.SetTextWithoutNotify(TextHelper.SanitiseNumericInput(Textbox.text));
        base.OnUIChanged();
    }
}