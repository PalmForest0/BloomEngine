using Il2CppReloaded.Input;

namespace BloomEngine.Inputs;

public class StringInputField : InputFieldBase<string>
{
    public ReloadedInputField Textbox { get; set; }

    public override void UpdateValue() => Value = Textbox.text;
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value);
}