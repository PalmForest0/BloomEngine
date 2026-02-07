namespace BloomEngine.Attributes;

/// <summary>
/// Indicates that the decorated class should be registered with the IL2CPP runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RegisterInIl2CppAttribute : Attribute { }