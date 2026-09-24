using System.Diagnostics.CodeAnalysis;
using BloomEngine.Core;
using BloomEngine.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Extensions;

/// <summary>
/// General extension methods that simplify working with Unity. 
/// </summary>
public static class UnityExtensions
{
    /// <summary>
    /// Extension methods that replace <see cref="UnityAction"/> uses with <see cref="Action"/>.
    /// </summary>
    extension(UnityEvent unityEvent)
    {
        /// <summary>
        /// Adds a listener action to a <see cref="UnityEvent"/>, taking in a normal <see cref="Action"/> instead of a <see cref="UnityAction"/>.
        /// </summary>
        /// <param name="call">This listener action to add to this <see cref="UnityEvent"/></param>
        public void AddListener(Action call) => unityEvent.AddListener(call);
        public void RemoveListener(Action call) => unityEvent.RemoveListener(call);
    }
    
    /// <summary>
    /// Extension methods that replace <see cref="UnityAction"/> uses with <see cref="Action"/>.
    /// </summary>
    extension<T>(UnityEvent<T> unityEvent)
    {
        /// <summary>
        /// Adds a listener action to a <see cref="UnityEvent"/>, taking in a normal <see cref="Action"/> instead of a <see cref="UnityAction"/>.
        /// </summary>
        /// <param name="call">This listener action to add to this <see cref="UnityEvent"/></param>
        public void AddListener(Action<T> call) => unityEvent.AddListener(call);
        public void RemoveListener(Action<T> call) => unityEvent.RemoveListener(call);
    }

    extension(Transform transform)
    {
        /// <summary>
        /// Searches for a child Transform at the specified path and returns the first component of type T found in its
        /// children, including inactive components.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
        /// <returns>The first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</returns>
        public T? FindComponent<T>(string path) where T : Component => transform ? transform.Find(path)?.GetComponentInChildren<T>(true) : null;
        
        /// <summary>
        /// Searches for a child Transform at the specified path and returns whether a component of type T was found in its
        /// children, including inactive components.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
        /// <param name="component">Contains the first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</param>
        /// <param name="logPrefix">An optional prefix for the log message if the component can be found. No log message is sent when this is null.</param>
        /// <returns><see langword="true"/> if a matching component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryFindComponent<T>(string path, [NotNullWhen(true)] out T? component, string? logPrefix = null) where T : Component
        {
            component = transform.FindComponent<T>(path);

            if(component.IsNull() && logPrefix is not null)
                BloomLogger.Error($"Component of type {typeof(T).Name} not found at path '{path}' on GameObject '{transform.name}'", logPrefix);
            
            return component;
        }
    }

    extension(GameObject gameObject)
    {
        /// <summary>
        /// Searches for a child Transform at the specified path and returns the first component of type T found in its
        /// children, including inactive components.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
        /// <returns>The first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</returns>
        public T? FindComponent<T>(string path) where T : Component => gameObject? gameObject.transform.FindComponent<T>(path) : null;
        
        /// <summary>
        /// Searches for a child Transform at the specified path and returns whether a component of type T was found in its
        /// children, including inactive components.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="path">The relative path to the child Transform to search for. Cannot be null or empty.</param>
        /// <param name="component">Contains the first component of type T found in the children of the Transform at the given path, or null if no matching component is found.</param>
        /// <param name="logPrefix">An optional prefix for the log message if the component can be found. No log message is sent when this is null.</param>
        /// <returns><see langword="true"/> if a matching component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryFindComponent<T>(string path, [NotNullWhen(true)] out T? component, string? logPrefix = null) where T : Component
        {
            component = gameObject.FindComponent<T>(path);

            if (component.IsNull() && logPrefix is not null)
                BloomLogger.Error($"Component of type {typeof(T).Name} not found at path '{path}' on GameObject '{gameObject.name}'", logPrefix);

            return component;
        }
    }
}