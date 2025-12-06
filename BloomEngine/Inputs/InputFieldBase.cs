namespace BloomEngine.Inputs;

public abstract class InputFieldBase<T> : IInputField<T>
{
    protected T Value
    {
        get => field;
        set
        {
            field = TransformValue is not null ? TransformValue.Invoke(value) : value;
            OnValueChanged?.Invoke(field);
        }
    }

    public Action<T> OnValueChanged { get; set; }
    public Func<T, T> TransformValue { get; set; }
    public Func<T, bool> ValidateValue { get; set; }

    //public T GetValue() => _value;
    //public void SetValue(T value) => _value

    //public object GetValueObject() => GetValue();
    //public void SetValueObject(object value) => SetValue((T)Convert.ChangeType(value, typeof(T)));

    public abstract void UpdateValue();
    public abstract void RefreshUI();
    public virtual void OnUIChanged() { }
}