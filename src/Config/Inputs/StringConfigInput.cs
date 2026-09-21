using BloomEngine.Config.Inputs.Base;
using BloomEngine.UI;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="string"/> input.<br/>
/// To create a <see cref="StringConfigInput"/>, use <see cref="ConfigService.CreateString(string, string, string)"/>
/// </summary>
public sealed class StringConfigInput : TypedConfigInput<string, StringConfigInput>
{
    /// <summary>
    /// The UI textbox which corresponds to this config input in the config panel.
    /// </summary>
    public ReloadedInputField Textbox { get; private set; } = null!;

    internal StringConfigInput(string name, string description, string defaultValue) : base(name, description, defaultValue) { }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        Textbox = UIHelper.CreateTextField(name, parent, ValueType.Name, onTextChanged: _ => HandleInputChanged());
        return Textbox.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = Textbox.text;

    /// <inheritdoc/>
    protected override void SetDisplayedValue(string value) => Textbox.SetTextWithoutNotify(value);
}