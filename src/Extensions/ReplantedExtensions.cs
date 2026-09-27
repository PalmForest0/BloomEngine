using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BloomEngine.Extensions;

public static class ReplantedExtensions
{
    extension(GameObject gameObject)
    {
        /// <summary>
        /// Destroys all <see cref="TextLocalizer"/> and <see cref="Binder"/> components on this GameObject and its children.
        /// </summary>
        public void DestroyBindersAndLocalizers()
        {
            foreach (var localizer in gameObject.GetComponentsInChildren<TextLocalizer>(true))
                Object.DestroyImmediate(localizer);
            foreach (var binder in gameObject.GetComponentsInChildren<Binder>(true))
                Object.DestroyImmediate(binder);
        }
    }
}