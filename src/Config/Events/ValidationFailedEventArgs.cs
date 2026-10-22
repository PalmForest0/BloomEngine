namespace BloomEngine.Config.Events;

/// <summary>
/// Provides the incoming value that was disapproved by the field's validation function.
/// </summary>
/// <typeparam name="T">The specific value type stored by the field providing these args.</typeparam>
public class ValidationFailedEventArgs<T> : EventArgs where T : notnull
{
    /// <summary>
    /// The incoming value that was disapproved by the field's validation function.
    /// </summary>
    public T FailedValue { get; }

    internal ValidationFailedEventArgs(T failedValue)
    {
        FailedValue = failedValue;
    }
}