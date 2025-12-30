using BloomEngine.ModMenu.Services;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// Creates a <see cref="bool"/> input field in the form of a checkbox in your mod's config menu and returns it.
/// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
/// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
/// </summary>
/// <param name="name">The name of this input checkbox, which will be displayed in the config menu.</param>
/// <param name="description">The description of this input checkbox, which will be displayed in the config menu.</param>
/// <param name="defaultValue">This default value of this config input checkbox.</param>
/// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
/// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
public sealed class BoolConfigInput(string name, string description, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null)
    : BaseConfigInputT<bool>(name, description, defaultValue, onValueChanged, onInputChanged, null, null)
{
    public override Type InputObjectType { get; } = typeof(Toggle);
    public Toggle Toggle { get; private set; }

    internal override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Toggle = inputObject.GetComponent<Toggle>();
    }

    internal override void UpdateFromUI() => Value = Toggle.isOn;
    internal override void RefreshUI() => Toggle.SetIsOnWithoutNotify(Value);   
}