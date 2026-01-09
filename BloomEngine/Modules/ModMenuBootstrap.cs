using BloomEngine.Config.Services;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using BloomEngine.ModMenu.UI;
using BloomEngine.Utilities;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Il2CppUI.Scripts;
using UnityEngine;

namespace BloomEngine;

internal static class ModMenuBootstrap
{
    public static AchievementsUI AchievementsUI { get; set; }
    public static MainMenuPanelView MainMenu { get; set; }
    public static PanelViewContainer GlobalPanels { get; set; }

    private static bool _initialized;

    public static void TryInitialize()
    {
        if (!AchievementsUI)
            return;
        if (!MainMenu)
            return;
        if (!GlobalPanels)
            return;

        InitOnSceneLoad();

        if (!_initialized)
        {
            InitOnce();
            _initialized = true;
        }
    }

    private static void InitOnce()
    {
        // Create config panels globally only once
        CreateConfigPanels(MainMenu, GlobalPanels);
    }

    private static void InitOnSceneLoad()
    {
        // Setup UI Helper and mod menu every time the main menu scene loads
        UIHelper.Initialize(MainMenu, GlobalPanels, AchievementsUI);
        ModMenuService.ModMenuUI = new ModMenuManager(AchievementsUI);
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