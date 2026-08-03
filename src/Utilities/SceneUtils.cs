using BloomEngine.Core;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BloomEngine.Utilities;

/// <summary>
/// Static utility class for scene-related helper methods.
/// </summary>
public static class SceneUtils
{
    /// <summary>
    /// Searches for a child Transform at the specified path and returns the first component of type T found in its
    /// children, including inactive components.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour component to search for.</typeparam>
    /// <param name="obj">The Transform to search within. Cannot be null.</param>
    /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
    /// <returns>The first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</returns>
    public static T? FindComponent<T>(this Transform obj, string path) where T : MonoBehaviour
    {
        return obj ? obj.Find(path)?.GetComponentInChildren<T>(true) : null;
    }

    /// <summary>
    /// Searches for a child Transform at the specified path and returns the first component of type T found in its
    /// children, including inactive components.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour component to search for.</typeparam>
    /// <param name="obj">The GameObject to search within. Cannot be null.</param>
    /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
    /// <returns>The first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</returns>
    public static T? FindComponent<T>(this GameObject obj, string path) where T : MonoBehaviour
    {
        return obj? obj.transform.FindComponent<T>(path) : null;
    }

    /// <summary>
    /// Searches for a child Transform at the specified path and returns whether a component of type T was found in its
    /// children, including inactive components.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour component to search for.</typeparam>
    /// <param name="obj">The Transform to search within. Cannot be null.</param>
    /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
    /// <param name="component">Contains the first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</param>
    /// <param name="logPrefix">An optional prefix for the log message if the component can be found. No log message is sent when this is null.</param>
    /// <returns><see langword="true"/> if a matching component was found; otherwise, <see langword="false"/>.</returns>
    public static bool TryFindComponent<T>(this Transform obj, string path, [NotNullWhen(true)] out T? component, string? logPrefix = null) where T : MonoBehaviour
    {
        component = obj.FindComponent<T>(path);

        if(!component && logPrefix is not null)
            BloomLogger.Error($"Component of type {typeof(T).Name} not found at path '{path}' on GameObject '{obj.name}'", logPrefix);

        return component;
    }

    /// <summary>
    /// Searches for a child Transform at the specified path and returns whether a component of type T was found in its
    /// children, including inactive components.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour component to search for.</typeparam>
    /// <param name="obj">The GameObject to search within. Cannot be null.</param>
    /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
    /// <param name="component">Contains the first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</param>
    /// <param name="logPrefix">An optional prefix for the log message if the component can be found. No log message is sent when this is null.</param>
    /// <returns><see langword="true"/> if a matching component was found; otherwise, <see langword="false"/>.</returns>
    public static bool TryFindComponent<T>(this GameObject obj, string path, [NotNullWhen(true)] out T? component, string? logPrefix = null) where T : MonoBehaviour
    {
        component = obj.FindComponent<T>(path);

        if (!component && logPrefix is not null)
            BloomLogger.Error($"Component of type {typeof(T).Name} not found at path '{path}' on GameObject '{obj.name}'", logPrefix);

        return component;
    }
}