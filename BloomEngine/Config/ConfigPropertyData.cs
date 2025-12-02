namespace BloomEngine.Config;

internal class ConfigPropertyData<T>(string name, T defaultValue, Action<T> onValueUpdated, Func<T, bool> validateFunc, Func<T, T> transformFunc)
{
    public string Name { get; set; } = name;
    public T DefaultValue { get; private set; } = defaultValue;
    public Action<T> OnValueUpdated { get; private set; } = onValueUpdated;
    public Func<T, bool> ValidateFunc { get; private set; } = validateFunc;
    public Func<T, T> TransformFunc { get; private set; } = transformFunc;
}