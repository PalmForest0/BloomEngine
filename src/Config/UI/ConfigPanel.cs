using BloomEngine.Config.Inputs.Base;
using BloomEngine.UI;
using BloomEngine.Helpers;
using Il2CppReloaded.Input;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BloomEngine.Core;

namespace BloomEngine.Config.UI;

internal sealed class ConfigPanel
{
    private const int InputsPerPage = 7;

    private readonly int pageCount;
    private int pageIndex = 0;

    private readonly ModConfig config;

    private readonly GameObject panel;
    private readonly RectTransform window;
    private readonly List<RectTransform> pages = new();

    // Page controls UI is only created if there are multiple pages
    private RectTransform? pageControlsRect;
    private GameObject? pageCountLabel;
    private GameObject? pageBackButton;
    private GameObject? pageNextButton;

    private static ModdedPopup ConfigPopup = null!;

    private static readonly Sprite ResetButtonSprite            = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.ResetButton.png");
    private static readonly Sprite ResetButtonSpriteSelected    = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.ResetButtonSelected.png");
    private static readonly Sprite InfoButtonSprite             = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.InfoButton.png");
    private static readonly Sprite InfoButtonSpriteSelected     = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.InfoButtonSelected.png");

    internal ConfigPanel(PanelView panel, ModConfig config)
    {
        this.config = config;
        pageCount = (int)Math.Ceiling((double)config.InputCount / InputsPerPage);

        this.panel = panel.gameObject;
        window = InitializePanel(panel);

        // Create popup that will be used to show input descriptions
        ConfigPopup = UIHelper.CreatePopup("configPopup", "P_ConfigPopup");
        ConfigPopup.SetFirstButton(true, "Close");

        SetupHeader();
        SetupButtons();
        SetupPages();
        
        // Create page controls if there are multiple pages
        if (pageCount > 1)
            CreatePageControls(window.parent.GetComponent<RectTransform>());

        // Add click blocker background
        if (UIHelper.MainMenuPanel)
        {
            var clickBlockerTemplate = UIHelper.MainMenuPanel!.transform.parent.Find("P_UsersPanel/Canvas/P_Scrim").gameObject;
            GameObject.Instantiate(clickBlockerTemplate, window.parent).transform.SetAsFirstSibling();
        }
        else BloomLogger.Error($"Cannot create config panel \"{config.Id}\" due to the MainMenuPanel being null.", ConfigService.LOG_PREFIX);

        // Destroy all localizers
        foreach (var localiser in panel.GetComponentsInChildren<TextLocalizer>(true))
            UnityEngine.Object.Destroy(localiser);

        Melon<BloomEngineMod>.Logger.Msg($"Successfully created {config.DisplayName} config panel with {config.InputCount} fields across {pageCount} page{(pageCount > 1 ? "s" : "")}.");
    }

    private RectTransform InitializePanel(PanelView panel)
    {
        panel.m_id = $"modConfig_{config.Id}";
        panel.gameObject.name = $"P_ModConfig_{config.Id}";

        var window = panel.transform.Find("Canvas/Layout/Center/Window").GetComponent<RectTransform>();

        // Make panel size static if there are multiple pages
        if (pageCount > 1)
        {
            UnityEngine.Object.Destroy(window.GetComponent<ContentSizeFitter>());
            window.sizeDelta = new Vector2(2800, 1900);
            window.anchoredPosition = new Vector2(0, -75);
        }
        else window.sizeDelta = new Vector2(2800, 0);

        window.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;

        return window;
    }

    private void SetupButtons()
    {
        // Setup apply and cancel buttons
        UIHelper.ModifyButton(window.Find("Buttons").GetChild(0).gameObject, "P_ConfigButton_Apply", "Apply", () =>
        {
            config.UpdateAllFromUI();
            config.Save(true);
            ConfigService.HideConfigPanel();
        });

        UIHelper.ModifyButton(window.Find("Buttons").GetChild(1).gameObject, "P_ConfigButton_Cancel", "Cancel", ConfigService.HideConfigPanel);
    }

    private void SetupHeader()
    {
        // Change header text and sizing options
        var header = window.Find("HeaderText").GetComponent<TextMeshProUGUI>();
        header.text = $"{config.DisplayName} Config";
        header.enableAutoSizing = false;

        var headerLayout = header.gameObject.GetComponent<LayoutElement>();
        headerLayout.minHeight = 130f;
        headerLayout.preferredHeight = 130f;
        headerLayout.flexibleHeight = 0;
        header.GetComponent<RectTransform>().sizeDelta = new Vector2(header.GetComponent<RectTransform>().sizeDelta.x, 130f);
    }

