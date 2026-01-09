namespace BloomEngine.Extensions;

public static class CollectionExtensions
{
    extension<T>(IEnumerable<T> collection)
    {
        public bool IsNullOrEmpty() => collection is null || !collection.Any();

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
        public bool IsNullOrEmpty() => il2cppCollection is null || il2cppCollection.Count == 0;

        public List<T> ToManagedList()
        {
            var result = new List<T>();

            foreach (var item in il2cppCollection)
                result.Add(item);

            return result;
        }
    }
}