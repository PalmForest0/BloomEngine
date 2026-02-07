namespace BloomEngine.Extensions;

/// <summary>
/// Static class containing <see cref="IEnumerable{T}"/> and <see cref="Il2CppSystem.Collections.Generic.List{T}"/> extensions.
/// </summary>
public static class CollectionExtensions
{
    extension<T>(IEnumerable<T> collection)
    {
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <returns>True if the collection is null or empty; otherwise, false.</returns>
        public bool IsNullOrEmpty() => collection is null || !collection.Any();

        /// <summary>
        /// Creates a new <see cref="Il2CppSystem.Collections.Generic.List{T}"/> and adds all elements from this collection to it.
        /// </summary>
        /// <returns>An Il2Cpp list with all elements of the current collection.</returns>
        public Il2CppSystem.Collections.Generic.List<T> ToIl2CppList()
        {
            var result = new Il2CppSystem.Collections.Generic.List<T>();
 
            foreach (var item in collection)
                result.Add(item);

            return result;
        }
    }

    extension<T>(Il2CppSystem.Collections.Generic.List<T> il2cppCollection)
    {
        /// <summary>
        /// Determines whether the Il2Cpp list is null or contains no elements.
        /// </summary>
        /// <returns>True if the Il2Cpp list is null or empty; otherwise, false.</returns>
        public bool IsNullOrEmpty() => il2cppCollection is null || il2cppCollection.Count == 0;

        /// <summary>
        /// Creates a new <see cref="List{T}"/> and adds all elements of this Il2Cpp list to it.
        /// </summary>
        /// <returns>A list with all elements of the current Il2Cpp list.</returns>
        public List<T> ToManagedList()
        {
            var result = new List<T>();

            foreach (var item in il2cppCollection)
                result.Add(item);

            return result;
        }
    }
}