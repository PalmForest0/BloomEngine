using Il2CppReloaded.UI;
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
        Font = MainMenu.transform.Find("Canvas").Find("Layout").Find("Center").Find("Main").Find("AccountSign").Find("SignTop").Find("NameLabel").GetComponent<TextMeshProUGUI>().font;
    }

    public static GameObject CreateButton(string name, Transform parent, string text, Action onClick)
    {
        GameObject button = GameObject.Instantiate(MainMenu.transform.parent.Find("P_QuitPanel").Find("Canvas").Find("Layout").Find("Center").Find("Window").Find("Buttons").Find("P_BacicButton_Quit").gameObject, parent);
        GameObject.Destroy(button.GetComponent<Il2CppReloaded.ExitGame>());
        GameObject.Destroy(button.GetComponent<Il2CppTekly.Localizations.TextLocalizer>());

        button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener((UnityAction)onClick);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        button.name = name;

        return button;
    }
}