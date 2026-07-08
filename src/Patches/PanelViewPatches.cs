using BloomEngine.Helpers;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class PanelViewPatches
{
    /// <summary>
    /// Saves all PanelViewContainers under their specific type and attempts to initialize the mod menu.
    /// </summary>
    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelViewContainer_Awake_Postfix(PanelViewContainer __instance)
    {
        // Global panels
        if (__instance.name == "GlobalPanels(Clone)")
        {
            BloomEngineBootstrap.GlobalPanels = __instance;
            BloomEngineBootstrap.TryInitMainMenu();
        }
        // Zen Garden panels
        else if (__instance.name == "Panels" && __instance.transform.FindChild("P_ZenGarden_MainHUD"))
        {
            UIHelper.ZenGardenPanels = __instance;
        }
        // Gameplay panels
        else if (__instance.name == "Panels" && __instance.transform.FindChild("P_Gameplay_MainHUD"))
        {
            UIHelper.GameplayPanels = __instance;
        }
    }

    /// <summary>
    /// Saves the MainMenuPanelView and attempts to initialize the mod menu.
    /// </summary>
    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuPanelView_Start_Postfix(MainMenuPanelView __instance)
    {
        BloomEngineBootstrap.MainMenu = __instance;
        BloomEngineBootstrap.TryInitMainMenu();
    }
}
