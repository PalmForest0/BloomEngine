namespace BloomEngine.Menu.Config;

public class ConfigProperty<T> : IConfigProperty
{
    public string Name { get; set; }
    public string Description { get; protected set; }
    //public PropertyInputType InputType { get; private set; } = PropertyInputType.Auto;
    public Type ValueType => typeof(T);

    public Func<T, bool> ValidateFunc { get; private set; }
    public Func<T, T> TransformFunc { get; private set; }
    public Action<T> OnValueUpdated { get; private set; }

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
    public void SetValue(object val)
    {
        if (val is T correctVal)
            Value = (T)val;
    }
    public object TransformInput(object value) => TransformFunc is not null ? TransformFunc((T)value) : value;

    internal ConfigProperty(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    {
        Name = name;
        DefaultValue = defaultValue;
        _value = defaultValue;

        OnValueUpdated = onValueUpdated;
        ValidateFunc = validateFunc;
        TransformFunc = transformFunc;
    }

    //private static PropertyInputType InferInputType(Type type) => type switch
    //{
    //    var t when t == typeof(int) => PropertyInputType.NumberBox,
    //    var t when t == typeof(float) => PropertyInputType.NumberBox,
    //    var t when t == typeof(double) => PropertyInputType.NumberBox,
    //    var t when t == typeof(long) => PropertyInputType.NumberBox,
    //    var t when t == typeof(short) => PropertyInputType.NumberBox,
    //    var t when t == typeof(bool) => PropertyInputType.Checkbox,
    //    var t when t == typeof(Action) => PropertyInputType.Button,
    //    _ => PropertyInputType.TextBox
    //};
}