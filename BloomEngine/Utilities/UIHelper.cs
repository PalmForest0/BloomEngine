using Il2CppReloaded;
using Il2CppReloaded.Binders;
using Il2CppReloaded.Input;
using Il2CppReloaded.UI;
using Il2CppSource.UI;
using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomEngine.Utilities;

public static class UIHelper
{
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

    public static GameObject CreateButton(string name, Transform parent, string text, Action onClick)
    {
        GameObject button = GameObject.Instantiate(buttonTemplate, parent);
        UIHelper.ModifyButton(button, name, text, onClick);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500f, rect.sizeDelta.y);

        return button;
    }

    public static GameObject ModifyButton(GameObject buttonObj, string newName, string newText, Action onClick)
    {
        // Update name and text
        buttonObj.name = newName;
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().SetText(newText);

        // Remove garbage components
        if(buttonObj.TryGetComponent<ExitGame>(out var exit))
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


    public static GameObject CreateTextField(string name, RectTransform parent, string placeholder = null, Action<ReloadedInputField> onTextChanged = null, Action<ReloadedInputField> onDeselect = null)
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

        return obj;
    }

    public static GameObject CreateCheckbox(string name, RectTransform parent, bool value = false, Action<bool> onValueChanged = null)
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

        return obj;
    }

    public static GameObject CreateDropdown(string name, RectTransform parent, Type enumType, int selectedIndex = 0, Action<Enum> onValueChanged = null)
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

        return obj;
    }

    public static GameObject CreateSlider(string name, RectTransform parent, float minValue, float maxValue, float value = default, Action<float> onValueChanged = null)
    {
        GameObject obj = GameObject.Instantiate(sliderTemplate, parent);
        obj.name = name;

        GameObject.Destroy(obj.GetComponent<BinderContainer>());
        GameObject.Destroy(obj.GetComponent<UnitySliderBinder>());
        GameObject.Destroy(obj.GetComponent<ForwardOnInactiveSelectable>());

        Slider slider = obj.GetComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.SetValueWithoutNotify(value == default ? minValue : value);

        slider.onValueChanged = new();
        slider.onValueChanged.AddListener(val => onValueChanged?.Invoke(val));

        return obj;
    }
}