    private void SetupPages()
    {
        var pages = config.ConfigInputs.Chunk(InputsPerPage).ToList();

        for (int i = 0; i < pages.Count; i++)
        {
            // Create layout for this page
            GameObject pageObj = new GameObject($"ConfigPage_{i}");
            var pageRect = pageObj.AddComponent<RectTransform>();
            pageRect.SetParent(window, false);
            pageRect.anchorMin = new Vector2(0, 1);
            pageRect.anchorMax = new Vector2(1, 1);
            pageRect.pivot = new Vector2(0.5f, 1);
            pageRect.offsetMin = Vector2.zero;
            pageRect.offsetMax = Vector2.zero;

            var pageLayout = pageObj.AddComponent<VerticalLayoutGroup>();
            pageLayout.spacing = 10;
            pageLayout.childControlWidth = true;

            var fitter = pageObj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (BaseConfigInput input in pages[i])
                CreateRow(input, pageRect);

            this.pages.Add(pageRect);
        }

        // Destroy the label used as a template
        UnityEngine.Object.Destroy(window.Find("SubheadingText").gameObject);
    }

    private void CreateRow(BaseConfigInput input, RectTransform parent)
    {
        // Create row GameObject
        GameObject rowObj = new GameObject($"ConfigRow_{input.Name.Trim().Replace(" ", "")}");
        RectTransform rowRect = rowObj.AddComponent<RectTransform>();
        rowRect.SetParent(parent, false);

        // Add a HorizontalLayoutGroup to the row to position elements
        HorizontalLayoutGroup rowGroup = rowObj.AddComponent<HorizontalLayoutGroup>();
        rowGroup.childAlignment = TextAnchor.MiddleLeft;
        rowGroup.childControlWidth = true;
        rowGroup.childControlHeight = false;
        rowGroup.childForceExpandWidth = false;
        rowGroup.childForceExpandHeight = false;
        rowGroup.spacing = 25;

        // Create LayoutElement to fixate height
        var layout = rowObj.AddComponent<LayoutElement>();
        layout.minHeight = 134;
        layout.preferredHeight = 134;
        layout.flexibleHeight = 0;

        // Create all the children in the right order
        CreateLabel(input, rowRect);
        CreateInput(input, rowRect);
        CreateSquareButton("InputResetButton", rowRect, input.ResetValueUI, ResetButtonSprite, ResetButtonSpriteSelected);
        if (!string.IsNullOrWhiteSpace(input.Description))
            CreateSquareButton("InputInfoButton", rowRect, () => ConfigPopup.ShowWithText(input.Name, input.Description), InfoButtonSprite, InfoButtonSpriteSelected);
    }

    private void CreateLabel(BaseConfigInput input, RectTransform parent)
    {
        GameObject labelObj = UnityEngine.Object.Instantiate(window.Find("SubheadingText").gameObject, parent);
        labelObj.name = $"Label_{input.Name.Trim().Replace(" ", "")}";
        labelObj.SetActive(true);

        LayoutElement layout = labelObj.AddComponent<LayoutElement>();
        layout.minWidth = 900;
        layout.preferredWidth = 900;
        layout.flexibleWidth = 0;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(900, 134);

        var text = labelObj.GetComponent<TextMeshProUGUI>();
        text.text = input.Name;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.Left;
        text.enabled = true;
    }

    private static void CreateInput(BaseConfigInput input, RectTransform parent)
    {
        GameObject inputObj = input.CreateInputObject(parent);
        LayoutElement layout = inputObj.AddComponent<LayoutElement>();
        layout.minWidth = 1200;
        layout.preferredWidth = 1200;
        layout.flexibleWidth = 0;

        layout.minHeight = 134;
        layout.preferredHeight = 134;
        layout.flexibleHeight = 0;
    }

