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
    private readonly List<GameObject> entryObjects = new List<GameObject>();
    private Transform container;
    private AchievementsUI ui;

    public void Start()
    {
        ui = transform.GetComponentInParent<AchievementsUI>();

        container = transform.parent.Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");

        CreateButtons();

        // Prevent header from blocking clicks on mod entries
        container.parent.FindComponent<Image>("Header/Shadow").raycastTarget = false;
        container.parent.FindComponent<Image>("Header/Left/Background_grass02").raycastTarget = false;

        CreateEntries();
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
            SetEntriesEnabled(false);
        }));

        ui.m_backButton.onClick.AddListener((UnityAction)(() => ModMenuOpen = false));
    }

    private void CreateEntries()
    {
        foreach (var mod in MelonMod.RegisteredMelons)
        {
            // Create a new mod achievement for this mod
            GameObject modObj = GameObject.Instantiate(transform.parent.parent.Find("Achievements/AchievementItem").gameObject, container);
            modObj.SetActive(true);
            modObj.name = $"ModEntry_{mod.Info.Name}";
            entryObjects.Add(modObj);

            string name = mod.Info.Name;
            string description = $"{mod.Info.Author}\n{mod.Info.Version}";

            // If the mod is registered in the ModMenu, use its display name and description
            if (ModMenu.Entries.TryGetValue(mod, out ModEntry registered))
            {
                name = registered.DisplayName;
                description = registered.Description ?? description;

                // Create a button for the modInfo's config panel if it has one
                if (registered.HasConfig)
                {
                    Button configButton = modObj.transform.Find("Icon").gameObject.AddComponent<Button>();
                    configButton.onClick.AddListener(() => ModMenu.ShowConfigPanel(registered));
                }
            }
            // If it isn't registered, make its heading yellow
            else modObj.FindComponent<TextMeshProUGUI>("Title").color = new Color(1f, 0.6f, 0.1f, 1f);

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
    /// <param name="enableMods">Whether to show the mod list or the achievement list.</param>
    private void SetEntriesEnabled(bool enableMods)
    {
        // Change the state of all mod entries
        foreach(var mod in entryObjects)
            mod.SetActive(enableMods);

        // Change the state of all achievement entries
        for (int i = 0; i < container.childCount; i++)
        {
            GameObject achievement = container.GetChild(i).gameObject;

            if (achievement.name.StartsWith("P_") && achievement.name.EndsWith("(Clone)"))
                achievement.SetActive(!enableMods);
        }
    }

    /// <summary>
    /// Opens the mod list and updates the UI.
    /// </summary>
    private void OpenModList()
    {
        SetHeaderText("Mods");
        SetEntriesEnabled(true);

        PlayTransitionAnim();
        ModMenuOpen = true;
        ui.m_achievementsIsActive = true;
    }

    /// <summary>
    /// Triggers the animation that plays when the camera pans down to the container screen.
    /// </summary>
    private static void PlayTransitionAnim() => UIHelper.MainMenu.PlayAnimation("A_MainMenu_Achievements_In");
}