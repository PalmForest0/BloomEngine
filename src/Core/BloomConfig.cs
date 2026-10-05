using BloomEngine.Config;
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
    
    public static readonly StringConfigField StringField = ConfigService.CreateString("String Field", "jarona")
        .WithOnInputChanged(ctx => ctx.InputValue = ctx.InputValue.ToUpperInvariant())
        .WithOnValueApplied(val => LogValueChanged(nameof(StringField), val));
    
    public static readonly EnumConfigField<TestEnum> EnumField = ConfigService.CreateEnum("Enum Field", TestEnum.ShortOption)
        .WithOptionOrder(TestEnum.ShortOption, TestEnum.ShorterEnumOption, TestEnum.EnumOptionWithAVeryLongName)
        .WithOptionNames(val => val.ToString().ToLowerInvariant())
        .WithOnValueApplied(val => LogValueChanged(nameof(EnumField), val));
    
    public static readonly FloatConfigField FloatField = ConfigService.CreateFloat("Float Field", 0.5f)
        .WithRange(0f, 2f)
        .WithOnValueApplied(val => LogValueChanged(nameof(FloatField), val));
    
    public static readonly BoolConfigField BoolField = ConfigService.CreateBool("Bool Field", false)
        .WithOnValueApplied(val => LogValueChanged(nameof(BoolField), val));
#endif
}