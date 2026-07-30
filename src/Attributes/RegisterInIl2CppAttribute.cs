using Il2CppInterop.Runtime.Injection;
using System.Reflection;

namespace BloomEngine.Attributes;

/// <summary>
/// Indicates that the decorated class should be registered with the IL2CPP runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RegisterInIl2CppAttribute : Attribute
{
    /// <summary>
    /// Registers all classes in a given assembly with Il2Cpp if they have the <see cref="RegisterInIl2CppAttribute"/> attribute.
    /// </summary>
    /// <param name="assembly">The assembly to check all types within.</param>
    internal static void RegisterClassesInAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)) && type.GetCustomAttribute<RegisterInIl2CppAttribute>() is not null)
                ClassInjector.RegisterTypeInIl2Cpp(type);
        }
    }
}