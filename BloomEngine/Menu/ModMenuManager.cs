using BloomEngine.Patches;
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
    private UnityEngine.UI.Button achieveButton;
    private AchievementsUI achieveUi;

    public void Start()
    {
        achieveUi = transform.GetComponentInParent<AchievementsUI>();

        achieveContainer = transform.parent.Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");
        achieveButton = transform.parent.parent.FindComponent<UnityEngine.UI.Button>("Main/BG_Tree/AchievementsButton");

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
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 25);

        achieveButton.onClick.AddListener((UnityAction)ShowAchievementList);
        achieveUi.m_backButton.onClick.AddListener((UnityAction)(() => ModMenuOpen = false));
    }

    private void CreateModsEntries()
    {
        GameObject prefab = transform.parent.parent.Find("Achievements/AchievementItem").gameObject;

        foreach (var mod in MelonMod.RegisteredMelons)
        {
            GameObject modEntry = GameObject.Instantiate(prefab, achieveContainer);
            modEntry.SetActive(true);
            modEntry.name = $"ModEntry_{mod.Info.Name}";

            ModEntry entry = ModMenu.Entries.FirstOrDefault(e => e.Mod == mod);
            string name = mod.Info.Name;
            string description = $"{mod.Info.Author}\n{mod.Info.Version}";

            if (entry is not null)
            {
                name = entry.DisplayName;

                if (!string.IsNullOrWhiteSpace(entry.Description))
                    description = entry.Description;

                Button button = modEntry.transform.Find("Icon").gameObject.AddComponent<Button>();
                button.onClick.AddListener((UnityAction)(() => ModConfigPanelPatch.OpenConfigPanel(entry)));
            }

            modEntry.FindComponent<TextMeshProUGUI>("Title").text = name;
            modEntry.FindComponent<TextMeshProUGUI>("Subheader").text = description;
            //modEntry.FindComponent<TextMeshProUGUI>("Subheader").lineSpacing = 5;
        }
    }

    private void ShowModList()
    {
        SetHeaderText("Mods");
        ReplaceWithModList(true);

        PanScreenDown();
        ModMenuOpen = true;
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
