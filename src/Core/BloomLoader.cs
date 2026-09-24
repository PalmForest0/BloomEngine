using BloomEngine.Config;
using BloomEngine.Extensions;
using BloomEngine.ModList;
using BloomEngine.UI;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Il2CppUI.Scripts;
using MelonLoader;

namespace BloomEngine.Core;

internal static class BloomLoader
{
    /// <summary>
    /// Specifies the prefix to use for all log messages from this service.
    /// </summary>
    private const string LogPrefix = $"[{nameof(BloomLoader)}] ";

    public static MainMenuPanelView? MainMenuPanel { get; private set; }
    public static PanelViewContainer? GlobalPanels { get; private set; }

    public static void LoadMainMenu(MainMenuPanelView mainMenuPanels)
    {
        BloomLogger.Info("Loading main menu...", LogPrefix);
        
        MainMenuPanel = mainMenuPanels;
        TryInitializeAll();
    }

    public static void LoadGlobalPanels(PanelViewContainer globalPanels)
    {
        BloomLogger.Info("Loading global panel container...", LogPrefix);
        
        GlobalPanels = globalPanels;
        TryInitializeAll();
    }

    public static void LoadAchievementsUI(AchievementsUI achievementsUI)
    {
        BloomLogger.Info("Loading achievements UI...", LogPrefix);
        
        UIHelper.AchievementsUI = achievementsUI;
        MelonCoroutines.Start(ModListService.Co_CreateModList(achievementsUI));
    }

    private static void TryInitializeAll()
    {
        if(MainMenuPanel.IsNull() || GlobalPanels.IsNull())
            return;
        
        UIHelper.TryLoadAll(MainMenuPanel, GlobalPanels);
        ConfigService.TryCreateConfigPanels(MainMenuPanel, GlobalPanels);
    }
}
