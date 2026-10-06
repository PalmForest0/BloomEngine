namespace BloomEngine.Config.Events;

/// <summary>
/// Provides a handle that can be modified to change the value currently displayed by the UI input. This is done by setting <see cref="InputValue"/>.
/// </summary>
/// <typeparam name="T">The specific value type stored by the field providing these args.</typeparam>
public sealed class InputChangedEventArgs<T> : EventArgs where T : notnull
{
    /// <summary>
    /// Whether <see cref="InputValue"/> has been assigned by a handler. When true, the new value is written back to the UI input.
    /// </summary>
    internal bool Dirty { get; private set; }

    /// <summary>
    /// The value currently displayed by the UI input. Assigning a new value replaces the displayed value.
    /// </summary>
    public T InputValue
    {
        get;
        set
        {
            Dirty = true;
            field = value;
        }
    }
    
    internal InputChangedEventArgs(T inputValue)
    {
        InputValue = inputValue;
        Dirty = false; // Reset after setting InputValue
    }
}