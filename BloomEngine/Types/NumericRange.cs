using BloomEngine.Utilities;

namespace BloomEngine.Types;

public class NumericRange<T>(T minValue, T maxValue, T value)
{
    public T MinValue { get; set; } = minValue;
    public T MaxValue { get; set; } = maxValue;
    public T Value { get; set; } = value;

    internal bool IsValid { get; private init; } = TypeHelper.IsNumericType(typeof(T));
}
