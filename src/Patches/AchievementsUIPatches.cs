using HarmonyLib;
using BloomEngine.Config;
using BloomEngine.Internal;
using Il2CppUI.Scripts;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class AchievementsUIPatches
{
    /// <summary>
    /// Loads the AchievementsUI and passes it to BloomLoader to init.
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.Start))]
    [HarmonyPostfix]
    private static void AchievementsUI_Start_Postfix(AchievementsUI __instance)
    {
        BloomBootstrap.OnAchievementsUIReady(__instance);
    }

    /// <summary>
    /// Hides the currently open config panel when the mod list is closed.
    /// </summary>
    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.SetAchievementsIsActive))]
    [HarmonyPrefix]
    private static void AchievementsUI_SetAchievementsIsActive_Prefix(AchievementsUI __instance, bool isActive)
    {
        if (!isActive && ConfigService.IsConfigPanelOpen)
            ConfigService.HideConfigPanel();
    }
}