using BloomEngine.Config.Inputs.Base;
using BloomEngine.Config.Services;
using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="string"/> input.<br/>
/// To create a <see cref="StringConfigInput"/>, use <see cref="ConfigService.CreateString(string, string, string, ConfigInputOptions{string})"/>
/// </summary>
public sealed class StringConfigInput : TypedConfigInput<string>
{
    /// <summary>
    /// The UI textbox which corresponds to this config input in the config panel.
    /// </summary>
    public ReloadedInputField Textbox { get; private set; }

    internal StringConfigInput(string name, string description, string defaultValue, ConfigInputOptions<string> options) : base(name, description, defaultValue, options) { }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Textbox = UIHelper.CreateTextField(InputObjectName, parent, ValueType.Name, onTextChanged: _ => OnUIChanged());
        return Textbox.gameObject;
    }

    internal override void UpdateFromUI() => Value = Textbox.text;
    protected override void SetDisplayedValue(string value) => Textbox.SetTextWithoutNotify(value);
}