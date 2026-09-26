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

    private static MainMenuPanelView? _mainMenuPanel;
    private static PanelViewContainer? _globalPanels;

    public static void LoadMainMenu(MainMenuPanelView mainMenuPanels)
    {
        BloomLogger.Info("Loading main menu...", LogPrefix);
        
        _mainMenuPanel = mainMenuPanels;
        TryInitializeAll();
    }

    public static void LoadGlobalPanels(PanelViewContainer globalPanels)
    {
        BloomLogger.Info("Loading global panel container...", LogPrefix);
        
        _globalPanels = globalPanels;
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
        if(_mainMenuPanel.IsNull() || _globalPanels.IsNull())
            return;
        
        UIHelper.TryLoadAll(_mainMenuPanel, _globalPanels);
        ConfigService.TryCreateConfigPanels(_mainMenuPanel, _globalPanels);
    }
}
