using MelonLoader;
using UnityEngine;

namespace BloomEngine.Config.Internal;

public abstract class BaseConfigInput
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Type ValueType { get; protected set; }

    public GameObject InputObject { get; protected set; }
    public abstract Type InputObjectType { get; }

    internal abstract void CreateMelonEntry(MelonPreferences_Category melonCategory);

    internal abstract void SetInputObject(GameObject inputObject);

    internal abstract void UpdateFromUI();
    internal abstract void RefreshUI();
    internal abstract void OnUIChanged();
}