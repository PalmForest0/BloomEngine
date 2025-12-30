using UnityEngine;

namespace BloomEngine.Modules.Config.Inputs;

public interface IConfigInput
{
    string Name { get; set; }

    Type ValueType { get; }

    GameObject InputObject { get; }
    Type InputObjectType { get; }

    void SetInputObject(GameObject inputObject);

    void UpdateFromUI();
    void RefreshUI();
    void OnUIChanged();
}