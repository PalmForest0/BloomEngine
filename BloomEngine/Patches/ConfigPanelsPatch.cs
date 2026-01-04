using BloomEngine.Config.Services;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using UnityEngine;

namespace PvZEnhanced.Patches;

[HarmonyPatch]
internal static class ConfigPanelsPatch
{
    private static PanelViewContainer globalPanels;
    private static bool configPanelsCreated = false;

    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelViewContainer_Awake_Postfix(PanelViewContainer __instance)
    {
        if (__instance.name == "GlobalPanels(Clone)")
            globalPanels = __instance;
    }


    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuPanelView_Start_Postfix(MainMenuPanelView __instance)
    {
        UIHelper.OnMainMenu(__instance, globalPanels);

        if(!configPanelsCreated)
        {
            CreateConfigPanels(UIHelper.MainMenu, UIHelper.GlobalPanels);
            configPanelsCreated = true;
        }
    }

    private static void CreateConfigPanels(MainMenuPanelView mainMenu, PanelViewContainer globalPanels)
    {
        var template = mainMenu.GetComponentInParent<PanelViewContainer>().m_panels.FirstOrDefault(p => p.m_id == "quit");

        // Create a modEntry panel for each mod with a registered (and not empty) modEntry
        foreach (ModMenuEntry modEntry in ModMenuService.ModEntries.Values)
        {
            ModConfig config = modEntry.Config;

            if (config is null || config.IsEmpty)
                continue;

            var panelObj = GameObject.Instantiate(template.gameObject, globalPanels.transform);
            config.Panel = new ConfigPanel(panelObj.GetComponent<PanelView>(), modEntry);
        }
    }
}