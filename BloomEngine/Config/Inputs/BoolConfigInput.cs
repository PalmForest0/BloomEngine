using BloomEngine.Config.Services;
using BloomEngine.Utilities;
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
        Toggle = UIHelper.CreateCheckbox(InputObjectName, parent, Value, onValueChanged: _ => OnUIChanged());
        return Toggle.gameObject;
    }

    internal override void UpdateFromUI() => Value = Toggle.isOn;
    internal override void RefreshUI() => Toggle.SetIsOnWithoutNotify(Value);   
}