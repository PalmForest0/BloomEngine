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
    [HarmonyPatch(typeof(MainMenuPanelView), nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuPanelView_Start_Postfix(MainMenuPanelView __instance)
    {
        UIHelper.Initialize(__instance);
        CreateConfigPanels(__instance.transform.GetComponentInParent<PanelViewContainer>());
    }

    private static void CreateConfigPanels(PanelViewContainer container)
    {
        var template = container.m_panels.FirstOrDefault(p => p.m_id == "quit");

        // Create a modEntry panel for each mod with a registered (and not empty) modEntry
        foreach (ModMenuEntry modEntry in ModMenuService.ModEntries.Values)
        {
            ModConfig config = modEntry.Config;

            if (config is null || config.IsEmpty)
                continue;

            var panelObj = GameObject.Instantiate(template.gameObject, container.transform);
            config.Panel = new ConfigPanel(panelObj.GetComponent<PanelView>(), modEntry);
        }
    }
}