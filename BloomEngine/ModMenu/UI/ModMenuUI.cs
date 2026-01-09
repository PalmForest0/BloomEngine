using BloomEngine.Utilities;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.ModMenu.UI;

internal sealed class ModMenuUI
{
    public bool ModMenuOpen { get; private set; } = false;

    private readonly GameObject achievementsContainer;
    private GameObject modsContainer;
    private readonly GameObject header;

    private readonly Transform achievements;
    private readonly AchievementsUI achievementsUi;

    private GameObject bloomEngineLabel;

    public ModMenuUI(AchievementsUI achievementsUi)
    {
        // Find the achievements container and AchievementsUI component
        achievementsContainer = achievementsUi.transform.Find("ScrollView/Viewport/Content/Achievements").gameObject;
        
        this.achievementsUi = achievementsUi;
        achievements = achievementsUi.transform;

        // Prevent header from blocking clicks on mod ModEntries
        header = achievements.Find("ScrollView/Viewport/Content/Header").gameObject;
        header.transform.Find("Shadow").GetComponent<Image>().raycastTarget = false;
        header.transform.Find("Left/Background_grass02").GetComponent<Image>().raycastTarget = false;

        CreateButtons();
        CreateBloomEngineLabel();
        CreateModsContainer();
        CreateEntries();
    }


    private void CreateModsContainer()
    {
        modsContainer = GameObject.Instantiate(achievementsContainer, achievementsContainer.transform.parent);
        modsContainer.name = "ModEntries";

        RectTransform modsContainerRect = modsContainer.GetComponent<RectTransform>();
        modsContainerRect.sizeDelta = new Vector2(2800, modsContainerRect.sizeDelta.y);
        modsContainerRect.anchoredPosition = new Vector2(70, -1020);

        GridLayoutGroup modsContainerGrid = modsContainer.GetComponent<GridLayoutGroup>();
        modsContainerGrid.childAlignment = TextAnchor.UpperCenter;
        modsContainerGrid.cellSize = new Vector2(1100, 250);
        modsContainerGrid.spacing = new Vector2(150, 100);

        for (int i = 0; i < modsContainer.transform.childCount; i++)
            GameObject.Destroy(modsContainer.transform.GetChild(i).gameObject);
    }

    private void CreateButtons()
    {
        GameObject obj = UIHelper.CreateButton("ModsButton", achievements, "Mods", OpenModMenu);

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 100);

        // Update the achievements button to reset the text and hide the mod ModEntries
        Button achievementsButton = achievements.parent.FindComponent<Button>("Main/BG_Tree/AchievementsButton");
        achievementsButton.onClick.AddListener(() => SetCurrentMenu(showModMenu: false));
        achievementsUi.m_backButton.onClick.AddListener(() => ModMenuOpen = false);
    }

    private void CreateBloomEngineLabel()
    {
        bloomEngineLabel = new GameObject("BloomEngineLabel");
        bloomEngineLabel.transform.SetParent(achievements, false);

        TextMeshProUGUI label = bloomEngineLabel.AddComponent<TextMeshProUGUI>();
        label.fontSize = 52;
        label.characterSpacing = 0;
        label.font = UIHelper.Font1;
        label.text = $"BloomEngine  v{BloomEngineMod.Version}";

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(600, 50);
        rect.anchoredPosition = new Vector2(40, 40);
    }

    private void CreateEntries()
    {
        foreach (var mod in MelonMod.RegisteredMelons)
            ModMenuItemUI.Create(mod, modsContainer.transform, achievements.Find("AchievementItem").gameObject); 
    }
    

    /// <summary>
    /// Sets the title text of the container screen
    /// </summary>
    private void SetHeaderText(string text)
    {
        header.transform.FindComponent<TextMeshProUGUI>("Center/HeaderRock").text = text;
        header.transform.FindComponent<TextMeshProUGUI>("Center/HeaderRockTop").text = text;
        header.transform.FindComponent<TextMeshProUGUI>("Center/HeaderRockBottom").text = text;
    }

    /// <summary>
    /// Sets the current menu to either the mod menu or achievements menu.
    /// </summary>
    /// <param name="showModMenu">If true, enables the mod menu, otherwise shows the achievements menu.</param>
    private void SetCurrentMenu(bool showModMenu)
    {
        SetHeaderText(showModMenu ? "Mods" : "Achievements");
        bloomEngineLabel.SetActive(showModMenu);

        achievementsContainer.SetActive(!showModMenu);
        modsContainer.SetActive(showModMenu);
    }

    /// <summary>
    /// Opens the mod list and updates the UI.
    /// </summary>
    private void OpenModMenu()
    {
        SetCurrentMenu(showModMenu: true);
        PlayTransitionAnim();
        ModMenuOpen = true;
        achievementsUi.m_achievementsIsActive = true;
    }

    /// <summary>
    /// Triggers the animation that plays when the camera pans down to the achievements screen.
    /// </summary>
    private static void PlayTransitionAnim() => UIHelper.MainMenu.PlayAnimation("A_MainMenu_Achievements_In");
}