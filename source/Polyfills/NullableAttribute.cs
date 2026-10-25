namespace System.Runtime.CompilerServices;

/// <summary>
/// Specifies nullability annotations for reference types.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
internal sealed class NullableAttribute : Attribute
{
    /// <summary>
    /// Gets the array of nullability flags for the annotated element.
    /// </summary>
    public readonly byte[] NullableFlags;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableAttribute"/> class with a single nullability flag.
    /// </summary>
    /// <param name="flag">The nullability flag value.</param>
    public NullableAttribute(byte flag) { NullableFlags = [flag]; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableAttribute"/> class with multiple nullability flags.
    /// </summary>
    /// <param name="flags">An array of nullability flag values.</param>
    public NullableAttribute(byte[] flags) { NullableFlags = flags; }
}