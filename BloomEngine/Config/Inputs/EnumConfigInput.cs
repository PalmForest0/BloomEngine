using BloomEngine.Config.Services;
using BloomEngine.Utilities;
using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="Enum"/> input.<br/>
/// To create an <see cref="EnumConfigInput"/>, use <see cref="ConfigService.CreateEnum(string, string, Enum, ConfigInputOptions{Enum})"/>
/// </summary>
public sealed class EnumConfigInput : TypedConfigInput<Enum>
{
    /// <summary>
    /// The UI dropdown which corresponds to this config input in the config panel.
    /// </summary>
    public ReloadedDropdown Dropdown { get; private set; }

    /// <summary>
    /// Contains a list of the individual options of the value enum type.
    /// </summary>
    private List<Enum> options;

    internal EnumConfigInput(string name, string description, Enum defaultValue, ConfigInputOptions<Enum> options) : base(name, description, defaultValue, options) { }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Dropdown = UIHelper.CreateDropdown(InputObjectName, parent, ValueType, Convert.ToInt32(Value), onValueChanged: _ => OnUIChanged());
        options = Enum.GetValues(ValueType).Cast<Enum>().ToList();

        return Dropdown.gameObject;
    }

    internal override void UpdateFromUI() => Value = options[Dropdown.value];
    internal override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(options.IndexOf(Value));
        Dropdown.RefreshShownValue();
    }
}