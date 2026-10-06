using BloomEngine.Config.Events;
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
    
    private static void LogValueChanged<TField, TValue>(TField field, ValueChangedEventArgs<TValue> args) where TField : ConfigFieldBase where TValue : notnull =>
        BloomLogger.Debug($"Value of '{field.Name}' has been updated: '{args.OldValue}' -> '{args.NewValue}'", LogPrefix);

    public static readonly StringConfigField StringField = new("String Field", "jarona")
    {
        OnInputChanged = (_, args) => args.InputValue = args.InputValue.ToUpperInvariant(),
        Transform = val => val.ToUpperInvariant(),
        OnValueChanged = LogValueChanged
    };

    public static readonly EnumConfigField<TestEnum> EnumField = new("Enum Field", TestEnum.ShortOption)
    {
        OptionOrder = [TestEnum.ShortOption, TestEnum.ShorterEnumOption, TestEnum.EnumOptionWithAVeryLongName],
        OptionNames = opt => opt.ToString().ToLowerInvariant(),
        OnValueChanged = LogValueChanged
    };

    public static readonly FloatConfigField FloatField = new("Float Field", 0.5f)
    {
        MaxValue = 2f,
        OnValueChanged = LogValueChanged
    };

    public static readonly BoolConfigField BoolField = new("Bool Field", false)
    {
        OnValueChanged = LogValueChanged
    };
#endif
}