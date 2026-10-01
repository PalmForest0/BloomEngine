using BloomEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="bool"/> value using a checkbox.
/// </summary>
public sealed class BoolConfigField(string identifier, string displayName, bool defaultValue)
    : ConfigField<bool, BoolConfigField>(identifier, displayName, defaultValue)
{
    private Toggle checkbox = null!;

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        var wrapper = UIHelper.CreateUIWrapper(parent, name);

        checkbox = UIHelper.CreateCheckbox("Toggle_Internal", wrapper, Value, onValueChanged: _ => HandleInputChanged());
        var toggleRect = checkbox.gameObject.GetComponent<RectTransform>();
        UIHelper.SetParentAndStretch(toggleRect, wrapper);

        toggleRect.anchoredPosition += new Vector2(0, -35);

        return wrapper.gameObject;
    }

    /// <inheritdoc/>
    protected override bool GetInputValue() => checkbox.isOn;

    /// <inheritdoc/>
    protected override void SetInputValue(bool inputValue) => checkbox.SetIsOnWithoutNotify(inputValue);
}