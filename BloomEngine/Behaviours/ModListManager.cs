using BloomEngine.Utilities;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Behaviours;

public class ModListManager : MonoBehaviour
{
    public bool ModMenuOpen { get; private set; } = false;

    private Transform achievementsMenu;
    private UnityEngine.UI.Button achievementsButton;
    private AchievementsUI achievementsUI;

    public void Start()
    {
        achievementsUI = transform.GetComponentInParent<AchievementsUI>();

        achievementsMenu = transform.parent.Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");
        achievementsButton = transform.parent.parent.Find("Main").Find("BG_Tree").Find("AchievementsButton").GetComponent<UnityEngine.UI.Button>();

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

        achievementsButton.onClick.AddListener((UnityAction)ShowAchievementList);
        achievementsUI.m_backButton.onClick.AddListener((UnityAction)(() => ModMenuOpen = false));
    }

    private void CreateModsEntries()
    {
        GameObject prefab = transform.parent.parent.Find("Achievements").Find("AchievementItem").gameObject;

        foreach (var mod in MelonMod.RegisteredMelons)
        {
            GameObject modEntry = GameObject.Instantiate(prefab, achievementsMenu);
            modEntry.SetActive(true);
            modEntry.name = $"ModEntry_{mod.Info.Name}";

            modEntry.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = mod.Info.Name;
            modEntry.transform.Find("Subheader").GetComponent<TextMeshProUGUI>().text = $"{mod.Info.Author}\n{mod.Info.Version}";
            modEntry.transform.Find("Subheader").GetComponent<TextMeshProUGUI>().lineSpacing = 5;
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
        achievementsMenu.parent.Find("Header").Find("Center").Find("HeaderRock").GetComponent<TextMeshProUGUI>().text = text;
        achievementsMenu.parent.Find("Header").Find("Center").Find("HeaderRockTop").GetComponent<TextMeshProUGUI>().text = text;
        achievementsMenu.parent.Find("Header").Find("Center").Find("HeaderRockBottom").GetComponent<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// Replaces all the achievements with the loaded mods
    /// </summary>
    /// <param name="show">Whether to show the mod list of to return the achievement list</param>
    private void ReplaceWithModList(bool showMods)
    {
        for (int i = 0; i < achievementsMenu.childCount; i++)
        {
            GameObject entry = achievementsMenu.GetChild(i).gameObject;

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
