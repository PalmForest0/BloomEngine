using UnityEngine;

namespace BloomEngine.Interfaces;

public interface IInputField
{
    string Name { get; set; }

    Type ValueType { get; }

    GameObject InputObject { get; }
    Type InputObjectType { get; }

    void SetInputObject(GameObject inputObject);

    object GetValueObject();
    void SetValueObject(object value);

    void UpdateFromUI();
    void RefreshUI();
    void OnUIChanged();
}