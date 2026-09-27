using BloomEngine.Config;
using BloomEngine.Config.Inputs;

namespace BloomEngine.Core;

public static class BloomConfig
{
    private const string LogPrefix = $"[{nameof(BloomConfig)}] ";
    
#if DEBUG
    public static readonly EnumConfigInput<DayOfWeek> TestEnumField = 
        ConfigService.CreateEnum<DayOfWeek>("test_enum_field", "Test Enum Field", DayOfWeek.Friday)
            .WithOptionOrder(DayOfWeek.Sunday)
            .WithOptionNames(day => day.ToString().ToUpperInvariant())
            .WithOnValueApplied(day => BloomLogger.Debug($"Value of Test Enum Field set to: {day}", LogPrefix));
    
    public static readonly FloatConfigInput TestFloatField = 
        ConfigService.CreateFloat("test_float_field", "Test Float Field", 0.25f, 0f, 1f)
            .WithOnValueApplied(val => BloomLogger.Debug($"Value of Test Float Field set to: {val}", LogPrefix));
#endif
}