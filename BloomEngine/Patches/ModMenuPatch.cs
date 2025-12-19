using BloomEngine.Menu;
using HarmonyLib;
using Il2CppUI.Scripts;
using UnityEngine;
using static Il2CppReloaded.Constants.Input.ActionMap;

namespace BloomEngine.Patches;

[HarmonyPatch(typeof(AchievementsUI))]
internal static class ModMenuPatches
{
    /// <summary>
    /// When the achievements menu is initialized, create the mod menu
    /// </summary>
    [HarmonyPatch(nameof(AchievementsUI.Start))]
    private static void Postfix(AchievementsUI __instance)
    {
        if (__instance.transform.Find("ModListManager") is not null)
            return;

        GameObject modList = new GameObject("ModListManager");
        modList.transform.SetParent(__instance.transform, false);

        // Stretch to screen
        RectTransform rect = modList.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        modList.AddComponent<ModMenuManager>();
    }

    /// <summary>
    /// When the mod menu is closed, hide the currently open config panel
    /// </summary>
    [HarmonyPatch(nameof(AchievementsUI.SetAchievementsIsActive))]
    private static void Prefix(AchievementsUI __instance, bool isActive)
    {
        if (!isActive && ModMenu.IsConfigPanelOpen)
            ModMenu.HideConfigPanel();
    }
}