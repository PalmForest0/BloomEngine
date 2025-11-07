using BloomEngine.Menu;
using BloomEngine.Utilities;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppUI.Scripts;
using UnityEngine;

namespace PvZEnhanced.Patches;

[HarmonyPatch]
static internal class MainMenuPatches
{
    private static ModMenuManager modListManager;

    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuStart(MainMenuPanelView __instance)
    {
        UIHelper.Initialize(__instance);
    }

    [HarmonyPatch(typeof(AchievementsUI), nameof(AchievementsUI.Start))]
    [HarmonyPostfix]
    private static void AchievementsStart(AchievementsUI __instance)
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

        modListManager = modList.AddComponent<ModMenuManager>();
    }
}