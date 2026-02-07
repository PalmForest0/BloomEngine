using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BloomEngine.Extensions;

/// <summary>
/// Extension methods that replace <see cref="UnityAction"/> uses with <see cref="Action"/>.
/// </summary>
public static class UnityEventExtensions
{
    // Listener extension methods for UnityEvents
    extension(UnityEvent unityEvent)
    {
        public void AddListener(Action call) => unityEvent.AddListener(call);
        public void RemoveListener(Action call) => unityEvent.RemoveListener(call);
    }

    // Listener extension methods for UnityEvents with parameters
    extension<T>(UnityEvent<T> unityEvent)
    {
        public void AddListener(Action<T> action) => unityEvent.AddListener(action);
        public void RemoveListener(Action<T> action) => unityEvent.RemoveListener(action);
    }

    // Event addition extensions for Unity SceneManager
    extension(SceneManager)
    {
        public static void add_sceneLoaded(Action<Scene, LoadSceneMode> call) => SceneManager.add_sceneLoaded(call);
        public static void add_activeSceneChanged(Action<Scene, Scene> call) => SceneManager.add_activeSceneChanged(call);
        public static void add_sceneUnloaded(Action<Scene> call) => SceneManager.add_sceneUnloaded(call);
    }
}