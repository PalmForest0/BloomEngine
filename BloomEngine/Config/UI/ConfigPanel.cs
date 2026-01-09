using BloomEngine.Config.Inputs.Base;
using BloomEngine.Config.Services;
using BloomEngine.ModMenu.Services;
using BloomEngine.Modules;
using BloomEngine.Utilities;
using BloomEngine.Extensions;
using Il2CppReloaded.Input;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomEngine.Config.UI;

internal sealed class ConfigPanel
{
    public ModMenuEntry Mod { get; set; }

    private readonly List<RectTransform> pageObjects = new List<RectTransform>();

    private readonly RectTransform window;
    private readonly GameObject panel;

    // Page navigation
    public int PageCount { get; private init; }
    public int PageIndex { get; private set; } = 0;

    const int InputFieldsPerPage = 7;

    private RectTransform pageControlsRect;
    private GameObject pageCountLabel;
    private GameObject pageBackButton;
    private GameObject pageNextButton;

    private static Sprite resetButtonSprite = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.ResetButton.png");
    private static Sprite resetButtonSpriteSelected = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.ResetButtonSelected.png");
    private static Sprite infoButtonSprite = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.InfoButton.png");
    private static Sprite infoButtonSpriteSelected = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.InfoButtonSelected.png");

    private static CustomPopup configPopup;

    internal ConfigPanel(PanelView panel, ModMenuEntry mod)
    {
        Mod = mod;
        PageCount = (int)Math.Ceiling((double)mod.Config.InputCount / InputFieldsPerPage);

        this.panel = panel.gameObject;
        panel.m_id = $"modConfig_{mod.Mod.Info.Name}";
        panel.gameObject.name = $"P_ModConfig_{mod.DisplayName.Trim().Replace(" ", "")}";
        window = panel.transform.Find("Canvas/Layout/Center/Window").GetComponent<RectTransform>();

        // Make panel size static if there are multiple pages
        if (PageCount > 1)
        {
            UnityEngine.Object.Destroy(window.GetComponent<ContentSizeFitter>());
            window.sizeDelta = new Vector2(2800, 1900);
            window.anchoredPosition = new Vector2(0, -75);
        }
        else window.sizeDelta = new Vector2(2800, 0);

        // Create popup that will be used to show input descriptions
        configPopup = UIHelper.CreatePopup("configPopup", "P_ConfigPopup");
        configPopup.SetFirstButton(true, "Close");

        // Setup apply and cancel buttons
        UIHelper.ModifyButton(window.Find("Buttons").GetChild(0).gameObject, "P_ConfigButton_Apply", "Apply", () =>
        {
            Mod.Config.UpdateAllFromUI();
            Mod.Config.Save(true);
            ConfigService.HideConfigPanel();
        });

        UIHelper.ModifyButton(window.Find("Buttons").GetChild(1).gameObject, "P_ConfigButton_Cancel", "Cancel", ConfigService.HideConfigPanel);

        var windowLayout = window.GetComponent<VerticalLayoutGroup>();
        windowLayout.childForceExpandHeight = false;

        // Change header text and sizing options
        var header = window.Find("HeaderText").GetComponent<TextMeshProUGUI>();
        header.text = $"{mod.DisplayName} Config";
        header.enableAutoSizing = false;
        
        var headerLayout = header.gameObject.GetComponent<LayoutElement>();
        headerLayout.minHeight = 130f;
        headerLayout.preferredHeight = 130f;
        headerLayout.flexibleHeight = 0;
        header.GetComponent<RectTransform>().sizeDelta = new Vector2(header.GetComponent<RectTransform>().sizeDelta.x, 130f);

        // Create inputs for each input on each page
        CreatePages();
        UnityEngine.Object.Destroy(window.Find("SubheadingText").gameObject);

        // Create page controls if there are multiple pages
        if (PageCount > 1)
            CreatePageControls(window.parent.GetComponent<RectTransform>());

        // Add click blocker background
        UnityEngine.Object.Instantiate(UIHelper.MainMenu.transform.parent.Find("P_UsersPanel/Canvas/P_Scrim").gameObject, window.parent).transform.SetAsFirstSibling();

        // Destroy all localisers
        foreach (var localiser in panel.GetComponentsInChildren<TextLocalizer>(true))
            UnityEngine.Object.Destroy(localiser);

        Melon<BloomEngineMod>.Logger.Msg($"Successfully created config panel for {mod.DisplayName} with {mod.Config.InputCount} input fields across {PageCount} page{(PageCount > 1 ? "s" : "")}.");
    }

