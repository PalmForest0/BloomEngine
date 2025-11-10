using System.Reflection;

namespace BloomEngine.Menu;

public struct InputChangeContext
{
    public PropertyInfo Property { get; private set; }
    public object Value { get; set; }
}
