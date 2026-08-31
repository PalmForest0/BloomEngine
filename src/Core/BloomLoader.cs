using BloomEngine.Config;
using BloomEngine.ModMenu;
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

        ConfigService.TryCreateConfigPanels(mainMenuPanels, GlobalPanels);
        UIHelper.TryLoadAll(mainMenuPanels, GlobalPanels);

        MainMenuPanel = mainMenuPanels;
    }

    public static void LoadGlobalPanels(PanelViewContainer globalPanels)
    {
        BloomLogger.Info("Loading global panel container...", LogPrefix);

        ConfigService.TryCreateConfigPanels(MainMenuPanel, globalPanels);
        UIHelper.TryLoadAll(MainMenuPanel, globalPanels);

        GlobalPanels = globalPanels;
    }

    public static void LoadAchievementsUI(AchievementsUI achievementsUI)
    {
        BloomLogger.Info("Loading achievements UI...", LogPrefix);

        MelonCoroutines.Start(ModMenuService.Co_CreateModMenu(achievementsUI));

        UIHelper.AchievementsUI = achievementsUI;
    }
}
