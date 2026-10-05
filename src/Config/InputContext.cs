namespace BloomEngine.Config;

/// <summary>
/// Gives access to a config field and the value that its input currently displays.
/// Assigning <see cref="InputValue"/> replaces the displayed value once all handlers run.
/// </summary>
/// <typeparam name="TField">The type of config field associated with the UI input.</typeparam>
/// <typeparam name="TValue">The type of value displayed by the UI input.</typeparam>
public sealed class InputContext<TField, TValue>
    where TField : notnull
    where TValue : notnull
{
    internal InputContext(TField owner, TValue inputValue)
    {
        Field = owner;
        InputValue = inputValue;
        
        Dirty = false; // Reset after setting InputValue
    }
    
    /// <summary>
    /// Whether <see cref="InputValue"/> has been assigned by a handler. When true, the new value is written back to the UI input.
    /// </summary>
    public bool Dirty { get; private set; }

    /// <summary>
    /// The config field that owns the UI input.
    /// </summary>
    public TField Field { get; }

    /// <summary>
    /// The value currently displayed by the UI input. Assigning a new value replaces the displayed value.
    /// </summary>
    public TValue InputValue
    {
        get;
        set
        {
            Dirty = true;
            field = value;
        }
    }
}