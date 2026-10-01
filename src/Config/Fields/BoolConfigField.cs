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
    protected override GameObject CreateInputObject(RectTransform parent, string name, Action<BoolConfigField> onInputChanged)
    {
        var wrapperRect = UIHelper.CreateUIWrapper(parent, name);

        checkbox = UIHelper.CreateCheckbox("Checkbox", wrapperRect, Value, onValueChanged: _ => onInputChanged(this));
        
        var toggleRect = checkbox.GetComponent<RectTransform>();
        UIHelper.StretchToParent(toggleRect);
        toggleRect.anchoredPosition += new Vector2(0, -35);

        return wrapperRect.gameObject;
    }

    /// <inheritdoc/>
    protected override bool GetInputValue() => checkbox.isOn;

    /// <inheritdoc/>
    protected override void SetInputValue(bool inputValue) => checkbox.SetIsOnWithoutNotify(inputValue);

    
}