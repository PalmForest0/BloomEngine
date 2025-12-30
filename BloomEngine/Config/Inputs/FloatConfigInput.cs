using BloomEngine.ModMenu.Services;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// Creates a <see cref="float"/> input field in the form of a slider in your mod's config menu and returns it.
/// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
/// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
/// </summary>
/// <param name="name">The name of this input slider, which will be displayed in the config menu.</param>
/// <param name="description">The description of this input slider, which will be displayed in the config menu.</param>
/// <param name="defaultValue">The default value of this config input slider.</param>
/// <param name="minValue">The minimum value of this input slider.</param>
/// <param name="maxValue">The maximum value of this input slider.</param>
/// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
/// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
/// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
/// <param name="validateValue">A function to validate the new value before it is updated.</param>
public sealed class FloatConfigInput(string name, string description, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null, Action onInputChanged = null, Func<float, float> transformValue = null, Func<float, bool> validateValue = null)
    : BaseConfigInputT<float>(name, description, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;

    public override Type InputObjectType { get; } = typeof(Slider);
    public Slider Slider { get; private set; }

    internal override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Slider = inputObject.GetComponent<Slider>();
    }

    internal override void UpdateFromUI() => Value = Slider.value;
    internal override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}