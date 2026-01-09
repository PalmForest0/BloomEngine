using UnityEngine.Events;

namespace BloomEngine.Extensions;

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
}