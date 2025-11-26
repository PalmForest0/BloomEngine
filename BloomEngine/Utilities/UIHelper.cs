using Il2CppReloaded.Input;
using Il2CppReloaded.UI;
using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Utilities;

public static class UIHelper
{
    public static MainMenuPanelView MainMenu { get; private set; }
    public static TMP_FontAsset Font { get; private set; }

    internal static void Initialize(MainMenuPanelView mainMenu)
    {
        MainMenu = mainMenu;
        Font = MainMenu.transform.FindComponent<TextMeshProUGUI>("Canvas/Layout/Center/Main/AccountSign/SignTop/NameLabel").font;
    }

    public static GameObject CreateButton(string name, Transform parent, string text, Action onClick)
    {
        GameObject button = GameObject.Instantiate(MainMenu.transform.parent.Find("P_QuitPanel/Canvas/Layout/Center/Window/Buttons/P_BacicButton_Quit").gameObject, parent);
        GameObject.Destroy(button.GetComponent<Il2CppReloaded.ExitGame>());
        GameObject.Destroy(button.GetComponent<TextLocalizer>());
        GameObject.Destroy(button.GetComponent<UnityButtonBinder>());

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500f, rect.sizeDelta.y);

        button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)onClick);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.name = name;

        return button;
    }

    public static GameObject CreateTextField(string name, RectTransform parent, string placeholder = null, Action<ReloadedInputField> onTextChanged = null, Action<ReloadedInputField> onDeselect = null)
    {
        GameObject obj = GameObject.Instantiate(MainMenu.transform.parent.Find("P_UsersPanel_Rename/Canvas/Layout/Center/Rename/NameInputField").gameObject, parent);
        obj.name = name;

        GameObject.Destroy(obj.GetComponent<InputBinder>());

        foreach (var local in obj.GetComponentsInChildren<TextLocalizer>())
            GameObject.Destroy(local);

        if (placeholder is null)
            obj.transform.Find("Text Area").Find("Placeholder").gameObject.SetActive(false);
        else obj.transform.Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().m_text = placeholder;

        ReloadedInputField field = obj.GetComponent<ReloadedInputField>();
        field.onValueChanged.RemoveAllListeners();
        field.onValueChanged.AddListener((Action<string>)(text => onTextChanged?.Invoke(field)));

        field.onDeselect.RemoveAllListeners();
        field.onDeselect.AddListener((Action<string>)(text => onDeselect?.Invoke(field)));
        field.onSubmit.RemoveAllListeners();
        field.onSubmit.AddListener((Action<string>)(text => onDeselect?.Invoke(field)));

        return obj;
    }
}