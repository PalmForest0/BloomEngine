using BloomEngine.Config.Services;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Il2CppUI.Scripts;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class ModMenuPatches
{
    /// <summary>
    /// Save the AchievementsUI and try to initialize the mod menu.
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.Start))]
    [HarmonyPostfix]
    private static void AchievementsUI_Start_Postfix(AchievementsUI __instance)
    {
        BloomEngineBootstrap.AchievementsUI = __instance;
        BloomEngineBootstrap.TryInitialize();
    }

    /// <summary>
    /// Save the global PanelViewContainer and try to initialize the mod menu.
    /// </summary>
    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelViewContainer_Awake_Postfix(PanelViewContainer __instance)
    {
        if (__instance.name == "GlobalPanels(Clone)")
        {
            BloomEngineBootstrap.GlobalPanels = __instance;
            BloomEngineBootstrap.TryInitialize();
        }
    }

    /// <summary>
    /// Save the MainMenuPanelView and try to initialize the mod menu.
    /// </summary>
    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuPanelView_Start_Postfix(MainMenuPanelView __instance)
    {
        BloomEngineBootstrap.MainMenu = __instance;
        BloomEngineBootstrap.TryInitialize();
    }

    /// <summary>
    /// When the mod menu is closed, hide the currently open config panel
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.SetAchievementsIsActive))]
    [HarmonyPrefix]
    private static void AchievementsUI_SetAchievementsIsActive_Prefix(AchievementsUI __instance, bool isActive)
    {
        if (!isActive && ConfigService.IsConfigPanelOpen)
            ConfigService.HideConfigPanel();
    }
}