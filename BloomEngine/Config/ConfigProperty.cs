using BloomEngine.Menu;

namespace BloomEngine.Config;

public class ConfigProperty<T> : IConfigProperty
{
    public string Name { get; set; }
    public string Description { get; protected set; }
    public Type ValueType => typeof(T);

    public Action<T> OnValueUpdated { get; private set; }
    public Func<T, bool> ValidateFunc { get; private set; }
    public Func<T, T> TransformFunc { get; private set; }

    public T DefaultValue { get; private set; }

    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (ValidateFunc is not null && !ValidateFunc(value))
                ModMenu.Log($"New value of config property {Name} was invalid.");
            _value = value;
            OnValueUpdated?.Invoke(value);
        }
    }

    public object GetValue() => Value;

    public void SetValue(object value)
    {
        if (value is T correctVal)
            Value = (T)value;
    }

    public bool ValidateValue(object value)
    {
        if (ValidateFunc is null || ValidateFunc.Invoke((T)value))
            return true;

        return false;
    }

    public object TransformValue(object value)
    {
        if (TransformFunc is not null)
            return TransformFunc.Invoke((T)value);
        return value;
    }

    internal ConfigProperty(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    {
        Name = name;
        DefaultValue = defaultValue;
        _value = defaultValue;

        OnValueUpdated = onValueUpdated;
        ValidateFunc = validateFunc;
        TransformFunc = transformFunc;
    }
}