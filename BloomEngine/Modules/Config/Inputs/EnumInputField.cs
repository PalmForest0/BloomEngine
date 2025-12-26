using Il2CppReloaded.Input;
using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Modules.Config.Inputs;

public class EnumInputField(string name, Enum value, Action<Enum> onValueChanged, Action onInputChanged, Func<Enum, Enum> transformValue, Func<Enum, bool> validateValue) : InputFieldBase<Enum>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public override Type InputObjectType => typeof(ReloadedDropdown);
    public ReloadedDropdown Dropdown => ((GameObject)InputObject).GetComponent<ReloadedDropdown>();

    public override void UpdateFromUI() => Value = GetOptions()[Dropdown.value];
    public override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(GetOptions().IndexOf(Value));
        Dropdown.RefreshShownValue();
    }


    private List<Enum> GetOptions() => Enum.GetValues(Value.GetType()).Cast<Enum>().ToList();
}