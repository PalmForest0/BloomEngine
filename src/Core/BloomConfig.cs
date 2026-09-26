using BloomEngine.Config;
using BloomEngine.Config.Fields;

namespace BloomEngine.Core;

public static class BloomConfig
{
    private const string LogPrefix = $"[{nameof(BloomConfig)}] ";
    
#if DEBUG
    public static readonly EnumConfigField<FileAccess> TestEnumField =
        ConfigService.CreateEnum<FileAccess>("Test Enum Field", "???", FileAccess.Write)
            .WithOptionOrder(FileAccess.ReadWrite)
            .WithOnValueApplied(day => BloomLogger.Debug($"Test Enum Field value set to: {day}", LogPrefix));
#endif
}