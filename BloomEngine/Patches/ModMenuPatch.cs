using BloomEngine.Menu;
using HarmonyLib;
using Il2CppUI.Scripts;
using UnityEngine;

namespace BloomEngine.Patches;

[HarmonyPatch(typeof(AchievementsUI))]
internal static class ModMenuPatch
{
    [HarmonyPatch(nameof(AchievementsUI.Start))]
    [HarmonyPostfix]
    private static void AchievementsUIStartPostfix(AchievementsUI __instance)
    {
        CreateModMenu(__instance);
    }

    private static void CreateModMenu(AchievementsUI ui)
    {
        if (ui.transform.Find("ModListManager") is not null)
            return;

        GameObject modList = new GameObject("ModListManager");
        modList.transform.SetParent(ui.transform, false);

        // Stretch to screen
        RectTransform rect = modList.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        modList.AddComponent<ModMenuManager>();
    }
}