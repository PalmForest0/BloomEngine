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

    public Func<object, object> OnInputChanged { get; set; }

    public ConfigProperty(
        string name,
        Type type,
        Func<object> getter,
        Action<object> setter,
        string placeholder,
        string description = default,
        Func<object, object> onInputChanged = default,
        PropertyInputType inputType = PropertyInputType.Auto)
    {
        Name = name;
        PropertyType = type;
        Getter = getter;
        Setter = setter;
        PlaceHolder = placeholder;
        Description = description;
        OnInputChanged = onInputChanged;
        InputType = inputType;
    }
}