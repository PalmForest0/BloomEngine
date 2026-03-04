using MelonLoader;

namespace BloomEngine.Config.Inputs.Base;

/// <summary>
/// Represents a generic config input with a specificly typed <see cref="Value"/>.
/// </summary>
/// <typeparam name="T">The type of value stored within this config input.</typeparam>
public abstract class TypedConfigInput<T> : BaseConfigInput
{
    /// <summary>
    /// Gets or sets the value stored in this config input, invoking <see cref="TransformValue"/>.
    /// If the new value is different to the old value,<br/> <see cref="OnValueChanged"/> is invoked and the <see cref="MelonEntry"/> value is updated.
    /// </summary>
    public T Value
    {
        get => field;
        set
        {
            T newValue = TransformFunc is not null ? TransformFunc.Invoke(value) : value;

            // Do not assign new value if validation fails
            if (ValidateFunc is not null && !ValidateFunc.Invoke(newValue))
                return;

            // Don't call event or update MelonEntry value if there is no difference
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return;
            
            field = newValue;
            OnValueChanged?.Invoke(field);
            MelonEntry?.Value = field;
        }
    }

    /// <summary>
    /// The default value of this config input. This is also used as a fallback when an unexpected value is encountered.
    /// </summary>
    public T DefaultValue { get; private init; }

    /// <summary>
    /// The <see cref="MelonPreferences_Entry"/> that corresponds to this config input and contains the saved value.
    /// </summary>
    public MelonPreferences_Entry<T> MelonEntry { get; private set; }

    /// <summary>
    /// An event that is invoked when <see cref="Value"/> is modified.
    /// </summary>
    public event Action<T> OnValueChanged;

    /// <summary>
    /// An event that is invoked when the UI input is modified by the user.
    /// </summary>
    public event Action OnInputChanged;

    /// <summary>
    /// A function that processes an incoming new value and returns a transformed value.
    /// </summary>
    public Func<T, T> TransformFunc { get; private set; }

    /// <summary>
    /// A function that validated an incoming new value and returns true if it should be assigned to <see cref="Value"/>.
    /// </summary>
    /// <remarks>The validation check occurs after the new value has been transformed by <see cref="TransformFunc"/></remarks>
    public Func<T, bool> ValidateFunc { get; private set; }

    private protected TypedConfigInput(string name, string description, T defaultValue)
    {
        Name = name;
        Description = description;

        DefaultValue = defaultValue;
        ValueType = defaultValue.GetType();
    }

    internal sealed override void CreateMelonEntry(MelonPreferences_Category melonCategory)
    {
        MelonEntry = melonCategory.CreateEntry(Name, DefaultValue, Name, Description);
        Value = MelonEntry.Value; // Should automatically contain any loaded value, otherwise the default
    }

    internal override void OnUIChanged() => OnInputChanged?.Invoke();

    internal sealed override void ResetValueUI() => SetDisplayedValue(DefaultValue);

    internal sealed override void RefreshUI() => SetDisplayedValue(Value);

    /// <summary>
    /// Sets the UI value using an implementation specific to the input type.
    /// </summary>
    /// <param name="value">The value to insert into the UI input.</param>
    protected abstract void SetDisplayedValue(T value);


    /// <summary>
    /// Subscribes to the <see cref="OnValueChanged"/> event, which is invoked when <see cref="Value"/> is modified.
    /// </summary>
    /// <param name="onValueChanged">The action to invoke when the value changes, receiving the new value as a parameter.</param>
    public TypedConfigInput<T> WithOnValueChanged(Action<T> onValueChanged)
    {
        OnValueChanged += onValueChanged;
        return this;
    }

    /// <summary>
    /// Subscribes to the <see cref="OnInputChanged"/> event, which is invoked when the UI input is modified by the user.
    /// </summary>
    /// <param name="onInputChanged">The action to invoke when the UI input changes.</param>
    public TypedConfigInput<T> WithOnInputChanged(Action onInputChanged)
    {
        OnInputChanged += onInputChanged;
        return this;
    }

    /// <summary>
    /// Sets a function that transforms an incoming value before it is assigned to <see cref="Value"/>.
    /// </summary>
    /// <param name="transformFunc">A function that takes the incoming value and returns the transformed value.</param>
    public TypedConfigInput<T> WithTransformFunc(Func<T, T> transformFunc)
    {
        TransformFunc = transformFunc;
        return this;
    }

    /// <summary>
    /// Sets a function that validates an incoming value before it is assigned to <see cref="Value"/>.
    /// The validation check occurs after any transformation applied by <see cref="TransformFunc"/>.
    /// </summary>
    /// <param name="validateFunc">A function that returns true if the value should be assigned, or false to reject it.</param>
    public TypedConfigInput<T> WithValidateFunc(Func<T, bool> validateFunc)
    {
        ValidateFunc = validateFunc;
        return this;
    }
}