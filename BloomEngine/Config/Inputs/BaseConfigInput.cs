using UnityEngine;

namespace BloomEngine.Config.Internal;

public abstract class BaseConfigInput
{
    public string Name { get; set; }

    public Type ValueType { get; }

    public GameObject InputObject { get; protected set; }
    public Type InputObjectType { get; protected set; }

    internal abstract void SetInputObject(GameObject inputObject);

    internal abstract void UpdateFromUI();
    internal abstract void RefreshUI();
    internal abstract void OnUIChanged();
}