using System.Diagnostics.CodeAnalysis;

namespace BloomEngine.Helpers;

/// <summary>
/// Provides helper methods for checking when a <see cref="UnityEngine.Object"/> reference is null using Unity's custom semantics.
/// </summary>
public static class UnityHelper
{
    /// <summary>
    /// Determines whether the specified <see cref="UnityEngine.Object"/> is considered null using Unity's custom null handling.
    /// </summary>
    /// <param name="obj">The <see cref="UnityEngine.Object"/> to check for null.</param>
    /// <returns><see langword="true"/> if the object is null or considered null by Unity, or <see langword="false"/> otherwise.</returns>
    public static bool IsNull([NotNullWhen(false)] this UnityEngine.Object? obj) => obj is not null ? obj : true;

    /// <summary>
    /// Determines whether the specified <see cref="UnityEngine.Object"/> is not considered null using Unity's custom null handling.
    /// </summary>
    /// <param name="obj">The <see cref="UnityEngine.Object"/> to check for null.</param>
    /// <returns><see langword="false"/> if the object is null or considered null by Unity, or <see langword="true"/> otherwise.</returns>
    public static bool NotNull([NotNullWhen(true)] this UnityEngine.Object? obj) => !obj.IsNull();
}
