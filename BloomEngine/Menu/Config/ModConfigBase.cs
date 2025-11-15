using System.Collections.ObjectModel;

namespace BloomEngine.Menu.Config;

public class ModConfigBase
{
    private List<IConfigProperty> properties = new List<IConfigProperty>();
    public ReadOnlyCollection<IConfigProperty> Properties => properties.AsReadOnly();

    protected ConfigProperty<T> Register<T>(string name, T defaultValue, Action<T> onValueUpdated = null, Func<T, bool> validateFunc = null, Func<T, T> transformFunc = null)
    {
        var property = new ConfigProperty<T>(name, defaultValue, onValueUpdated, validateFunc, transformFunc);
        properties.Add(property);
        return property;
    }
}