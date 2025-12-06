using UnityEngine.Events;

namespace BloomEngine.Utilities;

public static class Il2CppHelper
{
    // Listener extension methods for UnityEvents
    extension(UnityEvent unityEvent)
    {
        public void AddListener(Action action) => unityEvent.AddListener(action);
        public void RemoveListener(Action action) => unityEvent.RemoveListener(action);
        public void SetListeners(List<Action> actions)
        {
            unityEvent.RemoveAllListeners();

            foreach (Action action in actions)
                unityEvent.AddListener(action);
        }
    }

    // Listener extension methods for UnityEvents with parameters
    extension<T>(UnityEvent<T> unityEvent)
    {
        public void AddListener(Action<T> action) => unityEvent.AddListener(action);
        public void RemoveListener(Action<T> action) => unityEvent.RemoveListener(action);
        public void SetListeners(List<Action<T>> actions)
        {
            unityEvent.RemoveAllListeners();

            foreach (Action<T> action in actions)
                unityEvent.AddListener(action);
        }
    }

    extension<T>(IEnumerable<T> collection)
    {
        public Il2CppSystem.Collections.Generic.List<T> ToIl2CppList()
        {
            var result = new Il2CppSystem.Collections.Generic.List<T>();

            foreach (var item in collection)
                result.Add(item);

            return result;
        }
    }
}