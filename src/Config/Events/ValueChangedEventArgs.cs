namespace BloomEngine.Config.Events;

public class ValueChangedEventArgs<T> : EventArgs where T : notnull
{
    public T OldValue { get; }
    public T NewValue { get; }

    internal ValueChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}