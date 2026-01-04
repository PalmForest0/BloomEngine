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
            T newValue = TransformValue is not null ? TransformValue.Invoke(value) : value;

            // Do not assign new value if validation fails
            if (ValidateValue is not null && !ValidateValue.Invoke(newValue))
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
    /// An action that is invoked when <see cref="Value"/> is modified.
    /// </summary>
    public Action<T> OnValueChanged { get; set; }

    /// <summary>
    /// An action that is invoked when the UI input is modified by the user.
    /// </summary>
    public Action OnInputChanged { get; set; }

    /// <summary>
    /// A function that processes an incoming new value and returns a transformed value.
    /// </summary>
    public Func<T, T> TransformValue { get; set; }

    /// <summary>
    /// A function that validated an incoming new value and returns true if it should be assigned to <see cref="Value"/>.
    /// </summary>
    /// <remarks>The validation check occurs after the new value has been transformed by <see cref="TransformValue"/></remarks>
    public Func<T, bool> ValidateValue { get; set; }

    protected TypedConfigInput(string name, string description, T defaultValue, ConfigInputOptions<T> options = null)
    {
        Name = name;
        Description = description;

        OnValueChanged = options.OnValueChanged;
        OnInputChanged = options.OnInputChanged;
        TransformValue = options.TransformValue;
        ValidateValue = options.ValidateValue;

        Value = defaultValue;
        DefaultValue = Value;
        ValueType = defaultValue.GetType();
    }

    internal sealed override void CreateMelonEntry(MelonPreferences_Category melonCategory)
    {
        MelonEntry = melonCategory.CreateEntry(Name, DefaultValue, Name, Description);
        Value = MelonEntry.Value;
    }

    internal override void OnUIChanged() => OnInputChanged?.Invoke();

    internal sealed override void ResetValueUI() => SetDisplayedValue(DefaultValue);

    internal sealed override void RefreshUI() => SetDisplayedValue(Value);

    /// <summary>
    /// Sets the UI value using an implementation specific to the input type.
    /// </summary>
    /// <param name="value">The value to insert into the UI input.</param>
    protected abstract void SetDisplayedValue(T value);
}