using BloomEngine.Config.Services;
using BloomEngine.ModMenu.Services;
using BloomEngine.ModMenu.UI;
using HarmonyLib;
using Il2CppUI.Scripts;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class ModMenuPatches
{
    /// <summary>
    /// When the achievements menu is configPanelsCreated, create the mod menu
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.Start))]
    [HarmonyPostfix]
    private static void AchievementsUI_Start_Postfix(AchievementsUI __instance)
    {
        if(!__instance.GetComponent<ModMenuManager>())
            __instance.gameObject.AddComponent<ModMenuManager>();
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