using System.Diagnostics.CodeAnalysis;

namespace BloomEngine.Extensions;

public static class NullExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="UnityEngine.Object"/> is considered null using Unity's custom null handling.
    /// </summary>
    /// <param name="obj">The <see cref="UnityEngine.Object"/> to check for null.</param>
    /// <returns><see langword="true"/> if the object is null or has been destroyed, otherwise <see langword="false"/>.</returns>
    public static bool IsNull([NotNullWhen(false)] this UnityEngine.Object? obj) => !obj;

    /// <summary>
    /// Determines whether the specified <see cref="UnityEngine.Object"/> is not considered null using Unity's custom null handling.
    /// </summary>
    /// <param name="obj">The <see cref="UnityEngine.Object"/> to check for null.</param>
    /// <returns><see langword="false"/> if the object is null or has been destroyed, otherwise <see langword="true"/>.</returns>
    public static bool NotNull<T>([NotNullWhen(true)] this T? obj) where T : UnityEngine.Object => obj;
    
    /// <summary>
    /// Returns null if the Unity object is null or destroyed, allowing the <c>?.</c> and <c>??</c> operators to work correctly.
    /// </summary>
    /// <returns>The Unity object, or null if it is either null or destroyed.</returns>
    public static T? OrNull<T>(this T? obj) where T : UnityEngine.Object
        => obj ? obj : null;
}