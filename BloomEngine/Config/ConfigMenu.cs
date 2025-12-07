using BloomEngine.Config.Inputs;

namespace BloomEngine.Config;

public static class ConfigMenu
{
    public static StringInputField CreateStringInput(string name, string defaultValue, Action<string> onValueChanged = null, Action onInputChanged = null, Func<string, string> transformValue = null, Func<string, bool> validateValue = null)
        => new StringInputField(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);

    public static IntInputField CreateIntInput(string name, int defaultValue, Action<int> onValueChanged = null, Action onInputChanged = null, Func<int, int> transformValue = null, Func<int, bool> validateValue = null)
        => new IntInputField(name, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue);

    public static FloatInputField CreateFloatInput(string name, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null, Action onInputChanged = null, Func<float, float> transformValue = null, Func<float, bool> validateValue = null)
        => new FloatInputField(name, defaultValue, minValue, maxValue, onValueChanged, onInputChanged, transformValue, validateValue);

    public static BoolInputField CreateBoolInput(string name, bool defaultValue, Action<bool> onValueChanged = null, Action onInputChanged = null)
        => new BoolInputField(name, defaultValue, onValueChanged, onInputChanged, null, null);

    public static EnumInputField CreateEnumInput(string name, Enum defaultValue, Action<Enum> onValueChanged = null, Action onInputChanged = null, Func<Enum, bool> validateValue = null)
        => new EnumInputField(name, defaultValue, onValueChanged, onInputChanged, null, validateValue);
}