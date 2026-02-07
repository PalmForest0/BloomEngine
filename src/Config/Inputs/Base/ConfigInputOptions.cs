using BloomEngine.Config.Services;

namespace BloomEngine.Config.Inputs.Base;

/// <summary>
/// Optional options class through which additional logic can be added to a config input through actions and functions.<br/>
/// To create a config input with additional options, use one of the methods provided by <see cref="ConfigService"/> and pass in an instance of <see cref="ConfigInputOptions{T}"/>.
/// </summary>
/// <typeparam name="T">The value type of the config input.</typeparam>
public sealed class ConfigInputOptions<T>
{
    /// <inheritdoc cref="TypedConfigInput{T}.OnValueChanged"/>
    public Action<T> OnValueChanged { get; init; }

    /// <inheritdoc cref="TypedConfigInput{T}.OnInputChanged"/>
    public Action OnInputChanged { get; init; }

    /// <inheritdoc cref="TypedConfigInput{T}.TransformValue"/>
    public Func<T, T> TransformValue { get; init; }

    /// <inheritdoc cref="TypedConfigInput{T}.ValidateValue"/>
    public Func<T, bool> ValidateValue { get; init; }
}