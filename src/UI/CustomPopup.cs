using BloomEngine.Extensions;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.UI;

/// <summary>
/// Wrapper for a PvZ Replanted popup with a customizable header, subheader, and up to two buttons.
/// To create a new <see cref="CustomPopup"/>, use <see cref="UIHelper.CreatePopup(string, string)"/>.
/// </summary>
public class CustomPopup : MonoBehaviour
{
    /// <summary>
    /// The in-game <see cref="Il2CppTekly.PanelViews.PanelView"/> that is wrapped by this <see cref="CustomPopup"/> instance.
    /// </summary>
    public PanelView PanelView { get; private set; } = null!;

    /// <summary>
    /// Gets the transform of the window element of this panel.
    /// </summary>
    public Transform Window { get; private set; } = null!;
    
    /// <summary>
    /// Header label of the popup panel.
    /// </summary>
    public TextMeshProUGUI Header { get; private set; } = null!;

    /// <summary>
    /// Subheader (popup body) label of the popup panel.
    /// </summary>
    public TextMeshProUGUI Subheader { get; private set; } = null!;

    /// <summary>
    /// Gets the first button in the panel footer, which can be updated with <see cref="SetFirstButton(bool, string, Action, bool)"/>.
    /// </summary>
    public Button FirstButton { get; private set; } = null!;

    /// <summary>
    /// Gets the second button in the panel footer, which can be updated with <see cref="SetSecondButton(bool, string, Action, bool)"/>.
    /// </summary>
    public Button SecondButton { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the popup is currently visible.
    /// </summary>
    public bool IsVisible { get; private set; }

    public void Awake()
    {
        PanelView = GetComponent<PanelView>();
        
        // Locate all elements
        Window = transform.Find("Canvas/Layout/Center/Window");
        Header = Window.Find("HeaderText").GetComponentInChildren<TextMeshProUGUI>(true);
        Subheader = Window.Find("SubheadingText").GetComponentInChildren<TextMeshProUGUI>(true);
        FirstButton = Window.Find("Buttons/P_BacicButton_Yes").GetComponentInChildren<Button>(true);
        SecondButton = Window.Find("Buttons/P_BacicButton_Ok").GetComponentInChildren<Button>(true);
        
        // Set defaults
        SetHeader(name);
        SetSubheader($"See methods provided by the {nameof(CustomPopup)} class to customise this panel!");
        SetFirstButton(true, "Ok", null);

        // Clean up
        Destroy(Window.Find("Buttons/P_BacicButton_No").gameObject);
        Destroy(Window.Find("Buttons/P_BacicButton_Cancel").gameObject);
        UIHelper.CleanUpChildren(gameObject);
    }

    /// <summary>
    /// Makes this popup visible.
    /// </summary>
    public void Show()
    {
        PanelView.gameObject.SetActive(true);
        IsVisible = true;
    }

    /// <summary>
    /// Hides this popup.
    /// </summary>
    public void Hide()
    {
        PanelView.gameObject.SetActive(false);
        IsVisible = false;
    }


    /// <summary>
    /// Sets the header text at the top of the popup.
    /// </summary>
    /// <param name="text">Text to set the header label to.</param>
    public void SetHeader(string text) => Header.text = text;

    /// <summary>
    /// Sets the subheader text within the popup.
    /// </summary>
    /// <param name="text">Text to set the subheader label to.</param>
    public void SetSubheader(string text) => Subheader.text = text;

    /// <summary>
    /// Configures the first button that appears at the bottom of this popup.
    /// </summary>
    /// <param name="visible">Whether this button should be enabled.</param>
    /// <param name="text">The text to set the button's label to display.</param>
    /// <param name="onClick">A custom action that is invoked when this button is clicked.</param>
    /// <param name="hidePopupOnClick">Whether clicking the button should automatically hide the popup. True by default.</param>
    public void SetFirstButton(bool visible, string text, Action? onClick = null, bool hidePopupOnClick = true)
    {
        FirstButton.gameObject.SetActive(visible);
        FirstButton.GetComponentInChildren<TextMeshProUGUI>().text = text;

        FirstButton.onClick = new Button.ButtonClickedEvent();
        FirstButton.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            if(hidePopupOnClick) Hide();
        });
    }

    /// <summary>
    /// Configures the second button that appears at the bottom of this popup.
    /// </summary>
    /// <param name="visible">Whether this button should be enabled.</param>
    /// <param name="text">The text to set the button's label to display.</param>
    /// <param name="onClick">A custom action that is invoked when this button is clicked.</param>
    /// <param name="hidePopupOnClick">Whether clicking the button should automatically hide the popup. True by default.</param>
    public void SetSecondButton(bool visible, string text, Action? onClick = null, bool hidePopupOnClick = true)
    {
        SecondButton.gameObject.SetActive(visible);
        SecondButton.GetComponentInChildren<TextMeshProUGUI>().text = text;

        SecondButton.onClick = new Button.ButtonClickedEvent();
        SecondButton.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            if (hidePopupOnClick) Hide();
        });
    }

    /// <summary>
    /// Sets the header and subheader of this popup and shows it
    /// </summary>
    /// <param name="header">The string to set the header text to.</param>
    /// <param name="subheader">The string to set the subheader text to.</param>
    public void ShowWithText(string header, string subheader)
    {
        SetHeader(header);
        SetSubheader(subheader);
        Show();
    }
}