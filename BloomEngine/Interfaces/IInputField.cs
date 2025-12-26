namespace BloomEngine.Config.Inputs;

public interface IInputField
{
    string Name { get; set; }

    Type ValueType { get; }

    object InputObject { get; set; }
    Type InputObjectType { get; }

    object GetValueObject();
    void SetValueObject(object value);

    void UpdateFromUI();
    void RefreshUI();
    void OnUIChanged();
}