using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomEngine.Menu.Config;

public class ConfigPanel
{
    public ModEntry Mod { get; set; }

    private Dictionary<IConfigProperty, GameObject> inputFields = new Dictionary<IConfigProperty, GameObject>();

    private PanelView panelView;
    private Transform window;

    private RectTransform labelColumn;
    private RectTransform fieldColumn;

    internal ConfigPanel(PanelView panel, ModEntry mod)
    {
        if (mod.Config is null || mod.Config.Properties.IsNullOrEmpty())
        {
            Melon<BloomEnginePlugin>.Logger.Warning($"Failed to setup config panel for {mod.DisplayName}: No config or config properties registered.");
            return;
        }

        Mod = mod;

        panelView = panel;
        panelView.m_id = $"modConfig_{mod.Id}";
        panelView.gameObject.name = $"P_ModConfig_{mod.DisplayName.Replace(" ", "")}";

        window = panelView.transform.Find("Canvas/Layout/Center/Window");
        window.GetComponent<RectTransform>().sizeDelta = new Vector2(2200, 0);

        // Setup panel buttons
        var buttons = window.Find("Buttons").GetComponentsInChildren<Button>();
        SetupApplyButton(buttons[0]);
        SetupCancelButton(buttons[1]);

        // Change header text
        window.Find("HeaderText").GetComponent<TextMeshProUGUI>().text = $"{mod.DisplayName}";
        window.Find("SubheadingText").GetComponent<TextMeshProUGUI>().text = " ";

        // Create inputs for each property
        CreateLayout(window.GetComponent<RectTransform>());
        foreach (var property in mod.Config.Properties)
        {
            CreateInputLabel(property, labelColumn);
            inputFields[property] = CreateInputField(property, fieldColumn);
        }

        // Destroy all localisers
        foreach (var localiser in panelView.GetComponentsInChildren<TextLocalizer>(true))
            GameObject.Destroy(localiser);
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

        panelView.gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        panelView.gameObject.SetActive(false);
    }


    private void ApplyInputField(ReloadedInputField field, IConfigProperty property)
    {
        // Filter input value
        object value = null;
        if (TypeHelper.IsNumericType(property.ValueType))
            value = TextHelper.ValidateNumericInput(field.m_Text, property.ValueType);
        else if (property.ValueType == typeof(string))
            value = field.m_Text;

        // Transform value
        value = property.TransformValue(value);

        // Validate and apply value
        if (!property.ValidateValue(value))
            value = property.GetValue();
        else property.SetValue(Convert.ChangeType(value, property.ValueType));

        field.text = value.ToString();
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

            foreach (var input in inputFields)
                ApplyInputField(input.Value.GetComponent<ReloadedInputField>(), input.Key);

            ModMenu.HideConfigPanel();
        }));
    }

    private void SetupCancelButton(Button button)
    {
        // Update name and text
        button.name = "P_ConfigButton_Cancel";
        button.GetComponentInChildren<TextMeshProUGUI>().SetText("Cancel");

        // Remove garbage components
        GameObject.Destroy(button.GetComponent<TextLocalizer>());

        // Hide config panel on click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener((UnityAction)ModMenu.HideConfigPanel);
    }

    private void CreateLayout(RectTransform parent)
    {
        GameObject layoutObj = new GameObject("LayoutGroup");
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

        labelColumn = CreateColumn(layoutTransform);
        fieldColumn = CreateColumn(layoutTransform);
    }

    private RectTransform CreateColumn(RectTransform parent)
    {
        GameObject column = new GameObject("Column");
        var rect = column.AddComponent<RectTransform>();
        rect.SetParent(parent, false);

        VerticalLayoutGroup layout = column.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;

        return rect;
    }

    private GameObject CreateInputLabel(IConfigProperty property, RectTransform parent)
    {
        GameObject obj = GameObject.Instantiate(window.Find("SubheadingText").gameObject, parent);
        obj.name = $"PropertyLabel_{property.Name}";
        obj.SetActive(true);

        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = property.Name;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.Left;

        return obj;
    }

    private GameObject CreateInputField(IConfigProperty property, RectTransform parent)
    {
        GameObject inputObj = null;
        string name = $"PropertyInput_{property.Name}";
        string typeName = property.ValueType.Name;

        // Basic string input
        if (property.ValueType == typeof(string))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onDeselect: field => ApplyInputField(field, property));
            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }
        // Sanitised numeric input
        else if (TypeHelper.IsNumericType(property.ValueType))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onTextChanged: field =>
            {
                string sanitised = TextHelper.SanitiseNumericInput(field.m_Text);
                field.m_Text = sanitised;
            }, onDeselect: field => ApplyInputField(field, property));

            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }

        return inputObj;
    }
}