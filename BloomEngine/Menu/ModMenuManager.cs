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

    private Transform achieveContainer;
    private Button achieveButton;
    private AchievementsUI achieveUi;

    public void Start()
    {
        achieveUi = transform.GetComponentInParent<AchievementsUI>();

        achieveContainer = transform.parent.Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");
        achieveButton = transform.parent.parent.FindComponent<Button>("Main/BG_Tree/AchievementsButton");

        CreateModsEntries();
        CreateModsButton();
    }


    private void CreateModsButton()
    {
        GameObject obj = UIHelper.CreateButton("ModsButton", transform, "Mods", ShowModList);

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 100);

        achieveButton.onClick.AddListener((UnityAction)ShowAchievementList);
        achieveUi.m_backButton.onClick.AddListener((UnityAction)(() => ModMenuOpen = false));
    }

    private void CreateModsEntries()
    {
        GameObject prefab = transform.parent.parent.Find("Achievements/AchievementItem").gameObject;

        // Prevent header from blocking clicks on mod entry buttons
        achieveContainer.parent.FindComponent<Image>("Header/Shadow").raycastTarget = false;
        achieveContainer.parent.FindComponent<Image>("Header/Left/Background_grass02").raycastTarget = false;

        foreach (var mod in MelonMod.RegisteredMelons)
        {
            GameObject obj = GameObject.Instantiate(prefab, achieveContainer);
            obj.SetActive(true);
            obj.name = $"ModEntry_{mod.Info.Name}";

            string name = mod.Info.Name;
            string description = $"{mod.Info.Author}\n{mod.Info.Version}";

            if (ModMenu.Mods.TryGetValue(mod.Info.Name, out ModEntry entry))
            {
                name = entry.DisplayName;
                description = entry.Description ?? description;

                // Create a button for the mod's config panel if it has one
                if (entry.Config is not null)
                {
                    obj.transform.Find("Icon").gameObject.AddComponent<Button>().onClick.AddListener((UnityAction)(() =>
                    {
                        ModMenu.ShowConfigPanel(entry);
                    }));
                }
            }

            obj.FindComponent<TextMeshProUGUI>("Title").text = name;
            obj.FindComponent<TextMeshProUGUI>("Subheader").text = description;
        }
    }

    private void ShowModList()
    {
        SetHeaderText("Mods");
        ReplaceWithModList(true);

        PanScreenDown();
        ModMenuOpen = true;
        achieveUi.m_achievementsIsActive = true;
    }

    private void ShowAchievementList()
    {
        SetHeaderText("Achievements");
        ReplaceWithModList(false);
    }

    /// <summary>
    /// Sets the title text of the achievements screen
    /// </summary>
    private void SetHeaderText(string text)
    {
        achieveContainer.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRock").text = text;
        achieveContainer.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRockTop").text = text;
        achieveContainer.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRockBottom").text = text;
    }

    /// <summary>
    /// Replaces all the achievements with the loaded mods
    /// </summary>
    /// <param name="show">Whether to show the mod list of to return the achievement list</param>
    private void ReplaceWithModList(bool showMods)
    {
        for (int i = 0; i < achieveContainer.childCount; i++)
        {
            GameObject entry = achieveContainer.GetChild(i).gameObject;

            if (entry.name.StartsWith("ModEntry_"))
                entry.SetActive(showMods);
            else if (entry.name.StartsWith("P_") && entry.name.EndsWith("(Clone)"))
                entry.SetActive(!showMods);
        }
    }

    /// <summary>
    /// Opens the achievement screen below the main menu
    /// </summary>
    private void PanScreenDown()
    {
        UIHelper.MainMenu.PlayAnimation("A_MainMenu_Achievements_In");
    }
}
