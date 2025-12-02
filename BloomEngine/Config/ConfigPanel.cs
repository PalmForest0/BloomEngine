using BloomEngine.Menu;
using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomEngine.Config;

public class ConfigPanel
{
    const int PropertiesPerPage = 7;

    public ModEntry Mod { get; set; }

    private readonly List<RectTransform> pages = new List<RectTransform>();
    private readonly Dictionary<IConfigProperty, GameObject> inputFields = new Dictionary<IConfigProperty, GameObject>();

    private readonly PanelView panelView;
    private readonly RectTransform window;

    private RectTransform pageControlsRect;
    private GameObject pageCountLabel;
    private GameObject pageBackButton;
    private GameObject pageNextButton;

    private readonly int totalPageCount = 0;
    private int pageIndex = 0;

    internal ConfigPanel(PanelView panel, ModEntry mod)
    {
        if (mod.Config is null || mod.Config.Properties.IsNullOrEmpty())
        {
            Melon<BloomEngineMod>.Logger.Warning($"Failed to setup config panel for {mod.DisplayName}: No config or config properties registered.");
            return;
        }

        Mod = mod;

        panelView = panel;
        panelView.m_id = $"modConfig_{mod.Mod.Info.Name}";
        panelView.gameObject.name = $"P_ModConfig_{mod.DisplayName.Replace(" ", "")}";
        window = panelView.transform.Find("Canvas/Layout/Center/Window").GetComponent<RectTransform>();

        // Split properties into pages
        totalPageCount = (int)Math.Ceiling((double)mod.Config.Properties.Count / PropertiesPerPage);

        // Adjust size if there are multiple pages, but change width anyway
        if (totalPageCount > 1)
        {
            GameObject.Destroy(window.GetComponent<ContentSizeFitter>());
            window.sizeDelta = new Vector2(2500, 1900);
            window.anchoredPosition = new Vector2(0, -80);
        }
        else window.sizeDelta = new Vector2(2500, 0);

        // Add background click blocker
        GameObject.Instantiate(UIHelper.MainMenu.transform.parent.Find("P_UsersPanel/Canvas/P_Scrim").gameObject, window.parent).transform.SetAsFirstSibling();

        // Setup panel buttons
        var buttons = window.Find("Buttons").GetComponentsInChildren<Button>();
        SetupApplyButton(buttons[0]);
        SetupCancelButton(buttons[1]);

        // Change header text and sizing options
        var header = window.Find("HeaderText").GetComponent<TextMeshProUGUI>();
        header.text = $"{mod.DisplayName} Config";
        header.enableAutoSizing = false;

        var headerRect = header.GetComponent<RectTransform>();
        var headerLayout = header.gameObject.GetComponent<LayoutElement>();
        headerLayout.preferredWidth = headerRect.sizeDelta.x;
        headerLayout.preferredHeight = headerRect.sizeDelta.y;

        GameObject.Destroy(window.Find("SubheadingText").gameObject);

        // Create inputs for each property on each page
        var pagesData = mod.Config.Properties.Chunk(PropertiesPerPage).ToList();
        for (int i = 0; i < pagesData.Count; i++)
        {
            var page = CreateLayout(window, $"LayoutPage_{i}");
            pages.Add(page);

            RectTransform labelColumn = page.GetChild(0).GetComponent<RectTransform>();
            RectTransform fieldColumn = page.GetChild(1).GetComponent<RectTransform>();

            foreach (var property in pagesData[i])
            {
                CreateInputLabel(property, labelColumn);
                inputFields[property] = CreateInputField(property, fieldColumn);
            }
        }

        if (totalPageCount > 1)
            CreatePageControls(window.parent.GetComponent<RectTransform>());

        // Destroy all localisers
        foreach (var localiser in panelView.GetComponentsInChildren<TextLocalizer>(true))
            GameObject.Destroy(localiser);

        Melon<BloomEngineMod>.Logger.Msg($"Successfully created config panel for {mod.DisplayName} with {mod.Config.Properties.Count} properties across {totalPageCount} page{(totalPageCount > 1 ? "s" : "")}.");
    }

    /// <summary>
    /// Displays the panel and populates its input fields with the current values of the associated properties.
    /// </summary>
    public void ShowPanel()
    {
        // Populate input fields with current property values
        foreach (var input in inputFields)
        {
            var field = input.Value.GetComponent<ReloadedInputField>();
            var property = input.Key;

            if (property is not null)
                field.text = property.GetValue().ToString();
        }

        SetPageIndex(0);
        panelView.gameObject.SetActive(true);
        
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(pageControlsRect);
    }

    public void HidePanel()
    {
        panelView.gameObject.SetActive(false);
    }

