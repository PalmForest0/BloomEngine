using BloomEngine.Config.Fields.Base;
using BloomEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="bool"/> value using a checkbox.
/// To create a <see cref="BoolConfigField"/>, use <see cref="ConfigService.CreateBool(string, string, bool)"/>
/// </summary>
public sealed class BoolConfigField : TypedConfigField<bool, BoolConfigField>
{
    /// <summary>
    /// The UI checkbox element which corresponds to this config field in the config panel.
    /// </summary>
    public Toggle Checkbox { get; private set; } = null!;

    internal BoolConfigField(string name, string description, bool defaultValue) : base(name, description, defaultValue) { }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        var wrapper = UIHelper.CreateUIWrapper(parent, name);

        Checkbox = UIHelper.CreateCheckbox("Toggle_Internal", wrapper, Value, onValueChanged: _ => HandleInputChanged());
        var toggleRect = Checkbox.gameObject.GetComponent<RectTransform>();
        UIHelper.SetParentAndStretch(toggleRect, wrapper);

        toggleRect.anchoredPosition += new Vector2(0, -35);

        return wrapper.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = Checkbox.isOn;

    /// <inheritdoc/>
    protected override void SetDisplayedValue(bool value) => Checkbox.SetIsOnWithoutNotify(value);
}