using BloomEngine.Interfaces;
using UnityEngine;

namespace BloomEngine.Modules.Config.Inputs;

public abstract class InputFieldBase<T> : IInputField
{
    public string Name { get; set; }

    public Type ValueType => Value.GetType();
    public T Value
    {
        get => field;
        set
        {
            field = TransformValue is not null ? TransformValue.Invoke(value) : value;
            OnValueChanged?.Invoke(field);
        }
    }

    public GameObject InputObject { get; private set; }
    public Type InputObjectType { get; private set; }

    public Action<T> OnValueChanged { get; set; }
    public Action OnInputChanged { get; set; }
    public Func<T, T> TransformValue { get; set; }
    public Func<T, bool> ValidateValue { get; set; }

    public InputFieldBase(string name, T value, Action<T> onValueChanged, Action onInputChanged, Func<T, T> transformValue, Func<T, bool> validateValue)
    {
        Name = name;
        Value = value;
        OnValueChanged = onValueChanged;
        OnInputChanged = onInputChanged;
        TransformValue = transformValue;
        ValidateValue = validateValue;
    }

    public object GetValueObject() => Value;
    public void SetValueObject(object value) => Value = (T)Convert.ChangeType(value, typeof(T));

    public virtual void SetInputObject(GameObject inputObject)
    {
        InputObject = inputObject;
        InputObjectType = inputObject.GetType();
    }

    public abstract void UpdateFromUI();
    public abstract void RefreshUI();
    public virtual void OnUIChanged() => OnInputChanged?.Invoke();
}