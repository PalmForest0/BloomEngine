using BloomEngine.Extensions;
using BloomEngine.UI;
using BloomEngine.Utilities;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.ModMenu.UI;

internal sealed class ModMenuUI
{
    private readonly GameObject achievementsContainer;
    private readonly GameObject modsContainer;

    private readonly GameObject header;
    private readonly TextMeshProUGUI[] headerLabels;

    private readonly RectTransform achievementsRect;
    private readonly AchievementsUI achievementsUI;

    private readonly GameObject bloomLabel;

    public ModMenuUI(AchievementsUI achievementsUi)
    {
        // Find all required AchievementsUI objects
        achievementsUI = achievementsUi;
        achievementsRect = achievementsUi.GetComponent<RectTransform>();
        achievementsContainer = achievementsUi.transform.Find("ScrollView/Viewport/Content/Achievements").gameObject;

        // Prevent header from blocking clicks on mod ModEntries and save labels to be changed later
        header = achievementsRect.Find("ScrollView/Viewport/Content/Header").gameObject;
        header.transform.Find("Shadow").GetComponent<Image>().raycastTarget = false;
        header.transform.Find("Left/Background_grass02").GetComponent<Image>().raycastTarget = false;
        headerLabels = header.transform.Find("Center").GetComponentsInChildren<TextMeshProUGUI>(true);

        modsContainer = CreateModsContainer();
        bloomLabel = CreateBloomLabel();

        CreateButtons();
        CreateEntries();
    }


    /// <summary>
    /// Creates a new container for the mod entries based on the achievements container, and configures its layout.
    /// </summary>
    /// <returns>The created container object.</returns>
    private GameObject CreateModsContainer()
    {
        GameObject container = GameObject.Instantiate(achievementsContainer, achievementsContainer.transform.parent);
        container.name = "ModEntries";

        RectTransform modsContainerRect = container.GetComponent<RectTransform>();
        modsContainerRect.sizeDelta = new Vector2(2800, modsContainerRect.sizeDelta.y);
        modsContainerRect.anchoredPosition = new Vector2(70, -1020);

        GridLayoutGroup modsContainerGrid = container.GetComponent<GridLayoutGroup>();
        modsContainerGrid.childAlignment = TextAnchor.UpperCenter;
        modsContainerGrid.cellSize = new Vector2(1100, 250);
        modsContainerGrid.spacing = new Vector2(150, 100);

        for (int i = 0; i < container.transform.childCount; i++)
            GameObject.Destroy(container.transform.GetChild(i).gameObject);

        return container;
    }

    /// <summary>
    /// Creates the mods button that is placed in the bottom left corner of the main menu.
    /// </summary>
    private void CreateButtons()
    {
        GameObject obj = UIHelper.CreateButton("ModsButton", achievementsRect, "Mods", OpenModMenu);

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 100);

        // Update the achievements button to deactivate the mod menu when clicked
        if (achievementsRect.parent.TryFindComponent<Button>("Main/BG_Tree/AchievementsButton", out var btn, logErrorSource: ModMenuService.Log))
            btn.onClick.AddListener(() => SetModMenuActive(false));
    }

    /// <summary>
    /// Creates a label in the bottom left corner of the mod menu that displays the BloomEngine version.
    /// </summary>
    /// <returns>The created label object.</returns>
    private GameObject CreateBloomLabel()
    {
        var labelObj = new GameObject("BloomEngineLabel");
        labelObj.transform.SetParent(achievementsRect, false);

        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.fontSize = 52;
        label.characterSpacing = 0;
        label.font = UIHelper.Font_BrianneTod;
        label.text =$"""
        {MelonMod.RegisteredMelons.Count} Loaded,  {ModMenuService.RegisteredEntries.Count()} Registered
        BloomEngine  v{BloomEngineMod.Version}
        """;

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(600, 0);
        rect.anchoredPosition = new Vector2(40, 40);

        var fitter = labelObj.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return labelObj;
    }

    /// <summary>
    /// Loops through all registered mods and creates a ModMenuItemUI for each one, adding them to the container.
    /// </summary>
    private void CreateEntries()
    {
        foreach (var mod in MelonMod.RegisteredMelons)
            ModMenuItemUI.Create(mod, modsContainer.transform, achievementsRect.Find("AchievementItem").gameObject); 
    }


    /// <summary>
    /// Sets the header text of the mods/achievements menu to the specified string.
    /// </summary>
    private void SetHeaderText(string text)
    {
        foreach (var label in headerLabels)
            label.text = text;
    }

    /// <summary>
    /// Sets the current menu to either the mod menu or achievements menu.
    /// </summary>
    /// <param name="isActive">If true, enables the mod menu, otherwise shows the achievements menu.</param>
    private void SetModMenuActive(bool isActive)
    {
        SetHeaderText(isActive ? "Mods" : "Achievements");
        modsContainer.SetActive(isActive);
        bloomLabel.SetActive(isActive);

        achievementsContainer.SetActive(!isActive);  
    }

    /// <summary>
    /// Sets the current menu to the mod menu and plays the transition animation.
    /// </summary>
    private void OpenModMenu()
    {
        SetModMenuActive(true);
        PlayTransitionAnim();

        achievementsUI.m_achievementsIsActive = true;
    }

    /// <summary>
    /// Triggers the animation that plays when the camera pans down to the achievements screen.
    /// </summary>
    private static void PlayTransitionAnim() => UIHelper.MainMenuPanel!.PlayAnimation("A_MainMenu_Achievements_In");
}