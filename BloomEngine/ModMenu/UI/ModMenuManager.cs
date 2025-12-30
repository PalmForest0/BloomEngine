using BloomEngine.Config.Services;
using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomEngine.ModMenu.UI;

internal class ModMenuManager : MonoBehaviour
{
    public bool ModMenuOpen { get; private set; } = false;

    private GameObject achievementsContainer;
    private GameObject modsContainer;
    private GameObject header;

    private GameObject bloomEngineLabel;

    private AchievementsUI achievementsUi;

    private static Sprite configIconSprite = AssetHelper.LoadSprite("BloomEngine.Resources.ConfigIcon.png");
    private static Sprite defaultIconSprite = AssetHelper.LoadSprite("BloomEngine.Resources.DefaultModIcon.png");
    private static Sprite modIconBorderSprite = AssetHelper.LoadSprite("BloomEngine.Resources.ModIconBorder.png");

    private const int ModIconSize = 225;
    private const int ModIconBorderSize = 275;


    public void Start()
    {
        // Find the achievements container and AchievementsUI component
        achievementsContainer = transform.Find("ScrollView/Viewport/Content/Achievements").gameObject;
        achievementsUi = transform.GetComponent<AchievementsUI>();

        // Prevent header from blocking clicks on mod entries
        header = transform.Find("ScrollView/Viewport/Content/Header").gameObject;
        header.transform.Find("Shadow").GetComponent<Image>().raycastTarget = false;
        header.transform.Find("Left/Background_grass02").GetComponent<Image>().raycastTarget = false;

        CreateButtons();
        CreateBloomEngineLabel();
        CreateModsContainer();
        CreateEntries();

        Melon<BloomEngineMod>.Logger.Msg("Successfully initialized mod menu and created all mod entries.");
    }


    private void CreateModsContainer()
    {
        modsContainer = Instantiate(achievementsContainer, achievementsContainer.transform.parent);
        modsContainer.name = "ModEntries";

        RectTransform modsContainerRect = modsContainer.GetComponent<RectTransform>();
        modsContainerRect.sizeDelta = new Vector2(2800, modsContainerRect.sizeDelta.y);
        modsContainerRect.anchoredPosition = new Vector2(70, -1020);

        GridLayoutGroup modsContainerGrid = modsContainer.GetComponent<GridLayoutGroup>();
        modsContainerGrid.childAlignment = TextAnchor.UpperCenter;
        modsContainerGrid.cellSize = new Vector2(1100, 250);
        modsContainerGrid.spacing = new Vector2(150, 100);

        for (int i = 0; i < modsContainer.transform.childCount; i++)
            Destroy(modsContainer.transform.GetChild(i).gameObject);
    }

    private void CreateButtons()
    {
        GameObject obj = UIHelper.CreateButton("ModsButton", transform, "Mods", OpenModMenu);

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 100);

