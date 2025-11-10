using Il2CppReloaded.Input;
using Il2CppReloaded.UI;
using Il2CppTekly.DataModels.Binders;
using Il2CppTMPro;
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
        GameObject.Destroy(button.GetComponent<Il2CppTekly.Localizations.TextLocalizer>());
        GameObject.Destroy(button.GetComponent<UnityButtonBinder>());

        button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)onClick);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.name = name;

        return button;
    }

    public static ReloadedInputField CreateTextField(string name, Transform parent, string placeholder, Action onTextChanged = null)
    {
        GameObject obj = GameObject.Instantiate(MainMenu.transform.parent.Find("P_UsersPanel_Rename/Canvas/Layout/Center/Rename/NameInputField").gameObject, parent);
        GameObject.Destroy(obj.GetComponent<InputBinder>());

        obj.FindComponent<TextMeshProUGUI>("Text Area/Placeholder").text = placeholder;

        ReloadedInputField field = obj.GetComponent<ReloadedInputField>();

        field.onValueChanged.RemoveAllListeners();
        field.onValueChanged.AddListener((Action<string>)(text => onTextChanged?.Invoke()));

        return field;
    }
}