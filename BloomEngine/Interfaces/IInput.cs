using UnityEngine;

namespace BloomEngine.Interfaces;

public interface IInput
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