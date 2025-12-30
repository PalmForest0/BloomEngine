using UnityEngine;

namespace BloomEngine.Config.Internal;

public abstract class BaseConfigInputT<T> : BaseConfigInput
{
    public T Value
    {
        get => field;
        set
        {
            field = TransformValue is not null ? TransformValue.Invoke(value) : value;
            OnValueChanged?.Invoke(field);
        }
    }

    public Action<T> OnValueChanged { get; set; }
    public Action OnInputChanged { get; set; }
    public Func<T, T> TransformValue { get; set; }
    public Func<T, bool> ValidateValue { get; set; }

    protected BaseConfigInputT(string name, T value, Action<T> onValueChanged, Action onInputChanged, Func<T, T> transformValue, Func<T, bool> validateValue)
    {
        Name = name;
        Value = value;
        OnValueChanged = onValueChanged;
        OnInputChanged = onInputChanged;
        TransformValue = transformValue;
        ValidateValue = validateValue;
    }

    internal override void SetInputObject(GameObject inputObject)
    {
        InputObject = inputObject;
        InputObjectType = inputObject.GetType();
    }

    internal override void OnUIChanged() => OnInputChanged?.Invoke();
}