using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// Creates a <see cref="string"/> input field in the form of a textbox in your mod's config menu and returns it.
/// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
/// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
/// </summary>
/// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
/// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
/// <param name="defaultValue">This default value of this config input field.</param>
/// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
/// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
/// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
/// <param name="validateValue">A function to validate the new value before it is updated.</param>
public sealed class StringConfigInput(string name, string description, string defaultValue, Action<string> onValueChanged = null, Action onInputChanged = null, Func<string, string> transformValue = null, Func<string, bool> validateValue = null)
    : TypedConfigInput<string>(name, description, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedInputField Textbox { get; private set; }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Textbox = UIHelper.CreateTextField(InputObjectName, parent, ValueType.Name, onTextChanged: _ => OnUIChanged());
        return Textbox.gameObject;
    }

    internal override void UpdateFromUI() => Value = Textbox.text;
    internal override void RefreshUI() => Textbox.SetTextWithoutNotify(Value);
}