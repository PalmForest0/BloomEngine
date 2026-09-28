using BloomEngine.Extensions;
using BloomEngine.UI;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="string"/> value using a textbox.
/// </summary>
public sealed class StringConfigField(string identifier, string displayName, string defaultValue)
    : ConfigField<string, StringConfigField>(identifier, displayName, defaultValue)
{
    /// <summary>
    /// The UI textbox which corresponds to this config field in the config panel.
    /// </summary>
    public ReloadedInputField? Textbox { get; private set; }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        Textbox = UIHelper.CreateTextbox(name, Value, parent, onTextChanged: _ => HandleInputChanged());
        return Textbox.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput()
    {
        if(Textbox.NotNull())
            Value = Textbox.text;
    }

    /// <inheritdoc/>
    protected override void SetDisplayedValue(string value) => Textbox.OrNull()?.SetTextWithoutNotify(value);
}