namespace BloomEngine.Config.Events;

/// <summary>
/// Provides the old value that was stored previously, as well as the new value that was just assigned.
/// </summary>
/// <typeparam name="T">The specific value type stored by the field providing these args.</typeparam>
public class ValueChangedEventArgs<T> : EventArgs where T : notnull
{
    /// <summary>
    /// The old value that was stored in the config field.
    /// </summary>
    public T OldValue { get; }
    
    /// <summary>
    /// A new value that has just been saved to the config field.
    /// </summary>
    public T NewValue { get; }

    internal ValueChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}