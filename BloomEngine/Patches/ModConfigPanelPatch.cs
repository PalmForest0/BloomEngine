using BloomEngine.Menu;
using BloomEngine.Menu.Config;
using BloomEngine.Utilities;
using HarmonyLib;
using Il2CppReloaded.Input;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Patches;

[HarmonyPatch]
internal class ModConfigPanelPatch
{
    private static ModEntry currentEntry;
    private static PanelView configPanel;
    private static Transform windowContainer;

    private static Dictionary<ModEntry, GameObject[]> inputs = new Dictionary<ModEntry, GameObject[]>();

    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    private static void PanelContainerAwake(PanelViewContainer __instance)
    {
        if (__instance.name != "FrontendPanels")
            return;

        var quitPanel = __instance.m_panels.FirstOrDefault(p => p.m_id == "quit");

        // Clone the quit panel to use as a base for the entry config panel
        if (quitPanel is not null)
        {
            var configPanel = GameObject.Instantiate(quitPanel, __instance.transform);
            CreateModConfigPanel(configPanel);
        }
    }

    /// <summary>
    /// Sets up a new panel to contain entry configuration options.
    /// </summary>
    private static void CreateModConfigPanel(PanelView panel)
    {
        panel.m_id = "modConfig";
        panel.name = "P_ModConfig";
        configPanel = panel;

        // Destroy pesky localizers
        foreach (var local in panel.GetComponentsInChildren<TextLocalizer>())
            GameObject.Destroy(local);

        windowContainer = panel.transform.Find("Canvas/Layout/Center/Window");
        var buttons = panel.transform.Find("Canvas/Layout/Center/Window").FindComponents<UnityEngine.UI.Button>("Buttons");

        var applyBtn = buttons.FirstOrDefault(btn => btn.name == "P_BacicButton_Quit");
        if (applyBtn is not null)
        {
            // Update name and text
            applyBtn.name = "P_ConfigButton_Apply";
            applyBtn.GetComponentInChildren<TextMeshProUGUI>().SetText("Apply");

            // Remove garbage components
            GameObject.Destroy(applyBtn.GetComponent<Il2CppReloaded.ExitGame>());
            GameObject.Destroy(applyBtn.GetComponent<TextLocalizer>());

            // Update onClick event
            applyBtn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            applyBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)(() => { CloseConfigPanel(true); }));
        }

        // TODO: Setup Cancel button similarly to Apply button
        var cancelBtn = buttons.FirstOrDefault(btn => btn.name == "P_BacicButton_Cancel");
        if (cancelBtn is not null)
        {
            cancelBtn.name = "P_ConfigButton_Cancel";
            cancelBtn.GetComponentInChildren<TextMeshProUGUI>().SetText("Cancel");
            GameObject.Destroy(cancelBtn.GetComponent<TextLocalizer>());

            cancelBtn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            cancelBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)(() => { CloseConfigPanel(false); }));
        }
    }

    /// <summary>
    /// Closes the mod configuration panel and updates any changed config properties by calling their setters if needed.
    /// </summary>
    public static void CloseConfigPanel(bool applyChanges)
    {
        if (applyChanges)
            ModMenu.Log($"Applying config for mod: {currentEntry.DisplayName}");

        for (int i = 0; i < inputs[currentEntry].Length; i++)
        {
            var input = inputs[currentEntry][i];
            var property = currentEntry.Config.Properties[i];

            if (applyChanges)
            {
                string text = input.GetComponent<ReloadedInputField>().text;
                dynamic value = null;

                if (string.IsNullOrWhiteSpace(text))
                    value = null;
                else if (property.ValueType == typeof(string) || property.ValueType == typeof(int))
                    value = Convert.ChangeType(text, property.ValueType);
                // TODO: Handle other types

                property.SetValue(value);
            }

            GameObject.Destroy(input.transform.parent.Find($"PropertyLabel_{property.Name}").gameObject);
            GameObject.Destroy(input);
        }

        currentEntry = null;
        configPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the mod configuration panel for the specified entry and creates inputs for the config properties.
    /// </summary>
    public static void OpenConfigPanel(ModEntry entry)
    {
        // TODO: Create inputs for each config property in entry.Properties
        ModMenu.Log($"Editing config for mod: {entry.DisplayName}");

        windowContainer.Find("HeaderText").GetComponent<TextMeshProUGUI>().text = $"{entry.DisplayName}";
        windowContainer.Find("SubheadingText").GetComponent<TextMeshProUGUI>().text = " ";

        currentEntry = entry;
        configPanel.gameObject.SetActive(true);

        // Create inputs for each property
        var properties = entry.Config.Properties;
        inputs[entry] = new GameObject[properties.Count];

        var parentRect = windowContainer.GetComponent<RectTransform>();
        for (int i = 0; i < properties.Count; i++)
            inputs[entry][i] = CreateInput(properties[i], parentRect);
    }

    private static GameObject CreateInput(IConfigProperty property, RectTransform parent)
    {
        GameObject textObj = GameObject.Instantiate(windowContainer.Find("SubheadingText").gameObject);
        textObj.name = $"PropertyLabel_{property.Name}";
        textObj.GetComponent<RectTransform>().SetParent(parent);
        textObj.GetComponent<TextMeshProUGUI>().text = property.Name;
        textObj.GetComponent<TextMeshProUGUI>().fontSize = 50;
        textObj.SetActive(true);

        GameObject inputObj = null;
        string name = $"PropertyInput_{property.Name}";
        string typeName = property.ValueType.Name;

        // Basic string input
        if (property.ValueType == typeof(string))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName);
            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }
        // Sanitised numeric input
        else if (TypeHelper.IsNumericType(property.ValueType))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onTextChanged: field =>
            {
                string sanitised = TextHelper.SanitiseNumericInput(field.m_Text);
                field.m_Text = sanitised;
            }, onDeselect: field =>
            {
                object converted = TextHelper.ValidateNumericInput(field.m_Text, property.ValueType);
                field.text = converted.ToString();
                property.SetValue(converted);
            });

            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }

        return inputObj;
    }
}