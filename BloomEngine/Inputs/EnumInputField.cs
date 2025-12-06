using Il2CppSource.UI;

namespace BloomEngine.Inputs;

public class EnumInputField : InputFieldBase<Enum>
{
    public ReloadedDropdown Dropdown
    {
        get => field;
        set
        {
            field = value;
            values = Enum.GetValues(value.GetType()).Cast<Enum>().ToList();
        }
    }

    private List<Enum> values;

    public override void UpdateValue() => Value = values[Dropdown.value];
    public override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(values.IndexOf(Value));
        Dropdown.RefreshShownValue();
    }
}