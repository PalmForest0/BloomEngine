using BloomEngine.Config;
using BloomEngine.Config.Inputs;

namespace BloomEngine.Core;

public static class BloomConfig
{
    private const string LogPrefix = $"[{nameof(BloomConfig)}] ";
    
#if DEBUG
    public static readonly EnumConfigInput<DayOfWeek> TestEnumField =
        ConfigService.CreateEnum<DayOfWeek>("Test Enum Field", "???", 0)
            .WithOptionOrder(DayOfWeek.Sunday)
            .WithOnValueChanged(day => BloomLogger.Debug($"Test Enum Field value set to: {day}", LogPrefix));
#endif
}