namespace System.Runtime.CompilerServices;

/// <summary>
/// Specifies the nullability context for reference types within a scope.
/// </summary>
/// <param name="flag">The nullability context flag value.</param>
[AttributeUsage(AttributeTargets.All)]
internal sealed class NullableContextAttribute(byte flag) : Attribute
{
    /// <summary>
    /// Gets the nullability context flag value.
    /// </summary>
    public readonly byte Flag = flag;
}