using BloomEngine.Config.Internal;
using BloomEngine.Config.Services;
using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using Il2CppSource.UI;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Config.UI;

internal class ConfigPanel
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


    internal ConfigPanel(PanelView panel, ModMenuEntry mod)
    {
        Mod = mod;
        PageCount = (int)Math.Ceiling((double)mod.ConfigInputFields.Count / InputFieldsPerPage);

        this.panel = panel.gameObject;
        panel.m_id = $"modConfig_{mod.Mod.Info.Name}";
        panel.gameObject.name = $"P_ModConfig_{mod.DisplayName.Replace(" ", "")}";
        window = panel.transform.Find("Canvas/Layout/Center/Window").GetComponent<RectTransform>();

        // Make panel size static if there are multiple pages
        if (PageCount > 1)
        {
            UnityEngine.Object.Destroy(window.GetComponent<ContentSizeFitter>());
            window.sizeDelta = new Vector2(2500, 1900);
            window.anchoredPosition = new Vector2(0, -80);
        }
        else window.sizeDelta = new Vector2(2500, 0);

        // Setup apply and cancel buttons
        UIHelper.ModifyButton(window.Find("Buttons").GetChild(0).gameObject, "P_ConfigButton_Apply", "Apply", () =>
        {
            foreach (IConfigInput field in Mod.ConfigInputFields)
                field.UpdateFromUI();
            
            ConfigService.HideConfigPanel();
        });

        UIHelper.ModifyButton(window.Find("Buttons").GetChild(1).gameObject, "P_ConfigButton_Cancel", "Cancel", ConfigService.HideConfigPanel);

        // Change header text and sizing options
        var header = window.Find("HeaderText").GetComponent<TextMeshProUGUI>();
        header.text = $"{mod.DisplayName} Config";
        header.enableAutoSizing = false;
        var headerLayout = header.gameObject.GetComponent<LayoutElement>();
        headerLayout.preferredWidth = header.GetComponent<RectTransform>().sizeDelta.x;
        headerLayout.preferredHeight = header.GetComponent<RectTransform>().sizeDelta.y;

        // Create inputs for each field on each page
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

        Melon<BloomEngineMod>.Logger.Msg($"Successfully created config panel for {mod.DisplayName} with {mod.ConfigInputFields.Count} input fields across {PageCount} page{(PageCount > 1 ? "s" : "")}.");
    }

    private void CreatePages()
    {
        var pages = Mod.ConfigInputFields.Chunk(InputFieldsPerPage).ToList();

        for (int i = 0; i < pages.Count; i++)
        {
            // Create layout for this page
            GameObject pageObj = new GameObject($"LayoutPage_{i}");
            var pageRect = pageObj.AddComponent<RectTransform>();
            pageRect.SetParent(window, false);
            pageRect.anchorMin = new Vector2(0, 1);
            pageRect.anchorMax = new Vector2(1, 1);
            pageRect.pivot = new Vector2(0.5f, 1);
            pageRect.offsetMin = Vector2.zero;
            pageRect.offsetMax = Vector2.zero;

            var pageLayout = pageObj.AddComponent<HorizontalLayoutGroup>();
            pageLayout.spacing = 10;
            pageLayout.childControlWidth = true;
            pageLayout.childControlHeight = true;

            // Populate the columns with field labels and input fields
            RectTransform labelColumn = CreateColumn(pageRect, "LabelsColumn");
            RectTransform fieldColumn = CreateColumn(pageRect, "FieldsColumn");

            foreach (IConfigInput field in pages[i])
            {
                CreateLabel(field, labelColumn);
                CreateInput(field, fieldColumn);
            }

            pageObjects.Add(pageRect);
        }
    }

    private RectTransform CreateColumn(RectTransform parent, string name)
    {
        GameObject column = new GameObject(name);
        var columnRect = column.AddComponent<RectTransform>();
        columnRect.SetParent(parent, false);

        VerticalLayoutGroup columnLayout = column.AddComponent<VerticalLayoutGroup>();
        columnLayout.spacing = 10;
        columnLayout.childControlWidth = true;
        columnLayout.childControlHeight = true;
        columnLayout.childForceExpandWidth = true;
        columnLayout.childForceExpandHeight = false;
        
        if(PageCount > 1)
        {
            var element = column.AddComponent<LayoutElement>();
            element.preferredHeight = 2100;
            element.preferredWidth = 998;
        }

        return columnRect;
    }

    private void CreateLabel(IConfigInput field, RectTransform parent)
    {
        GameObject obj = UnityEngine.Object.Instantiate(window.Find("SubheadingText").gameObject, parent);
        obj.name = $"PropertyLabel_{field.Name}";
        obj.SetActive(true);

        var layout = obj.AddComponent<LayoutElement>();
        layout.preferredHeight = 134;

        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = field.Name;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.Left;
        text.enabled = true;
    }

    private static void CreateInput(IConfigInput field, RectTransform parent)
    {
        GameObject inputObj = null;
        string name = $"InputField_{field.Name.Replace(" ", "")}";
        
        // Create the correct input field
        if (field.InputObjectType == typeof(ReloadedInputField))
            inputObj = UIHelper.CreateTextField(name, parent, field.ValueType.Name, onTextChanged: (_) => field.OnUIChanged());
        else if(field.InputObjectType == typeof(Toggle))
            inputObj = UIHelper.CreateCheckbox(name, parent, onValueChanged: (_) => field.OnUIChanged());
        else if (field.InputObjectType == typeof(ReloadedDropdown))
            inputObj = UIHelper.CreateDropdown(name, parent, field.ValueType, onValueChanged: (_) => field.OnUIChanged());
        else if (field.InputObjectType == typeof(Slider) && field is FloatConfigInput sliderInput)
            inputObj = UIHelper.CreateSlider(name, parent, sliderInput.MinValue, sliderInput.MaxValue, sliderInput.MinValue, onValueChanged: (_) => field.OnUIChanged());

        var layout = inputObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 134;
        layout.preferredWidth = 900;

        field.SetInputObject(inputObj);
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
        // Populate input fields with current field values
        foreach (IConfigInput field in Mod.ConfigInputFields)
            field.RefreshUI();

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
    public void HidePanel() => panel.SetActive(false);

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