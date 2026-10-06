using BloomEngine.Config.Fields;

namespace BloomEngine.Core;

public static class BloomConfig
{
#if DEBUG
    public enum TestEnum
    {
        ThisEnumOptionHasTheLongestNameEver,
        EnumOptionWithAVeryLongName,
        ShorterEnumOption,
        ShortOption
    }

    private const string LogPrefix = $"[{nameof(BloomConfig)}] ";
    
    private static void LogValueChanged(string fieldName, object newValue) =>
        BloomLogger.Debug($"Value of {fieldName} set to: '{newValue}'", LogPrefix);

    public static readonly StringConfigField StringField = new("String Field", "jarona")
    {
        OnInputChanged = ctx => ctx.InputValue = ctx.InputValue.ToUpperInvariant(),
        Transform = val => val.ToUpperInvariant(),
        OnValueChanged = val => LogValueChanged(nameof(StringField), val)
    };

    public static readonly EnumConfigField<TestEnum> EnumField = new("Enum Field", TestEnum.ShortOption)
    {
        OptionOrder = [TestEnum.ShortOption, TestEnum.ShorterEnumOption, TestEnum.EnumOptionWithAVeryLongName],
        OptionNames = opt => opt.ToString().ToLowerInvariant(),
        OnValueChanged = val => LogValueChanged(nameof(EnumField), val)
    };

    public static readonly FloatConfigField FloatField = new("Float Field", 0.5f)
    {
        MaxValue = 2f,
        OnValueChanged = val => LogValueChanged(nameof(FloatField), val)
    };

    public static readonly BoolConfigField BoolField = new("Bool Field", false)
    {
        OnValueChanged = val => LogValueChanged(nameof(BoolField), val)
    };
#endif
}