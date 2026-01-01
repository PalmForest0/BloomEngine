using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// Creates an <see cref="Enum"/> input field in the form of a dropdown in your mod's config menu and returns it.
/// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
/// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
/// </summary>
/// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
/// <param name="description">The description of this input slider, which will be displayed in the config menu.</param>
/// <param name="defaultValue">This default value of this config input field.</param>
/// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
/// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
/// <param name="validateValue">A function to validate the new value before it is updated.</param>
public sealed class EnumConfigInput : TypedConfigInput<Enum>
{
    public ReloadedDropdown Dropdown { get; private set; }

    internal EnumConfigInput(string name, string description, Enum defaultValue, ConfigInputOptions<Enum> options) : base(name, description, defaultValue, options) { }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Dropdown = UIHelper.CreateDropdown(InputObjectName, parent, ValueType, Convert.ToInt32(Value), onValueChanged: _ => OnUIChanged());
        return Dropdown.gameObject;
    }

    internal override void UpdateFromUI() => Value = GetOptions()[Dropdown.value];
    internal override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(GetOptions().IndexOf(Value));
        Dropdown.RefreshShownValue();
    }

    private List<Enum> GetOptions() => Enum.GetValues(Value.GetType()).Cast<Enum>().ToList();
}