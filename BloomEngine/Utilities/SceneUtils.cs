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
    public static T FindComponent<T>(this Transform obj, string path) where T : MonoBehaviour => obj?.Find(path)?.GetComponentInChildren<T>(true);

    /// <summary>
    /// Searches for a child Transform at the specified path and returns the first component of type T found in its
    /// children, including inactive components.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour component to search for.</typeparam>
    /// <param name="obj">The GameObject to search within. Cannot be null.</param>
    /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
    /// <returns>The first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</returns>
    public static T FindComponent<T>(this GameObject obj, string path) where T : MonoBehaviour => obj?.transform?.Find(path)?.GetComponentInChildren<T>(true);
}