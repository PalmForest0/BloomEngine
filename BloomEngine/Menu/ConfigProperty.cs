using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomEngine.Menu;

public class ConfigProperty
{
    public string Name { get; }
    public string Info { get; }
    public Func<string> Getter { get; }
    public Action<string> Setter { get; }
    public ConfigProperty(string name, Func<string> getter, Action<string> setter, string info = default)
    {
        Name = name;
        Info = info;
        Getter = getter;
        Setter = setter;
    }
}
