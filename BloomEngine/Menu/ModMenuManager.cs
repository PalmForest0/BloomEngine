using BloomEngine.Utilities;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomEngine.Menu;

internal class ModMenuManager : MonoBehaviour
{
    public bool ModMenuOpen { get; private set; } = false;
    public List<GameObject> ModEntryObjects { get; } = new List<GameObject>();

    private Transform container;
    private AchievementsUI ui;

    public void Start()
    {
        ui = transform.GetComponentInParent<AchievementsUI>();

        container = transform.parent.Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");

        CreateButtons();
        CreateModsEntries();
    }


    private void CreateButtons()
    {
        GameObject obj = UIHelper.CreateButton("ModsButton", transform, "Mods", OpenModList);

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 100);

        // Update the achievements button to reset the text and hide the mod entries
        transform.parent.parent.FindComponent<Button>("Main/BG_Tree/AchievementsButton").onClick.AddListener((UnityAction)(() =>
        {
            SetHeaderText("Achievements");
            ShowModEntries(false);
        }));

        ui.m_backButton.onClick.AddListener((UnityAction)(() => ModMenuOpen = false));
    }

    private void CreateModsEntries()
    {
        // Prevent header from blocking clicks on modInfo registered buttons
        container.parent.FindComponent<Image>("Header/Shadow").raycastTarget = false;
        container.parent.FindComponent<Image>("Header/Left/Background_grass02").raycastTarget = false;

        foreach (var modInfo in MelonMod.RegisteredMelons.Select(m => m.Info))
        {
            // Create a new mod achievement for this mod
            GameObject modObj = GameObject.Instantiate(transform.parent.parent.Find("Achievements/AchievementItem").gameObject, container);
            modObj.SetActive(true);
            modObj.name = $"ModEntry_{modInfo.Name}";
            ModEntryObjects.Add(modObj);

            string name = modInfo.Name;
            string description = $"{modInfo.Author}\n{modInfo.Version}";

            // If the mod is registered in the ModMenu, use its display name and description
            if (ModMenu.Mods.TryGetValue(modInfo.Name, out ModEntry registered))
            {
                name = registered.DisplayName;
                description = registered.Description ?? description;

                // Create a button for the modInfo's config panel if it has one
                if (registered.Config is not null)
                {
                    Button configButton = modObj.transform.Find("Icon").gameObject.AddComponent<Button>();
                    configButton.onClick.AddListener(() => ModMenu.ShowConfigPanel(registered));
                }
            }

            modObj.FindComponent<TextMeshProUGUI>("Title").text = name;
            modObj.FindComponent<TextMeshProUGUI>("Subheader").text = description;
        }
    }

    /// <summary>
    /// Sets the title text of the container screen
    /// </summary>
    private void SetHeaderText(string text)
    {
        container.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRock").text = text;
        container.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRockTop").text = text;
        container.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRockBottom").text = text;
    }

    /// <summary>
    /// Replaces the achievement entries with the mod entires, or vice versa.
    /// </summary>
    /// <param name="show">Whether to show the mod list of the achievement list.</param>
    private void ShowModEntries(bool showMods)
    {
        // Change the state of all mod entries
        foreach(var mod in ModEntryObjects)
            mod.SetActive(showMods);

        // Change the state of all achievement entries
        for (int i = 0; i < container.childCount; i++)
        {
            GameObject achievement = container.GetChild(i).gameObject;

            if (achievement.name.StartsWith("P_") && achievement.name.EndsWith("(Clone)"))
                achievement.SetActive(!showMods);
        }
    }

    /// <summary>
    /// Opens the mod list and updates the UI.
    /// </summary>
    private void OpenModList()
    {
        SetHeaderText("Mods");
        ShowModEntries(true);

        PlayTransitionAnim();
        ModMenuOpen = true;
        ui.m_achievementsIsActive = true;
    }

    /// <summary>
    /// Triggers the animation that plays when the camera pans down to the container screen.
    /// </summary>
    private static void PlayTransitionAnim() => UIHelper.MainMenu.PlayAnimation("A_MainMenu_Achievements_In");
}