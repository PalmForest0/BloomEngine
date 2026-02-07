using UnityEngine;

namespace BloomEngine.Utilities;

/// <summary>
/// Static utility class for scene-related helper methods.
/// </summary>
public static class SceneUtils
{
    public static T FindComponent<T>(this Transform obj, string path) where T : MonoBehaviour => obj?.Find(path)?.GetComponentInChildren<T>(true);
    public static T FindComponent<T>(this GameObject obj, string path) where T : MonoBehaviour => obj?.transform?.Find(path)?.GetComponentInChildren<T>(true);
}