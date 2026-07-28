using BloomEngine.Modules;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal static class PanelViewPatches
{
    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelViewContainer_Awake_Postfix(PanelViewContainer __instance)
    {
        switch(__instance.name)
        {
            case "GlobalPanels(Clone)":
                BloomLoader.LoadGlobalPanels(__instance);
                return;
        }


        //// Global panels
        //if (__instance.name == "GlobalPanels(Clone)")
        //{
        //    BloomEngineBootstrap.GlobalPanels = __instance;
        //    BloomEngineBootstrap.TryInitMainMenu();
        //}
        //// Zen Garden panels
        //else if (__instance.name == "Panels" && __instance.transform.FindChild("P_ZenGarden_MainHUD"))
        //{
        //    UIHelper.ZenGardenPanels = __instance;
        //}
        //// Gameplay panels
        //else if (__instance.name == "Panels" && __instance.transform.FindChild("P_Gameplay_MainHUD"))
        //{
        //    UIHelper.GameplayPanels = __instance;
        //}
    }

    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuPanelView_Start_Postfix(MainMenuPanelView __instance)
    {
        BloomLoader.LoadMainMenu(__instance);
    }
}
