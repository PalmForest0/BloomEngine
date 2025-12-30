using BloomEngine.Utilities;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using UnityEngine;
using BloomEngine.Services.ModMenu;
using BloomEngine.UI;

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

        // Create a config panel for each mod that has a config
        foreach (var mod in ModMenuService.Entries.Values.Where(mod => mod is not null && mod.HasConfig && !mod.ConfigInputFields.IsNullOrEmpty()))
        {
            var panel = GameObject.Instantiate(template.gameObject, container.transform);
            var config = new ConfigPanel(panel.GetComponent<PanelView>(), mod);
            ModMenuService.RegisterConfigPanel(config);
        }
    }
}