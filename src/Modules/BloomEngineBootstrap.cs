using BloomEngine.Config.UI;
using BloomEngine.ModMenu.UI;
using Il2CppReloaded.UI;
using Il2CppTekly.PanelViews;
using Il2CppUI.Scripts;
using UnityEngine;
using BloomEngine.Helpers;
using BloomEngine.ModMenu;
namespace BloomEngine;

internal static class BloomEngineBootstrap
{
    public static AchievementsUI AchievementsUI { get; set; }
    public static MainMenuPanelView MainMenu { get; set; }
    public static PanelViewContainer GlobalPanels { get; set; }

    private static bool CanInitialize => AchievementsUI && MainMenu && GlobalPanels;

    private static int initIndex = 0;

    public static void TryInitMainMenu()
    {
        if (!CanInitialize)
            return;

        OnEarlyInit();

        if (initIndex++ == 0)
            OnFirstInit();

        OnEveryInit();
    }

    private static void OnEarlyInit()
    {
        // Setup UI Helper and mod menu every time the main menu scene loads
        UIHelper.OnMainMenuLoaded(MainMenu, GlobalPanels, AchievementsUI);
    }

    private static void OnFirstInit()
    {
        // Create all UI templates only once
        UIHelper.CreateTemplates();

        // Create config panels globally only once
        CreateConfigPanels(MainMenu, GlobalPanels);
    }

    private static void OnEveryInit()
    {
        // Recreate the mod menu every time the main menu loads
        ModMenuService.ModMenuUI = new ModMenuUI(AchievementsUI);
    }


    /// <summary>
    /// Clones an existing panel for every registered mod with a config and creates a new ConfigPanel for it.
    /// </summary>
    private static void CreateConfigPanels(MainMenuPanelView mainMenu, PanelViewContainer globalPanels)
    {
        var template = mainMenu.GetComponentInParent<PanelViewContainer>().m_panels.FirstOrDefault(p => p.m_id == "quit");

        // Create a config panel for each mod entry with a registered config
        foreach (var config in ModMenuService.RegisteredEntries.Where(e => e.HasConfigInputs).Select(e => e.Config))
        {
            var panelObj = GameObject.Instantiate(template.gameObject, globalPanels.transform);
            config.Panel = new ConfigPanel(panelObj.GetComponent<PanelView>(), config);
        }
    }
}