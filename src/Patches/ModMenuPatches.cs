using BloomEngine.Config;
using HarmonyLib;
using Il2CppUI.Scripts;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class ModMenuPatches
{
    /// <summary>
    /// Saves the AchievementsUI and attempts to initialize the mod menu.
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.Start))]
    [HarmonyPostfix]
    private static void AchievementsUI_Start_Postfix(AchievementsUI __instance)
    {
        BloomEngineBootstrap.AchievementsUI = __instance;
        BloomEngineBootstrap.TryInitMainMenu();
    }

    /// <summary>
    /// Hides the currently open config panel when the mod menu is closed.
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.SetAchievementsIsActive))]
    [HarmonyPrefix]
    private static void AchievementsUI_SetAchievementsIsActive_Prefix(AchievementsUI __instance, bool isActive)
    {
        if (!isActive && ConfigService.IsConfigPanelOpen)
            ConfigService.HideConfigPanel();
    }
}