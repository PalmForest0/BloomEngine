namespace BloomEngine.Interfaces;

public interface IInputField<T> : IInputField
{
    Action<T> OnValueChanged { get; set; }
    Action OnInputChanged { get; set; }
    Func<T, T> TransformValue { get; set; }
    Func<T, bool> ValidateValue { get; set; }

    T Value { get; set; }
}