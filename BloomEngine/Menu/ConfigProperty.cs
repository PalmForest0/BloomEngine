namespace BloomEngine.Menu;

public class ConfigProperty
{
    public string Name { get; }
    public string PlaceHolder { get; }
    public string Description { get; }
    public Type PropertyType { get; }
    public PropertyInputType InputType { get; } = PropertyInputType.Auto;

    public Func<object> Getter { get; }
    public Action<object> Setter { get; }
    public Action<object> OnValueChanged { get; }

    public ConfigProperty(
        string name,
        Type type,
        Func<object> getter,
        Action<object> setter,
        Action<object> onValueChanged,
        string placeholder,
        string description = default,
        PropertyInputType inputType = PropertyInputType.Auto)
    {
        Name = name;
        PropertyType = type;
        Getter = getter;
        Setter = setter;
        OnValueChanged = onValueChanged;
        PlaceHolder = placeholder;
        Description = description;
        InputType = inputType;
    }
}