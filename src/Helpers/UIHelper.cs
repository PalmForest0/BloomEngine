using BloomEngine.Extensions;
using BloomEngine.Modules;
using BloomEngine.Utilities;
using Il2CppReloaded;
using Il2CppReloaded.Input;
using Il2CppReloaded.UI;
using Il2CppSource.UI;
using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BloomEngine.Helpers;

/// <summary>
/// Static helper class that provides methods for creating UI elements.
/// </summary>
public static class UIHelper
{
    /// <summary>
    /// Gets a bool value that specifies whether the Main Menu is currently active.
    /// </summary>
    public static bool InMainMenu { get; private set; }

    /// <summary>
    /// Gets the Main Menu panel view, or null if the Main Menu is currently inactive.
    /// </summary>
    public static MainMenuPanelView MainMenu { get; private set; }

    /// <summary>
    /// Gets the global panel container.
    /// </summary>
    public static PanelViewContainer GlobalPanels { get; private set; }

    /// <summary>
    /// Gets the Main Menu's achievements UI, if the Main Menu is currently active.
    /// </summary>
    public static AchievementsUI Achievements { get; private set; }

    public static TMP_FontAsset Font1 { get; private set; }
    public static TMP_FontAsset Font2 { get; private set; }

    // UI element templates which have been cloned and saved
    private static GameObject textboxTemplate;
    private static GameObject buttonTemplate;
    private static GameObject checkboxTemplate;
    private static GameObject dropdownTemplate;
    private static GameObject sliderTemplate;

    /// <summary>
    /// Called by the Bootstrap when the Main Menu is loaded.
    /// </summary>
    internal static void OnMainMenuLoaded(MainMenuPanelView mainMenu, PanelViewContainer globalPanels, AchievementsUI achievements)
    {
        MainMenu = mainMenu;
        GlobalPanels = globalPanels;
        Achievements = achievements;

        Font1 = MainMenu.transform.FindComponent<TextMeshProUGUI>("Canvas/Layout/Center/Main/AccountSign/SignTop/NameLabel").font;
        Font2 = MainMenu.transform.parent.FindComponent<TextMeshProUGUI>("P_HelpPanel/Canvas/Layout/Center/PageCount/PageLabel").font;
    }

    /// <summary>
    /// Called by the bootstrap when the UI templates are ready to be created.
    /// </summary>
    internal static void CreateTemplates()
    {
        Transform optionsPanelContent = GlobalPanels.transform.Find("P_OptionsPanel/P_OptionsPanel_Canvas/Layout/Center/Panel/Top/NormalOptions/");

        GameObject templateContainer = new GameObject("BloomEngine_Templates");
        GameObject.DontDestroyOnLoad(templateContainer);
        Transform container = templateContainer.transform;

        buttonTemplate = GameObject.Instantiate(MainMenu.transform.parent.Find("P_QuitPanel/Canvas/Layout/Center/Window/Buttons/P_BacicButton_Quit").gameObject, container);
        buttonTemplate.name = "ButtonTemplate";
        textboxTemplate = GameObject.Instantiate(MainMenu.transform.parent.Find("P_UsersPanel_Rename/Canvas/Layout/Center/Rename/NameInputField").gameObject, container);
        textboxTemplate.name = "TextboxTemplate";
        checkboxTemplate = GameObject.Instantiate(optionsPanelContent.Find("Vibration/VibrationP_CheckBox (1)").gameObject, container);
        checkboxTemplate.name = "CheckboxTemplate";
        dropdownTemplate = GameObject.Instantiate(optionsPanelContent.Find("Resolution/Dropdown").gameObject, container);
        dropdownTemplate.name = "DropdownTemplate";
        sliderTemplate = GameObject.Instantiate(optionsPanelContent.Find("Music/MusicP_Slider").gameObject, container);
        sliderTemplate.name = "SliderTemplate";

        RemoveBindersAndLocalizers(templateContainer.gameObject);
        GameObject.Destroy(buttonTemplate.GetComponent<ExitGame>());

        // Hook scene loaded event to determine if the current scene is the main menu
        SceneManager.add_sceneLoaded((scene, mode) => InMainMenu = scene.name == "Frontend");
    }

