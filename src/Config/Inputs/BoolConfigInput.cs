using BloomEngine.Config.Inputs.Base;
using BloomEngine.Config.Services;
using BloomEngine.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="bool"/> input.<br/>
/// To create a <see cref="BoolConfigInput"/>, use <see cref="ConfigService.CreateBool(string, string, bool, ConfigInputOptions{bool})"/>
/// </summary>
public sealed class BoolConfigInput : TypedConfigInput<bool>
{
    /// <summary>
    /// The UI checkbox which corresponds to this config input in the config panel.
    /// </summary>
    public Toggle Toggle { get; private set; }

    internal BoolConfigInput(string name, string description, bool defaultValue, ConfigInputOptions<bool> options) : base(name, description, defaultValue, options) { }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        RectTransform wrapper = UIHelper.CreateUIWrapper(parent, InputObjectName);

        Toggle = UIHelper.CreateCheckbox("Toggle_Internal", wrapper, Value, onValueChanged: _ => OnUIChanged());
        RectTransform toggleRect = Toggle.gameObject.GetComponent<RectTransform>();
        UIHelper.SetParentAndStretch(toggleRect, wrapper);

        toggleRect.anchoredPosition += new Vector2(0, -35);

        return wrapper.gameObject;
    }

    internal override void UpdateFromUI() => Value = Toggle.isOn;

    /// <inheritdoc/>
    protected override void SetDisplayedValue(bool value) => Toggle.SetIsOnWithoutNotify(value);
}