    private void SetupApplyButton(Button button)
    {
        // Update name and text
        button.name = "P_ConfigButton_Apply";
        button.GetComponentInChildren<TextMeshProUGUI>().SetText("Apply");

        // Remove garbage components
        GameObject.Destroy(button.GetComponent<Il2CppReloaded.ExitGame>());
        GameObject.Destroy(button.GetComponent<TextLocalizer>());

        // Apply all input fields on click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener((UnityAction)(() =>
        {
            ModMenu.Log($"Updating all config properties of {Mod.DisplayName}");

            foreach (var (property, field) in inputFields)
                property.ApplyFromInputField(field.GetComponent<ReloadedInputField>());

            ModMenu.HideConfigPanel();
        }));
    }

    private static void SetupCancelButton(Button button)
    {
        // Update name and text
        button.name = "P_ConfigButton_Cancel";
        button.GetComponentInChildren<TextMeshProUGUI>().SetText("Cancel");

        // Remove garbage components
        UnityEngine.Object.Destroy(button.GetComponent<TextLocalizer>());

        // Hide config panel on click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener((UnityAction)ModMenu.HideConfigPanel);
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
        pageBackButton = GameObject.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Back").gameObject, pageControlsRect);
        GameObject.Destroy(pageBackButton.GetComponent<NavigationCheck>());
        pageBackButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 200);
        var backButton = pageBackButton.GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener((UnityAction)(() => SetPageIndex(pageIndex - 1)));

        // Create page count label
        pageCountLabel = GameObject.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/PageCount").gameObject, pageControlsRect);

        // Create next page button
        pageNextButton = GameObject.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Next").gameObject, pageControlsRect);
        GameObject.Destroy(pageNextButton.GetComponent<NavigationCheck>());
        pageNextButton.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 200);
        var nextButton = pageNextButton.GetComponent<Button>();
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener((UnityAction)(() => SetPageIndex(pageIndex + 1)));

        SetPageIndex(0);
    }

    private void SetPageIndex(int index)
    {
        if (totalPageCount == 1)
            return;

        // Clamp and update page index and label
        pageIndex = Mathf.Clamp(index, 0, totalPageCount - 1);
        pageCountLabel.transform.FindChild("Count").GetComponent<TextMeshProUGUI>().text = $"{pageIndex + 1}/{totalPageCount}";

        // Active the correct page
        for (int i = 0; i < pages.Count; i++)
            pages[i].gameObject.SetActive(i == index);

        // Update button interactability
        pageBackButton.GetComponent<Button>().interactable = pageIndex > 0;
        pageNextButton.GetComponent<Button>().interactable = pageIndex < totalPageCount - 1;
    }

    private RectTransform CreateLayout(RectTransform parent, string name)
    {
        GameObject layoutObj = new GameObject(name);
        var layoutTransform = layoutObj.AddComponent<RectTransform>();
        layoutTransform.SetParent(parent, false);
        layoutTransform.anchorMin = new Vector2(0, 1);
        layoutTransform.anchorMax = new Vector2(1, 1);
        layoutTransform.pivot = new Vector2(0.5f, 1);
        layoutTransform.offsetMin = Vector2.zero;
        layoutTransform.offsetMax = Vector2.zero;
        var layoutGroup = layoutObj.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;

        CreateColumn(layoutTransform, "LabelColumn");
        CreateColumn(layoutTransform, "FieldColumn");

        return layoutTransform;
    }

    private void CreateColumn(RectTransform parent, string name)
    {
        GameObject column = new GameObject(name);
        var rect = column.AddComponent<RectTransform>();
        rect.SetParent(parent, false);

        VerticalLayoutGroup layout = column.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        
        if(totalPageCount > 1)
        {
            var element = column.AddComponent<LayoutElement>();
            element.preferredHeight = 2100;
            element.preferredWidth = 998;
        }
    }

    private void CreateInputLabel(IConfigProperty property, RectTransform parent)
    {
        GameObject obj = UnityEngine.Object.Instantiate(window.Find("SubheadingText").gameObject, parent);
        obj.name = $"PropertyLabel_{property.Name}";
        obj.SetActive(true);

        var layout = obj.AddComponent<LayoutElement>();
        layout.preferredHeight = 134;

        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = property.Name;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.Left;
        text.enabled = true;
    }

    private static GameObject CreateInputField(IConfigProperty property, RectTransform parent)
    {
        GameObject inputObj = null;
        string name = $"PropertyInput_{property.Name}";
        string typeName = property.ValueType.Name;
        
        // Sanitised numeric input
        if (TypeHelper.IsNumericType(property.ValueType))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onTextChanged: field =>
            {
                string sanitised = TextHelper.SanitiseNumericInput(field.m_Text);
                field.m_Text = sanitised;
            }, onDeselect: property.ApplyFromInputField);

            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }
        // Basic string input is the default fallback
        else
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onDeselect: property.ApplyFromInputField);
            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }

        var layout = inputObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 134;

        return inputObj;
    }
}