    private void CreatePages()
    {
        var pages = Mod.Config.ConfigInputs.Chunk(InputFieldsPerPage).ToList();

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

            pageObjects.Add(pageRect);
        }
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
        CreateSquareButton("InputResetButton", rowRect, input.ResetValueUI, resetButtonSprite, resetButtonSpriteSelected);
        if(!string.IsNullOrWhiteSpace(input.Description))
            CreateSquareButton("InputInfoButton", rowRect, () => configPopup.ShowWithText(input.Name, input.Description), infoButtonSprite, infoButtonSpriteSelected);
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

    private static void CreateSquareButton(string name, RectTransform parent, Action onClick, Sprite normalSprite, Sprite hoverSprite = null)
    {
        // Create the button using a wrapper and destroy the garbage
        RectTransform wrapper = UIHelper.CreateUIWrapper(parent, name);
        GameObject buttonObj = UIHelper.CreateButton("Button_Internal", wrapper, "", onClick!);
        GameObject.Destroy(buttonObj.transform.Find("Label").gameObject);
        GameObject.Destroy(buttonObj.transform.Find("Background/ImageSelected").gameObject);

        UIHelper.SetParentAndStretch(buttonObj.GetComponent<RectTransform>(), wrapper);

        // Modify and cleanup the image component
        Image buttonImg = buttonObj.FindComponent<Image>("Background/Image");
        buttonImg.type = Image.Type.Simple;
        buttonImg.sprite = normalSprite;
        buttonImg.preserveAspect = true;

        // Make the sprite change on hover if needed
        if(hoverSprite)
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

        // Create previous page button
        pageBackButton = UnityEngine.Object.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Back").gameObject, pageControlsRect);
        UnityEngine.Object.Destroy(pageBackButton.GetComponent<NavigationCheck>());
        pageBackButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 200);
        var backButton = pageBackButton.GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => SetPageIndex(PageIndex - 1));

        // Create page count label
        pageCountLabel = UnityEngine.Object.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/PageCount").gameObject, pageControlsRect);

        // Create next page button
        pageNextButton = UnityEngine.Object.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Next").gameObject, pageControlsRect);
        UnityEngine.Object.Destroy(pageNextButton.GetComponent<NavigationCheck>());
        pageNextButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 200);
        var nextButton = pageNextButton.GetComponent<Button>();
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => SetPageIndex(PageIndex + 1));

        SetPageIndex(0);
    }


    /// <summary>
    /// Displays the panel and populates its input fields with the current values of the associated properties.
    /// </summary>
    public void ShowPanel()
    {
        // Populate inputs with currently stored values
        Mod.Config.RefreshAllUI();

        SetPageIndex(0);
        panel.SetActive(true);

        if(PageCount > 1)
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
        configPopup.Hide();
        panel.SetActive(false);
    }

    /// <summary>
    /// Sets the current page index, updating the displayed page and related UI elements accordingly.
    /// </summary>
    /// <param name="index">The index of the page to display.</param>
    public void SetPageIndex(int index)
    {
        if (PageCount == 1)
            return;

        // Clamp and update page index and label
        PageIndex = Mathf.Clamp(index, 0, PageCount - 1);
        pageCountLabel.transform.FindChild("Count").GetComponent<TextMeshProUGUI>().text = $"{PageIndex + 1}/{PageCount}";

        // Active the correct page
        for (int i = 0; i < pageObjects.Count; i++)
            pageObjects[i].gameObject.SetActive(i == index);

        // Update button interactability
        pageBackButton.GetComponent<Button>().interactable = PageIndex > 0;
        pageNextButton.GetComponent<Button>().interactable = PageIndex < PageCount - 1;
    }
}