using MelonLoader;
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
            MelonEntry?.Value = field;
        }
    }

    public MelonPreferences_Entry<T> MelonEntry { get; private set; }

    public Action<T> OnValueChanged { get; set; }
    public Action OnInputChanged { get; set; }
    public Func<T, T> TransformValue { get; set; }
    public Func<T, bool> ValidateValue { get; set; }

    protected BaseConfigInputT(string name, string description, T value, Action<T> onValueChanged, Action onInputChanged, Func<T, T> transformValue, Func<T, bool> validateValue)
    {
        Name = name;
        Description = description;

        Value = value;
        ValueType = value.GetType();

        OnValueChanged = onValueChanged;
        OnInputChanged = onInputChanged;
        TransformValue = transformValue;
        ValidateValue = validateValue;
    }

    internal override void CreateMelonEntry(MelonPreferences_Category melonCategory)
        => MelonEntry = melonCategory.CreateEntry(Name, Value, Name, Description);

    internal override void SetInputObject(GameObject inputObject) => InputObject = inputObject;

    internal override void OnUIChanged() => OnInputChanged?.Invoke();
}