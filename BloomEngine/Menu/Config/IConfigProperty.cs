namespace BloomEngine.Menu.Config;

public interface IConfigProperty
{
    abstract string Name { get; set; }
    abstract Type ValueType { get; }


    abstract object GetValue();
    abstract void SetValue(object value);
    //abstract string TransformInput(string value);
}