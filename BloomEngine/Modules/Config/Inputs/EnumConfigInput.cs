using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Modules.Config.Inputs;

public sealed class EnumConfigInput(string name, Enum value, Action<Enum> onValueChanged, Action onInputChanged, Func<Enum, Enum> transformValue, Func<Enum, bool> validateValue) : BaseConfigInput<Enum>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedDropdown Dropdown { get; private set; }

    public override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Dropdown = inputObject.GetComponent<ReloadedDropdown>();
    }

    public override void UpdateFromUI() => Value = GetOptions()[Dropdown.value];
    public override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(GetOptions().IndexOf(Value));
        Dropdown.RefreshShownValue();
    }

    private List<Enum> GetOptions() => Enum.GetValues(Value.GetType()).Cast<Enum>().ToList();
}