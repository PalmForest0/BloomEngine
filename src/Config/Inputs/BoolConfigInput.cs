using BloomEngine.Config.Inputs.Base;
using BloomEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="bool"/> input.<br/>
/// To create a <see cref="BoolConfigInput"/>, use <see cref="ConfigService.CreateBool(string, string, bool)"/>
/// </summary>
public sealed class BoolConfigInput : TypedConfigInput<bool, BoolConfigInput>
{
    /// <summary>
    /// The UI checkbox which corresponds to this config input in the config panel.
    /// </summary>
    public Toggle Toggle { get; private set; } = null!;

    internal BoolConfigInput(string name, string description, bool defaultValue) : base(name, description, defaultValue) { }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        RectTransform wrapper = UIHelper.CreateUIWrapper(parent, name);

        Toggle = UIHelper.CreateCheckbox("Toggle_Internal", wrapper, Value, onValueChanged: _ => RaiseInputChanged());
        RectTransform toggleRect = Toggle.gameObject.GetComponent<RectTransform>();
        UIHelper.SetParentAndStretch(toggleRect, wrapper);

        toggleRect.anchoredPosition += new Vector2(0, -35);

        return wrapper.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = Toggle.isOn;

    /// <inheritdoc/>
    protected override void SetDisplayedValue(bool value) => Toggle.SetIsOnWithoutNotify(value);
}