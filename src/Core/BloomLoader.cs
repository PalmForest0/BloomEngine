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
    internal static MelonLogger.Instance Log { get; } = new MelonLogger.Instance($"{nameof(BloomEngine)}.{nameof(BloomLoader)}");

    public static MainMenuPanelView? MainMenuPanel { get; private set; }
    public static PanelViewContainer? GlobalPanels { get; private set; }

    public static void LoadMainMenu(MainMenuPanelView mainMenuPanels)
    {
        Log.Msg("Loading main menu...");

        ConfigService.TryCreateConfigPanels(mainMenuPanels, GlobalPanels);
        UIHelper.TryLoadAll(mainMenuPanels, GlobalPanels);

        MainMenuPanel = mainMenuPanels;
    }

    public static void LoadGlobalPanels(PanelViewContainer globalPanels)
    {
        Log.Msg("Loading global panel container...");

        ConfigService.TryCreateConfigPanels(MainMenuPanel, globalPanels);
        UIHelper.TryLoadAll(MainMenuPanel, globalPanels);

        GlobalPanels = globalPanels;
    }

    public static void LoadAchievementsUI(AchievementsUI achievementsUI)
    {
        Log.Msg("Loading achievements UI...");

        MelonCoroutines.Start(ModMenuService.Co_CreateModMenu(achievementsUI));

        UIHelper.AchievementsUI = achievementsUI;
    }
}
