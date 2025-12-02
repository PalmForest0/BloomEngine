using Il2Cpp;
using Il2CppReloaded;
using Il2CppReloaded.Input;
using Il2CppReloaded.UI;
using Il2CppSource.UI;
using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using Il2CppTMPro;
using MelonLoader;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomEngine.Utilities;

public static class UIHelper
{
    public static MainMenuPanelView MainMenu { get; private set; }
    public static TMP_FontAsset Font1 { get; private set; }
    public static TMP_FontAsset Font2 { get; private set; }

    internal static void Initialize(MainMenuPanelView mainMenu)
    {
        MainMenu = mainMenu;
        Font1 = MainMenu.transform.FindComponent<TextMeshProUGUI>("Canvas/Layout/Center/Main/AccountSign/SignTop/NameLabel").font;
        Font2 = MainMenu.transform.parent.FindComponent<TextMeshProUGUI>("P_HelpPanel/Canvas/Layout/Center/PageCount/PageLabel").font;
    }

    public static GameObject CreatePvZButton(string name, Transform parent, string text, Action onClick)
    {
        GameObject button = GameObject.Instantiate(MainMenu.transform.parent.Find("P_QuitPanel/Canvas/Layout/Center/Window/Buttons/P_BacicButton_Quit").gameObject, parent);
        UIHelper.ModifyPvZButton(button, name, text, onClick);

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500f, rect.sizeDelta.y);

        return button;
    }

    public static GameObject ModifyPvZButton(GameObject buttonObj, string newName, string newText, Action onClick)
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
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener((UnityAction)onClick);

        return buttonObj;
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