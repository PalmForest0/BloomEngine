using BloomEngine.Menu;
using BloomEngine.Config;
using BloomEngine.Utilities;
using HarmonyLib;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using UnityEngine;

namespace PvZEnhanced.Patches;

[HarmonyPatch(typeof(MainMenuPanelView))]
static internal class ConfigPanelsPatch
{
    [HarmonyPatch(nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuStartPostfix(MainMenuPanelView __instance)
    {
        UIHelper.Initialize(__instance);
        CreateConfigPanels(__instance.transform.GetComponentInParent<PanelViewContainer>());
    }

    private static void CreateConfigPanels(PanelViewContainer container)
    {
        var template = container.m_panels.FirstOrDefault(p => p.m_id == "quit");

        // Create a config panel for each mod that has a config
        foreach (var mod in ModMenu.Mods.Values.Where(mod => mod is not null && mod.Config is not null && !mod.Config.Properties.IsNullOrEmpty()))
        {
            var panel = GameObject.Instantiate(template.gameObject, container.transform);
            var config = new ModConfig(panel.GetComponent<PanelView>(), mod);
            ModMenu.RegisterConfigPanel(config);
        }
    }
}