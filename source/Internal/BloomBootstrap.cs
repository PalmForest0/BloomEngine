using BloomEngine.Config;
using BloomEngine.Extensions;
using BloomEngine.ModList;
using BloomEngine.UI;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Il2CppUI.Scripts;
using MelonLoader;

namespace BloomEngine.Internal;

internal static class BloomBootstrap
{
    /// <summary>
    /// Specifies the prefix to use for all log messages from this service.
    /// </summary>
    private const string LogPrefix = $"[{nameof(BloomBootstrap)}] ";

    private static MainMenuPanelView? _mainMenuPanel;
    private static PanelViewContainer? _globalPanels;

    public static void OnMainMenuReady(MainMenuPanelView mainMenuPanels)
    {
        BloomLogger.Info($"{nameof(OnMainMenuReady)} called, attempting initialization.", LogPrefix);
        
        _mainMenuPanel = mainMenuPanels;
        InitializeIfReady();
    }

    public static void OnGlobalPanelsReady(PanelViewContainer globalPanels)
    {
        BloomLogger.Info($"{nameof(OnGlobalPanelsReady)} called, attempting initialization.", LogPrefix);
        
        _globalPanels = globalPanels;
        InitializeIfReady();
    }

    public static void OnAchievementsUIReady(AchievementsUI achievementsUI)
    {
        BloomLogger.Info($"{nameof(OnAchievementsUIReady)} called, creating the mod list.", LogPrefix);
        
        UIHelper.AchievementsUI = achievementsUI;
        MelonCoroutines.Start(ModListService.Co_CreateModList(achievementsUI));
    }

    private static void InitializeIfReady()
    {
        if(_mainMenuPanel.IsNull() || _globalPanels.IsNull())
            return;
        
        UIHelper.InitializeIfReady(_mainMenuPanel, _globalPanels);
        ConfigService.CreatePanelsIfReady(_mainMenuPanel, _globalPanels);
    }
}
