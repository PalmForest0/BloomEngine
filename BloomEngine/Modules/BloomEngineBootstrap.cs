using BloomEngine.Config.Services;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using BloomEngine.ModMenu.UI;
using BloomEngine.Utilities;
using BloomEngine.Extensions;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Il2CppUI.Scripts;
using UnityEngine;

namespace BloomEngine;

internal static class BloomEngineBootstrap
{
    public static AchievementsUI AchievementsUI { get; set; }
    public static MainMenuPanelView MainMenu { get; set; }
    public static PanelViewContainer GlobalPanels { get; set; }

    private static int initIndex = 0;

    public static void TryInitialize()
    {
        if (!AchievementsUI)
            return;
        if (!MainMenu)
            return;
        if (!GlobalPanels)
            return;

        // Setup UI Helper and mod menu every time the main menu scene loads
        UIHelper.OnMainMenuLoaded(MainMenu, GlobalPanels, AchievementsUI);

        // Create UI templates only on the first load
        if (initIndex == 0)
        {
            // Create all UI templates only once
            UIHelper.CreateTemplates();

            // Create config panels globally only once
            CreateConfigPanels(MainMenu, GlobalPanels);
        }

        // Recreate the mod menu every time the main menu loads
        ModMenuService.ModMenuUI = new ModMenuUI(AchievementsUI);

        initIndex++;
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