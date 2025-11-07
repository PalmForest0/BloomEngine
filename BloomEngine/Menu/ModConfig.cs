using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomEngine.Menu;

public class ModConfig
{
    public List<ConfigProperty> Properties { get; } = new List<ConfigProperty>();

    public ModConfig() { }
    public ModConfig(params ConfigProperty[] properties)
    {
        Properties.AddRange(properties);
    }

    public ModConfig AddProperty(ConfigProperty property)
    {
        Properties.Add(property);
        return this;
    }

    public ModConfig AddProperty(string name, Func<string> getter, Action<string> setter, string info = default)
    {
        Properties.Add(new ConfigProperty(name, getter, setter, info));
        return this;
    }

    public ModConfig AddProperties(params ConfigProperty[] properties)
    {
        Properties.AddRange(properties);
        return this;
    }
}
