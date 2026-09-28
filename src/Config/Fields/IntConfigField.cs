using System.Globalization;
using BloomEngine.Helpers;
using BloomEngine.UI;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes an <see cref="int"/> value using a numeric textbox.
/// </summary>
public sealed class IntConfigField(string identifier, string displayName, int defaultValue)
    : ConfigField<int, IntConfigField>(identifier, displayName, defaultValue)
{
    /// <summary>
    /// The UI textbox which corresponds to this config field in the config panel.
    /// </summary>
    public ReloadedInputField Textbox { get; private set; } = null!;

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        Textbox = UIHelper.CreateTextbox(name, Value.ToString(CultureInfo.InvariantCulture), parent, onTextChanged: _ => HandleInputChanged());
        return Textbox.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = (int)StringHelper.ValidateNumericInput(Textbox.text, typeof(int));

    /// <inheritdoc/>
    protected override void SetDisplayedValue(int value) => Textbox.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc/>
    internal override void HandleInputChanged()
    {
        // Perform basic sanitization on live input change
        Textbox.SetTextWithoutNotify(StringHelper.SanitizeNumericInput(Textbox.text));
        base.HandleInputChanged();
    }
}