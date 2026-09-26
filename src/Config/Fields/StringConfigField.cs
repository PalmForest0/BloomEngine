using BloomEngine.Config.Fields.Base;
using BloomEngine.UI;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="string"/> value using a textbox.
/// To create a <see cref="StringConfigField"/>, use <see cref="ConfigService.CreateString(string, string, string)"/>
/// </summary>
public sealed class StringConfigField : TypedConfigField<string, StringConfigField>
{
    /// <summary>
    /// The UI textbox which corresponds to this config field in the config panel.
    /// </summary>
    public ReloadedInputField Textbox { get; private set; } = null!;

    internal StringConfigField(string name, string description, string defaultValue) : base(name, description, defaultValue) { }

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