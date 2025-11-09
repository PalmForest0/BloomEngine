namespace BloomEngine.Menu;

public enum PropertyInputType
{
    Auto,       // Infer from property type
    TextBox,
    NumberBox,
    Checkbox,
    Button
}

[AttributeUsage(AttributeTargets.Property)]
public class ConfigPropertyAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public string Placeholder { get; set; }
    public PropertyInputType InputType { get; set; }
    public Action<object> OnValueChanged { get; set; }

    public ConfigPropertyAttribute(string name, string description = null)
    {
        Name = name;
        Description = description;
    }
}