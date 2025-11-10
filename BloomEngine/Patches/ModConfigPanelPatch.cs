using BloomEngine.Menu;
using BloomEngine.Utilities;
using HarmonyLib;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal class ModConfigPanelPatch
{
    private static ModEntry currentEntry;
    private static PanelView configPanel;

    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelContainerAwake(PanelViewContainer __instance)
    {
        if (__instance.name != "FrontendPanels")
            return;

        var quitPanel = __instance.m_panels.FirstOrDefault(p => p.m_id == "quit");

        // Clone the quit panel to use as a base for the entry config panel
        if (quitPanel is not null)
        {
            var configPanel = GameObject.Instantiate(quitPanel, __instance.transform);
            CreateModConfigPanel(configPanel);
        }
    }

    /// <summary>
    /// Sets up a new panel to contain entry configuration options.
    /// </summary>
    private static void CreateModConfigPanel(PanelView panel)
    {
        panel.m_id = "modConfig";
        panel.name = "P_ModConfig";
        configPanel = panel;

        // Destroy pesky localizers
        foreach (var local in panel.GetComponentsInChildren<TextLocalizer>())
            GameObject.Destroy(local);

        var buttons = panel.transform.FindComponents<UnityEngine.UI.Button>("Canvas/Layout/Center/Window/Buttons");

        var applyBtn = buttons.FirstOrDefault(btn => btn.name == "P_BacicButton_Quit");
        if (applyBtn is not null)
        {
            // Update name and text
            applyBtn.name = "P_ConfigButton_Apply";
            applyBtn.GetComponentInChildren<TextMeshProUGUI>().SetText("Apply");

            // Remove garbage components
            GameObject.Destroy(applyBtn.GetComponent<Il2CppReloaded.ExitGame>());
            GameObject.Destroy(applyBtn.GetComponent<TextLocalizer>());

            // Update onClick event
            applyBtn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            applyBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)(() => { CloseConfigPanel(true); }));
        }

        // TODO: Setup Cancel button similarly to Apply button
        var cancelBtn = buttons.FirstOrDefault(btn => btn.name == "P_BacicButton_Cancel");
        if (cancelBtn is not null)
        {
            cancelBtn.name = "P_ConfigButton_Cancel";
            cancelBtn.GetComponentInChildren<TextMeshProUGUI>().SetText("Cancel");
            GameObject.Destroy(cancelBtn.GetComponent<TextLocalizer>());

            cancelBtn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)(() => { CloseConfigPanel(false); }));
        }
    }

    /// <summary>
    /// Closes the mod configuration panel and updates any changed config properties by calling their setters if needed.
    /// </summary>
    private static void CloseConfigPanel(bool applyChanges)
    {
        // TODO: Update config properties by calling their setters
        ModMenu.Log($"[ModMenu] Applying config for mod: {currentEntry.DisplayName}");

        currentEntry = null;
        configPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the mod configuration panel for the specified entry and creates inputs for the config properties.
    /// </summary>
    private static void OpenConfigPanel(ModEntry entry)
    {
        // TODO: Create inputs for each config property in entry.Properties
        ModMenu.Log($"[ModMenu] Editing config for mod: {entry.DisplayName}");

        currentEntry = entry;
        configPanel.gameObject.SetActive(true);
    }
}
