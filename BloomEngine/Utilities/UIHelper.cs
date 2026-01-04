using Il2CppReloaded;
using Il2CppReloaded.Binders;
using Il2CppReloaded.Input;
using Il2CppReloaded.UI;
using Il2CppSource.UI;
using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomEngine.Utilities;

/// <summary>
/// Static helper class that provides methods for creating UI elements.
/// </summary>
public static class UIHelper
{
    private static readonly Dictionary<RectTransform, object> fadeCoroutines = new();

    public static MainMenuPanelView MainMenu { get; private set; }
    public static TMP_FontAsset Font1 { get; private set; }
    public static TMP_FontAsset Font2 { get; private set; }

    private static GameObject textFieldTemplate;
    private static GameObject buttonTemplate;
    private static GameObject checkboxTemplate;
    private static GameObject dropdownTemplate;
    private static GameObject sliderTemplate;

    internal static void Initialize(MainMenuPanelView mainMenu)
    {
        MainMenu = mainMenu;
        Font1 = MainMenu.transform.FindComponent<TextMeshProUGUI>("Canvas/Layout/Center/Main/AccountSign/SignTop/NameLabel").font;
        Font2 = MainMenu.transform.parent.FindComponent<TextMeshProUGUI>("P_HelpPanel/Canvas/Layout/Center/PageCount/PageLabel").font;

        Transform optionsPanelContent = GameObject.Find("GlobalPanels(Clone)").transform.Find("P_OptionsPanel/P_OptionsPanel_Canvas/Layout/Center/Panel/Top/NormalOptions/");

        textFieldTemplate = MainMenu.transform.parent.Find("P_UsersPanel_Rename/Canvas/Layout/Center/Rename/NameInputField").gameObject;
        buttonTemplate = MainMenu.transform.parent.Find("P_QuitPanel/Canvas/Layout/Center/Window/Buttons/P_BacicButton_Quit").gameObject;
        checkboxTemplate = optionsPanelContent.Find("Vibration/VibrationP_CheckBox (1)").gameObject;
        dropdownTemplate = optionsPanelContent.Find("Resolution/Dropdown").gameObject;
        sliderTemplate = optionsPanelContent.Find("Music/MusicP_Slider").gameObject;
    }

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
        var button = buttonObj.GetComponent<Button>();
        button.onClick = new();
        button.onClick.AddListener(onClick);

        return buttonObj;
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

    public static GameObject CreateButton(string name, Transform parent, string text, Action onClick)
    {
        GameObject button = GameObject.Instantiate(buttonTemplate, parent);
        UIHelper.ModifyButton(button, name, text, onClick);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500f, rect.sizeDelta.y);

        return button;
    }


    public static ReloadedInputField CreateTextField(string name, RectTransform parent, string placeholder = null, Action<ReloadedInputField> onTextChanged = null, Action<ReloadedInputField> onDeselect = null)
    {
        GameObject obj = GameObject.Instantiate(textFieldTemplate, parent);
        obj.name = name;

        GameObject.Destroy(obj.GetComponent<InputBinder>());

        foreach (var local in obj.GetComponentsInChildren<TextLocalizer>())
            GameObject.Destroy(local);

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

    public static Toggle CreateCheckbox(string name, RectTransform parent, bool value = false, Action<bool> onValueChanged = null)
    {
        GameObject obj = GameObject.Instantiate(checkboxTemplate, parent);
        obj.name = name;

        GameObject.Destroy(obj.GetComponent<BinderContainer>());
        GameObject.Destroy(obj.GetComponent<UnityToggleBinder>());
        GameObject.Destroy(obj.GetComponent<ForwardOnInactiveSelectable>());

        Toggle toggle = obj.GetComponent<Toggle>();
        toggle.isOn = value;

        toggle.onValueChanged = new();
        toggle.onValueChanged.AddListener(val => onValueChanged?.Invoke(val));

        return toggle;
    }

    public static ReloadedDropdown CreateDropdown(string name, RectTransform parent, Type enumType, int selectedIndex = 0, Action<Enum> onValueChanged = null)
    {
        if (!enumType.IsEnum)
            return null;

        GameObject obj = GameObject.Instantiate(dropdownTemplate, parent);
        obj.name = name;

        GameObject.Destroy(obj.GetComponent<BinderContainer>());
        GameObject.Destroy(obj.GetComponent<UnityDropdownBinder>());
        GameObject.Destroy(obj.GetComponent<ForwardOnInactiveSelectable>());

        ReloadedDropdown dropdown = obj.GetComponent<ReloadedDropdown>();
        dropdown.ClearOptions();

        var values = Enum.GetValues(enumType).Cast<object>().ToArray();

        if (selectedIndex > values.Length -1 || selectedIndex < 0)
            selectedIndex = 0;

        dropdown.AddOptions(values.Select(value => value.ToString()).ToIl2CppList());
        dropdown.SetValueWithoutNotify(selectedIndex);
        dropdown.RefreshShownValue();

        // On value changed events
        dropdown.onValueChanged = new();
        dropdown.onValueChanged?.AddListener(selection => onValueChanged?.Invoke((Enum)values[selection]));

        return dropdown;
    }

    public static Slider CreateSlider(string name, RectTransform parent, float defaultValue, float minValue, float maxValue, Action<float> onValueChanged = null)
    {
        GameObject obj = GameObject.Instantiate(sliderTemplate, parent);
        obj.name = name;

        GameObject.Destroy(obj.GetComponent<BinderContainer>());
        GameObject.Destroy(obj.GetComponent<UnitySliderBinder>());
        GameObject.Destroy(obj.GetComponent<ForwardOnInactiveSelectable>());

        Slider slider = obj.GetComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.SetValueWithoutNotify(defaultValue);

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
}