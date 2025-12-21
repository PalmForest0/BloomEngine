namespace BloomEngine.Utilities;

/// <summary>
/// Provides a simple way to store and retrieve instances of game components globally.
/// </summary>
internal static class InstanceHelper
{
    private static readonly Dictionary<Type, object> instances = new();

    public static void SetInstance<T>(T instance)
    {
        instances[typeof(T)] = instance!;
    }

    public static T GetInstance<T>()
    {
        return (T)instances.GetValueOrDefault(typeof(T));
    }
}
