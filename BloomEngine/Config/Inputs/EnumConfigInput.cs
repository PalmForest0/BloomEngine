using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Config.Internal;

public sealed class EnumConfigInput(string name, Enum value, Action<Enum> onValueChanged, Action onInputChanged, Func<Enum, Enum> transformValue, Func<Enum, bool> validateValue) : BaseConfigInputT<Enum>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedDropdown Dropdown { get; private set; }

    internal override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Dropdown = inputObject.GetComponent<ReloadedDropdown>();
    }

    internal override void UpdateFromUI() => Value = GetOptions()[Dropdown.value];
    internal override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(GetOptions().IndexOf(Value));
        Dropdown.RefreshShownValue();
    }

    private List<Enum> GetOptions() => Enum.GetValues(Value.GetType()).Cast<Enum>().ToList();
}