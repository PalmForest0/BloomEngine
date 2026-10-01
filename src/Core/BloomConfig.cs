using BloomEngine.Config.Fields;

namespace BloomEngine.Core;

public static class BloomConfig
{
    private const string LogPrefix = $"[{nameof(BloomConfig)}] ";
    
#if DEBUG
    public enum TestEnum
    {
        ThisEnumOptionHasTheLongestNameEver,
        EnumOptionWithAVeryLongName,
        ShorterEnumOption,
        ShortOption
    }
    
    public static readonly StringConfigField TestStringField = 
        new StringConfigField("test_string_field", "Test String Field", "jarona")
            .WithOnInputChanged(field => field.InputValue = field.InputValue.ToUpperInvariant())
            .WithTransform(val => val.ToUpperInvariant())
            .WithOnValueApplied(val => BloomLogger.Debug($"Value of Test String Field set to: {val}", LogPrefix));
    
    public static readonly EnumConfigField<TestEnum> TestEnumField = 
        new EnumConfigField<TestEnum>("test_enum_field", "Test Enum Field", TestEnum.ShortOption)
            .WithOptionOrder(TestEnum.ShortOption, TestEnum.ShorterEnumOption, TestEnum.EnumOptionWithAVeryLongName)
            .WithOptionNames(val => val.ToString().ToUpperInvariant())
            .WithOnValueApplied(val => BloomLogger.Debug($"Value of Test Enum Field set to: {val}", LogPrefix));
    
    public static readonly FloatConfigField TestFloatField = 
        new FloatConfigField("test_float_field", "Test Float Field", 0.5f)
            .WithRange(0f, 2f)
            .WithOnValueApplied(val => BloomLogger.Debug($"Value of Test Float Field set to: {val}", LogPrefix));
    
    public static readonly BoolConfigField TestBoolField = 
        new BoolConfigField("test_bool_field", "Test Bool Field", false)
            .WithOnValueApplied(val => BloomLogger.Debug($"Value of Test Bool Field set to: {val}", LogPrefix));
#endif
}