using BloomEngine.Core;
using BloomEngine.UI;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class PanelViewPatches
{
    /// <summary>
    /// Passes the loaded PanelViewContainer to BloomLoader on Awake after determining its type.
    /// </summary>
    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelViewContainer_Awake_Postfix(PanelViewContainer __instance)
    {
        if(__instance.name == "GlobalPanels(Clone)")
            BloomLoader.LoadGlobalPanels(__instance);
        else if(__instance.transform.FindChild("P_ZenGarden_MainHUD"))
            UIHelper.ZenGardenPanels = __instance;
        else if(__instance.transform.FindChild("P_Gameplay_MainHUD"))
            UIHelper.GameplayPanels = __instance;
    }

    /// <summary>
    /// Passes the MainMenuPanelView to BloomLoader on Start to init UIHelper and the mod list.
    /// </summary>
    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuPanelView_Start_Postfix(MainMenuPanelView __instance)
    {
        BloomLoader.LoadMainMenu(__instance);
    }
}