    /// <summary>
    /// Creates a new Button UI element with the specified name, parent, display text, and click action.
    /// </summary>
    /// <param name="name">The name to assign to the button GameObject.</param>
    /// <param name="parent">The parent RectTransform under which the button will be instantiated.</param>
    /// <param name="text">The text to set the button label to.</param>
    /// <param name="onClick">The action to invoke when the button is clicked.</param>
    /// <returns>A GameObject representing the newly created button, configured with the specified properties.</returns>
    public static GameObject CreateButton(string name, RectTransform parent, string text, Action onClick)
    {
        GameObject button = GameObject.Instantiate(buttonTemplate, parent);
        UIHelper.ModifyButton(button, name, text, onClick);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500f, rect.sizeDelta.y);

        return button;
    }

    /// <summary>
    /// Creates a new text input field as a child of the specified parent, with optional placeholder text.
    /// </summary>
    /// <param name="name">The name to assign to the created input field GameObject.</param>
    /// <param name="parent">The parent RectTransform under which the input field will be instantiated.</param>
    /// <param name="placeholder">The placeholder text to display when the input field is empty. If null, the placeholder will be hidden.</param>
    /// <param name="onTextChanged">An optional callback invoked whenever the text in the input field changes.</param>
    /// <param name="onDeselect">An optional callback invoked when the input field is deselected or submitted.</param>
    /// <returns>A ReloadedInputField instance representing the newly created text input field.</returns>
    public static ReloadedInputField CreateTextField(string name, RectTransform parent, string placeholder = null, Action<ReloadedInputField> onTextChanged = null, Action<ReloadedInputField> onDeselect = null)
    {
        GameObject obj = GameObject.Instantiate(textboxTemplate, parent);
        obj.name = name;

        if (placeholder is null)
            obj.transform.Find("Text Area").Find("Placeholder").gameObject.SetActive(false);
        else obj.transform.Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().m_text = placeholder;

        ReloadedInputField field = obj.GetComponent<ReloadedInputField>();
        field.onValueChanged = new();
        field.onValueChanged.AddListener(text => onTextChanged?.Invoke(field));

        field.onDeselect = new();
        field.onDeselect.AddListener(text => onDeselect?.Invoke(field));
        field.onSubmit = new();
        field.onSubmit.AddListener(text => onDeselect?.Invoke(field));

        return field;
    }

    /// <summary>
    /// Creates a new Checkbox UI element as a Toggle under the specified parent.
    /// </summary>
    /// <param name="name">The name to assign to the created checkbox GameObject.</param>
    /// <param name="parent">The RectTransform that will serve as the parent for the checkbox.</param>
    /// <param name="value">The initial checked state of the checkbox, which is <see langword="true"/> by default.</param>
    /// <param name="onValueChanged">An optional callback that is invoked whenever the checkbox value changes.</param>
    /// <returns>A Toggle component representing the created checkbox with the provided default value.</returns>
    public static Toggle CreateCheckbox(string name, RectTransform parent, bool value = false, Action<bool> onValueChanged = null)
    {
        GameObject obj = GameObject.Instantiate(checkboxTemplate, parent);
        obj.name = name;

        Toggle toggle = obj.GetComponent<Toggle>();
        toggle.isOn = value;

        toggle.onValueChanged = new();
        toggle.onValueChanged.AddListener(val => onValueChanged?.Invoke(val));

        return toggle;
    }

    /// <summary>
    /// Creates a PvZ-style Dropdown UI element as a child of the specified parent, and sets its options to the values of an enum.
    /// </summary>
    /// <param name="name">The name to assign to the newly created Dropdown GameObject.</param>
    /// <param name="parent">The RectTransform that will serve as the parent for the Dropdown.</param>
    /// <param name="selectedIndex">The index of the default selected option. If the index is out of range, it is set to 0.</param>
    /// <param name="onValueChanged">An optional callback that is invoked whenever the Dropdowns's selection changes.</param>
    /// <returns>A ReloadedDropdown component with the newly added enum options.</returns>
    public static ReloadedDropdown CreateDropdown<TEnum>(string name, RectTransform parent, int selectedIndex = 0, Action<TEnum> onValueChanged = null) where TEnum : Enum
    {
        GameObject obj = GameObject.Instantiate(dropdownTemplate, parent);
        obj.name = name;

        ReloadedDropdown dropdown = obj.GetComponent<ReloadedDropdown>();
        dropdown.ClearOptions();

        TEnum[] values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();

        if (selectedIndex > values.Length - 1 || selectedIndex < 0)
            selectedIndex = 0;

        dropdown.AddOptions(values.Select(value => value.ToString()).ToIl2CppList());
        dropdown.SetValueWithoutNotify(selectedIndex);
        dropdown.RefreshShownValue();

        // On value changed events
        dropdown.onValueChanged = new();
        dropdown.onValueChanged?.AddListener(selection =>
        {
            onValueChanged?.Invoke(values[selection]);
            dropdown.Hide(); // Force hide dropdown after selection
        });

        return dropdown;
    }

    /// <summary>
    /// Creates a new PvZ-style Slider UI element as a child of the specified parent, with the given name, value range, and
    /// default value. Optionally attaches a callback to handle value changes.
    /// </summary>
    /// <param name="name">The name to assign to the newly created Slider GameObject.</param>
    /// <param name="parent">The RectTransform that will serve as the parent for the Slider.</param>
    /// <param name="defaultValue">The initial value to set for the Slider. Must be within the defined range.</param>
    /// <param name="minValue">The minimum value allowed for the Slider.</param>
    /// <param name="maxValue">The maximum value allowed for the Slider.</param>
    /// <param name="onValueChanged">An optional callback that is invoked whenever the Slider's value changes.</param>
    /// <returns>A Slider component configured with the specified range and value.</returns>
    public static Slider CreateSlider(string name, RectTransform parent, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null)
    {
        GameObject obj = GameObject.Instantiate(sliderTemplate, parent);
        obj.name = name;

        Slider slider = obj.GetComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.SetValueWithoutNotify(Math.Clamp(defaultValue, minValue, maxValue));

        slider.onValueChanged = new();
        slider.onValueChanged.AddListener(val => onValueChanged?.Invoke(val));

        // Modify anchor and pivot of slider rects to stretch horizontally
        var handleArea = obj.transform.Find("Handle Slide Area").gameObject.GetComponent<RectTransform>();
        if (handleArea)
        {
            handleArea.anchorMin = new Vector2(0f, handleArea.anchorMin.y);
            handleArea.anchorMax = new Vector2(1f, handleArea.anchorMax.y);
            handleArea.offsetMin = new Vector2(0f, handleArea.offsetMin.y);
            handleArea.offsetMax = new Vector2(0f, handleArea.offsetMax.y);
        }

        handleArea.Find("Handle").GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

        var background = obj.transform.Find("Background").gameObject.GetComponent<RectTransform>();
        if (background)
        {
            background.anchorMin = new Vector2(0f, background.anchorMin.y);
            background.anchorMax = new Vector2(1f, background.anchorMax.y);
            background.offsetMin = new Vector2(0f, background.offsetMin.y);
            background.offsetMax = new Vector2(0f, background.offsetMax.y);
        }

        // Force a layout rebuild so the UI updates
        LayoutRebuilder.ForceRebuildLayoutImmediate(obj.GetComponent<RectTransform>());

        return slider;
    }


    /// <summary>
    /// Creates a custom popup that can be modified to display any message.
    /// </summary>
    /// <param name="panelId">The internal id of the new panel.</param>
    /// <param name="panelName">The object name of the new panel.</param>
    /// <param name="header">The text to set as the header of the new panel.</param>
    /// <param name="subheader">The text to show in the main body of the new panel.</param>
    /// <returns>A <see cref="CustomPopup"/> instance that can be used to customize the popup.</returns>
    public static CustomPopup CreatePopup(string panelId, string panelName, string header = null, string subheader = null)
    {
        if(string.IsNullOrWhiteSpace(panelId))
            throw new ArgumentNullException(nameof(panelId));
        if(string.IsNullOrWhiteSpace(panelName))
            throw new ArgumentNullException(nameof(panelName));

        var popup = new CustomPopup(panelId, panelName);

        if(!string.IsNullOrWhiteSpace(header))
            popup.SetHeader(header);
        if(!string.IsNullOrWhiteSpace(subheader))
            popup.SetSubheader(subheader);

        return popup;
    }

    /// <summary>
    /// Creates a new <see cref="GameObject"/> with a <see cref="RectTransform"/> and returns it.
    /// Useful for quickly creating a UI container object.
    /// </summary>
    /// <param name="parent">UI parent rect transform.</param>
    /// <param name="name">The string to set as the name of the created object.</param>
    /// <returns>The UI <see cref="RectTransform"/> component on the created wrapper object.</returns>
    public static RectTransform CreateUIWrapper(RectTransform parent, string name)
    {
        RectTransform rect = new GameObject(name).AddComponent<RectTransform>();
        rect.SetParent(parent);
        return rect;
    }

    /// <summary>
    /// Sets the parent RectTransform of another UI RectTransform, resetting anchors, offsets and pivot.
    /// </summary>
    /// <param name="parent">The parent UI rect.</param>
    /// <param name="child">The UI rect to place under the parent and modify.</param>
    public static void SetParentAndStretch(RectTransform child, RectTransform parent)
    {
        child.SetParent(parent);
        child.anchorMin = Vector2.zero;
        child.anchorMax = Vector2.one;
        child.offsetMin = Vector2.zero;
        child.offsetMax = Vector2.zero;
        child.pivot = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Automatically adds an <see cref="EventTrigger"/> component to an object and adds an action of the provided type.
    /// </summary>
    /// <param name="obj">The <see cref="GameObject"/> to add an event trigger to.</param>
    /// <param name="type">Type of event to add the action to.</param>
    /// <param name="action">The code to execute on this event.</param>
    public static void AddEventTrigger(GameObject obj, EventTriggerType type, Action<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry() { eventID = type };
        entry.callback.AddListener(action);

        EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
        trigger.triggers ??= new Il2CppSystem.Collections.Generic.List<EventTrigger.Entry>();
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// Destroys all <see cref="TextLocalizer"/> and <see cref="Binder"/> components on an object and its children.
    /// </summary>
    /// <param name="obj">The <see cref="GameObject"/> to remove these components from.</param>
    public static void RemoveBindersAndLocalizers(GameObject obj)
    {
        foreach (var localizer in obj.GetComponentsInChildren<TextLocalizer>(true))
            GameObject.Destroy(localizer);
        foreach (var binder in obj.GetComponentsInChildren<Binder>(true))
            GameObject.Destroy(binder);
    }

    /// <summary>
    /// Starts a coroutine to fade the alpha of a UI element over time.
    /// Calling this method again on the same UI element will stop previous fade coroutines.
    /// </summary>
    /// <param name="uiRect">The UI element to apply the fade to.</param>
    /// <param name="target">Target alpha value by the end of the fade.</param>
    /// <param name="duration">The duration of the fade in seconds.</param>
    public static void FadeUIAlpha(RectTransform uiRect, float target, float duration)
    {
        if (!uiRect) return;

        if (fadeCoroutines.TryGetValue(uiRect, out object fadeCoToken))
            MelonCoroutines.Stop(fadeCoToken);

        fadeCoroutines[uiRect] = MelonCoroutines.Start(CoFadeAlphaTo(uiRect, target, duration));
    }

    private static IEnumerator CoFadeAlphaTo(RectTransform uiRect, float target, float duration)
    {
        CanvasGroup group = uiRect.GetComponent<CanvasGroup>() ?? uiRect.gameObject.AddComponent<CanvasGroup>();

        float start = group.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        group.alpha = target;
        fadeCoroutines.Remove(uiRect, out _);
    }

    private static readonly Dictionary<RectTransform, object> fadeCoroutines = new();


    /// <summary>
    /// Cleans up a PvZ Button and updates it with custom values.
    /// </summary>
    internal static GameObject ModifyButton(GameObject buttonObj, string newName, string newText, Action onClick)
    {
        // Update name and text
        buttonObj.name = newName;
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().SetText(newText);

        // Remove garbage components
        if (buttonObj.TryGetComponent<ExitGame>(out var exit))
            GameObject.Destroy(exit);
        if (buttonObj.TryGetComponent<TextLocalizer>(out var localiser))
            GameObject.Destroy(localiser);
        foreach (var local in buttonObj.GetComponentsInChildren<TextLocalizer>())
            GameObject.Destroy(local);
        if (buttonObj.TryGetComponent<UnityButtonBinder>(out var binder))
            GameObject.Destroy(binder);

        // Add onClick event
        if (onClick is not null)
        {
            var button = buttonObj.GetComponent<Button>();
            button.onClick = new();
            button.onClick.AddListener(onClick);
        }

        return buttonObj;
    }
}