        // Update the achievements button to reset the text and hide the mod entries
        Button achievementsButton = transform.parent.FindComponent<Button>("Main/BG_Tree/AchievementsButton");
        achievementsButton.onClick.AddListener(() => SetCurrentMenu(showModMenu: false));
        achievementsUi.m_backButton.onClick.AddListener(() => ModMenuOpen = false);
    }

    private void CreateBloomEngineLabel()
    {
        bloomEngineLabel = new GameObject("BloomEngineLabel");
        bloomEngineLabel.transform.SetParent(transform, false);

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
        {
            // Create a new mod achievement for this mod
            GameObject modObj = Instantiate(transform.Find("AchievementItem").gameObject, modsContainer.transform);
            modObj.SetActive(true);
            modObj.name = $"ModEntry_{mod.Info.Name}";

            // Get this mod's mod menu entry if it has one
            bool isRegistered = ModMenuService.Entries.TryGetValue(mod, out ModMenuEntry entry);

            var title = modObj.FindComponent<TextMeshProUGUI>("Title");
            var subheader = modObj.FindComponent<TextMeshProUGUI>("Subheader");

            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, 15);
            subheader.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, -67);
            subheader.maxVisibleLines = 4;

            // Update the icon's pivot and size
            GameObject modIconObj = modObj.transform.Find("Icon").gameObject;
            RectTransform modIconRect = modIconObj.GetComponent<RectTransform>();
            modIconRect.pivot = new Vector2(0.5f, 0.5f);
            modIconRect.sizeDelta = new Vector2(ModIconSize, ModIconSize);

            // Set the mod icon to either the mod's custom icon or the default icon
            modIconObj.GetComponent<Image>().sprite = entry is null ? defaultIconSprite : entry.Icon ?? defaultIconSprite;

            // Create an icon container to hold the icon and border ( + config icon)
            GameObject iconContainer = Instantiate(modIconObj, modObj.transform);
            Destroy(iconContainer.GetComponent<Image>());
            iconContainer.name = "IconContainer";
            modIconObj.transform.SetParent(iconContainer.transform);

            RectTransform iconContainerRect = iconContainer.GetComponent<RectTransform>();
            iconContainerRect.pivot = new Vector2(0.2f, 0.5f);

            CreateModIconBorder(modIconObj, iconContainerRect);

            // Sets the name and description of unregistered mods to default values
            if (!isRegistered)
            {
                title.text = ModMenuEntry.GetDefaultModName(mod);
                title.color = new Color(1f, 0.6f, 0.1f, 1f);
                subheader.text = ModMenuEntry.GetDefaultModDescription(mod);
                continue;
            }

            // If this mod is registered, update the display name, description and icon
            title.text = entry.DisplayName;
            subheader.text = entry.Description;

            // If this entry has a config, create a config button
            if (entry.HasConfig)
            {
                RectTransform configIconRect = CreateModConfigIcon(modIconObj, iconContainerRect);
                SetupdModConfigButton(modIconObj, configIconRect, entry);
            }
        }
    }

    private static RectTransform CreateModConfigIcon(GameObject modIcon, RectTransform iconContainer)
    {
        // Create a config icon that appears when you hover over the mod entry
        GameObject configIcon = Instantiate(modIcon, iconContainer);
        configIcon.name = "ConfigIcon";

        RectTransform configIconRect = configIcon.GetComponent<RectTransform>();
        configIconRect.pivot = new Vector2(0.5f, 0.5f);

        configIconRect.sizeDelta = new Vector2(ModIconSize, ModIconSize);
        Image configIconImg = configIcon.GetComponent<Image>();
        configIconImg.sprite = configIconSprite;
        configIconImg.raycastTarget = false;

        configIcon.AddComponent<CanvasGroup>().alpha = 0f;

        return configIconRect;
    }

    private static void SetupdModConfigButton(GameObject modIcon, RectTransform configIconRect, ModMenuEntry modEntry)
    {
        // Add a button component to the icon object
        Button configButton = modIcon.AddComponent<Button>();
        configButton.onClick.AddListener(() => ConfigService.ShowConfigPanel(modEntry));

        // Adjust the icon's hover colors
        var colors = configButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.fadeDuration = 0.1f;
        configButton.colors = colors;

        // Add event triggers for pointer enter and exit to fade in/out the config icon
        EventTrigger trigger = modIcon.AddComponent<EventTrigger>();
        trigger.triggers = new Il2CppSystem.Collections.Generic.List<EventTrigger.Entry>();

        // On pointer enter trigger - fade in config icon
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener(_ => UIHelper.FadeUIAlpha(configIconRect, 1f, 0.2f));
        trigger.triggers.Add(pointerEnter);

        // On pointer exit trigger - fade out config icon
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener(_ => UIHelper.FadeUIAlpha(configIconRect, 0f, 0.2f));
        trigger.triggers.Add(pointerExit);
    }

    private static void CreateModIconBorder(GameObject modIcon, RectTransform iconContainer)
    {
        // Create a border around the mod icon to indicate it has a config
        GameObject iconBorder = Instantiate(modIcon, iconContainer);
        iconBorder.name = "IconBorder";

        RectTransform borderRect = iconBorder.GetComponent<RectTransform>();
        borderRect.pivot = new Vector2(0.5f, 0.5f);
        borderRect.sizeDelta = new Vector2(ModIconBorderSize, ModIconBorderSize);

        Image borderImg = iconBorder.GetComponent<Image>();
        borderImg.sprite = modIconBorderSprite;
        borderImg.raycastTarget = false;
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