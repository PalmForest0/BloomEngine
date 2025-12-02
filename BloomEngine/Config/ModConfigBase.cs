using BloomEngine.Utilities;
using MelonLoader;
using System.Collections.ObjectModel;

namespace BloomEngine.Config;

public class ModConfigBase
{
    private readonly List<IConfigProperty> properties = new List<IConfigProperty>();
    public ReadOnlyCollection<IConfigProperty> Properties => properties.AsReadOnly();

    public ConfigProperty<T> AddProperty<T>(ConfigPropertyData<T> data)
    {
        if (!IConfigProperty.IsValidPropertyType<T>())
        {
            Melon<BloomEngineMod>.Logger.Error($"Attempted to add a config property with an unsupported type. Supported types currently include numeric types and strings.");
            return null;
        }

        var property = new ConfigProperty<T>(data);
        properties.Add(property);
        return property;
    }

    public IConfigProperty GetProperty(string name) => properties.Find(prop => prop.Name == name);

    
}