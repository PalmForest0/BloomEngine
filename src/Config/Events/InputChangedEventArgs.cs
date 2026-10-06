namespace BloomEngine.Config.Events;

public sealed class InputChangedEventArgs<T> : EventArgs where T : notnull
{
    internal InputChangedEventArgs(T inputValue)
    {
        InputValue = inputValue;
        Dirty = false; // Reset after setting InputValue
    }
    
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
}