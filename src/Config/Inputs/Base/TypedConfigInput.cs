using MelonLoader;

namespace BloomEngine.Config.Inputs.Base;

/// <summary>
/// Represents a generic config input with a specificly typed <see cref="Value"/>.
/// </summary>
/// <typeparam name="T">The type of value stored within this config input.</typeparam>
/// <typeparam name="TSelf">The type of this config input.</typeparam>
public abstract class TypedConfigInput<T, TSelf> : BaseConfigInput
    where T : notnull
    where TSelf : TypedConfigInput<T, TSelf>
{
    /// <summary>
    /// Gets or sets the value stored in this config input, invoking <see cref="TransformFunc"/>.
    /// If the new value is different to the old value, any handlers added with <see cref="WithOnValueChanged(Action{T})"/>
    /// are invoked and the <see cref="MelonEntry"/> value is updated.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            T newValue = TransformFunc is not null ? TransformFunc.Invoke(value) : value;

            // Do not assign new value if validation fails
            if (ValidateFunc is not null && !ValidateFunc.Invoke(newValue))
                return;

            // Don't call event or update MelonEntry value if there is no difference
            if (EqualityComparer<T>.Default.Equals(_value, newValue))
                return;
            
            _value = newValue;
            MelonEntry?.Value = newValue;

            OnValueChanged?.Invoke(newValue);
        }
    }

    /// <summary>
    /// The underlying field containing the value. Setting this directly is used to sidestep the MelonEntry update on init.
    /// </summary>
    private T _value;

    /// <summary>
    /// The default value of this config input. This is also used as a fallback when an unexpected value is encountered.
    /// </summary>
    public T DefaultValue { get; private init; }

    /// <summary>
    /// The type of value stored within this config input.
    /// </summary>
    public Type ValueType { get; private init; }

    /// <summary>
    /// The <see cref="MelonPreferences_Entry"/> that corresponds to this config input and contains the saved value.
    /// </summary>
    public MelonPreferences_Entry<T> MelonEntry { get; private set; } = null!;

    /// <summary>
    /// A function that processes an incoming new value and returns a transformed value.
    /// </summary>
    private Func<T, T>? TransformFunc;

    /// <summary>
    /// A function that validated an incoming new value and returns true if it should be assigned to <see cref="Value"/>.
    /// </summary>
    /// <remarks>The validation check occurs after the new value has been transformed by <see cref="TransformFunc"/></remarks>
    private Func<T, bool>? ValidateFunc;

    /// <summary>
    /// An event that is invoked when <see cref="Value"/> is modified.
    /// </summary>
    private event Action<T>? OnValueChanged;

    /// <summary>
    /// An event that is invoked when the UI input is modified by the user.
    /// </summary>
    private event Action? OnInputChanged;

    private protected TypedConfigInput(string name, string description, T defaultValue) : base(name, description)
    {
        DefaultValue = defaultValue;
        _value = defaultValue;
        ValueType = _value.GetType();
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
    /// Subscribes to an event which is invoked when <see cref="Value"/> is modified.
    /// </summary>
    /// <param name="handler">The action to invoke when the value changes, receiving the new value as a parameter.</param>
    public TSelf WithOnValueChanged(Action<T> handler)
    {
        OnValueChanged += handler;
        return (TSelf)this;
    }

    /// <summary>
    /// Subscribes to and event which is invoked immediately every time the UI input is modified by the user.
    /// Depending on the type of input, the UI element can be accessed to modify the value.
    /// </summary>
    /// <param name="handler">The action to invoke when the UI input is changed by the user.</param>
    public TSelf WithOnInputChanged(Action handler)
    {
        OnInputChanged += handler;
        return (TSelf)this;
    }

    /// <summary>
    /// Sets a function that transforms an incoming value before it is assigned to <see cref="Value"/>.<br/>
    /// Be sure that the validator added through <see cref="WithValidation(Func{T, bool})"/> will approve the transformed value.
    /// </summary>
    /// <param name="transform">A function that takes the incoming value and returns the transformed value.</param>
    public TSelf WithTransform(Func<T, T> transform)
    {
        TransformFunc = transform;
        return (TSelf)this;
    }

    /// <summary>
    /// Sets a function that validates an incoming value before it is assigned to <see cref="Value"/>.<br/>
    /// This validation check occurs after any transformations added with <see cref="WithTransform(Func{T, T})"/>.
    /// It is therefore important to ensure that the performed transformation is valid.
    /// </summary>
    /// <param name="validator">A function that returns true if the value should be assigned, or false to reject it.</param>
    public TSelf WithValidation(Func<T, bool> validator)
    {
        ValidateFunc = validator;
        return (TSelf)this;
    }
}