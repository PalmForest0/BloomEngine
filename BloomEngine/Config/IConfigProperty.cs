namespace BloomEngine.Config;

public interface IConfigProperty
{
    abstract string Name { get; set; }
    abstract Type ValueType { get; }


    abstract object GetValue();
    abstract void SetValue(object value);
    abstract bool ValidateValue(object value);
    abstract object TransformValue(object value);
}