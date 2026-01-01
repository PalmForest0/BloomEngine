using MelonLoader;

namespace BloomEngine.Config.Inputs;

public abstract class TypedConfigInput<T> : BaseConfigInput
{
    public T Value
    {
        get => field;
        set
        {
            T oldValue = field;
            field = TransformValue is not null ? TransformValue.Invoke(value) : value;

            // Don't call event or update MelonEntry defaultValue if there was no change
            if (!EqualityComparer<T>.Default.Equals(field, oldValue))
            {
                OnValueChanged?.Invoke(field);
                MelonEntry?.Value = field;
            }
        }
    }

    public T DefaultValue { get; private init; }

    public MelonPreferences_Entry<T> MelonEntry { get; private set; }

    public Action<T> OnValueChanged { get; set; }
    public Action OnInputChanged { get; set; }
    public Func<T, T> TransformValue { get; set; }
    public Func<T, bool> ValidateValue { get; set; }

    protected TypedConfigInput(string name, string description, T defaultValue, ConfigInputOptions<T> options = null)
    {
        Name = name;
        Description = description;

        Value = defaultValue;
        DefaultValue = defaultValue;
        ValueType = defaultValue.GetType();

        OnValueChanged = options.OnValueChanged;
        OnInputChanged = options.OnInputChanged;
        TransformValue = options.TransformValue;
        ValidateValue = options.ValidateValue;
    }

    internal override void CreateMelonEntry(MelonPreferences_Category melonCategory)
    {
        MelonEntry = melonCategory.CreateEntry(Name, DefaultValue, Name, Description);
        Value = MelonEntry.Value;
    }

    internal override void OnUIChanged() => OnInputChanged?.Invoke();
}