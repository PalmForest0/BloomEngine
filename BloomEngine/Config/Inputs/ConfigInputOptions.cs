namespace BloomEngine.Config.Inputs;

public sealed class ConfigInputOptions<T>
{
    public Action<T> OnValueChanged { get; init; }
    public Action OnInputChanged { get; init; }
    public Func<T, T> TransformValue { get; init; }
    public Func<T, T> ValidateValue { get; init; }
}