    private static void CreateSquareButton(string name, RectTransform parent, Action onClick, Sprite normalSprite, Sprite? hoverSprite = null)
    {
        // Create the button using a wrapper and destroy the garbage
        RectTransform wrapper = UIHelper.CreateUIWrapper(parent, name);
        GameObject buttonObj = UIHelper.CreateButton("Button_Internal", wrapper, "", onClick);
        GameObject.Destroy(buttonObj.transform.Find("Label").gameObject);
        GameObject.Destroy(buttonObj.transform.Find("Background/ImageSelected").gameObject);

        UIHelper.SetParentAndStretch(buttonObj.GetComponent<RectTransform>(), wrapper);

        // Modify and cleanup the image component
        Image? buttonImg = buttonObj.FindComponent<Image>("Background/Image");
        buttonImg!.type = Image.Type.Simple;
        buttonImg.sprite = normalSprite;
        buttonImg.preserveAspect = true;

        // Make the sprite change on hover if needed
        if (hoverSprite)
        {
            UIHelper.AddEventTrigger(buttonObj, EventTriggerType.PointerEnter, _ => buttonImg.sprite = hoverSprite);
            UIHelper.AddEventTrigger(buttonObj, EventTriggerType.PointerExit, _ => buttonImg.sprite = normalSprite);
        }

        LayoutElement buttonLayout = wrapper.gameObject.AddComponent<LayoutElement>();
        buttonLayout.preferredWidth = 105;
        buttonLayout.preferredHeight = 105;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchoredPosition += new Vector2(0, 12);
    }

    private void CreatePageControls(RectTransform parent)
    {
        var pageControls = new GameObject("PageControls");
        pageControlsRect = pageControls.AddComponent<RectTransform>();
        pageControlsRect.SetParent(parent);

        pageControlsRect.anchorMin = new Vector2(0.5f, 0);
        pageControlsRect.anchorMax = new Vector2(0.5f, 0);
        pageControlsRect.pivot = new Vector2(0.5f, 0.5f);
        pageControlsRect.anchoredPosition = new Vector2(-420, 510);

        var horizontalLayout = pageControls.AddComponent<HorizontalLayoutGroup>();
        horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
        horizontalLayout.spacing = 100;
        horizontalLayout.childControlHeight = false;
        horizontalLayout.childControlWidth = false;
        horizontalLayout.childForceExpandWidth = false;
        horizontalLayout.childForceExpandHeight = false;

        if(!UIHelper.MainMenuPanel)
        {
            BloomLogger.Error($"Cannot create page controls for config panel \"{config.Id}\" due to the MainMenuPanel being null.", ConfigService.LOG_PREFIX);
            return;
        }

        // Create previous page button
        pageBackButton = UnityEngine.Object.Instantiate(UIHelper.MainMenuPanel!.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Back").gameObject, pageControlsRect);
        UnityEngine.Object.Destroy(pageBackButton.GetComponent<NavigationCheck>());
        pageBackButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 200);
        var backButton = pageBackButton.GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => SetPageIndex(pageIndex - 1));

        // Create page count label
        pageCountLabel = UnityEngine.Object.Instantiate(UIHelper.MainMenuPanel.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/PageCount").gameObject, pageControlsRect);

        // Create next page button
        pageNextButton = UnityEngine.Object.Instantiate(UIHelper.MainMenuPanel.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Next").gameObject, pageControlsRect);
        UnityEngine.Object.Destroy(pageNextButton.GetComponent<NavigationCheck>());
        pageNextButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 200);
        var nextButton = pageNextButton.GetComponent<Button>();
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => SetPageIndex(pageIndex + 1));

        SetPageIndex(0);
    }


    /// <summary>
    /// Displays the panel and populates its input fields with the current values of the associated properties.
    /// </summary>
    public void ShowPanel()
    {
        // Populate inputs with currently stored values
        config.RefreshAllUI();

        SetPageIndex(0);
        panel.SetActive(true);

        if (pageCount > 1)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(pageControlsRect);
        }
    }

    /// <summary>
    /// Hides this panel.
    /// </summary>
    public void HidePanel()
    {
        ConfigPopup.Hide();
        panel.SetActive(false);
    }

    /// <summary>
    /// Sets the current page index, updating the displayed page and related UI elements accordingly.
    /// </summary>
    /// <param name="index">The index of the page to display.</param>
    public void SetPageIndex(int index)
    {
        // Page controls exist if there are multiple pages
        if (pageCount == 1)
            return;

        // Clamp and update page index and label
        pageIndex = Mathf.Clamp(index, 0, pageCount - 1);
        pageCountLabel!.transform.FindChild("Count").GetComponent<TextMeshProUGUI>().text = $"{pageIndex + 1}/{pageCount}";

        // Active the correct page
        for (int i = 0; i < pages.Count; i++)
            pages[i].gameObject.SetActive(i == index);

        // Update button interactability
        pageBackButton!.GetComponent<Button>().interactable = pageIndex > 0;
        pageNextButton!.GetComponent<Button>().interactable = pageIndex < pageCount - 1;
    }
}