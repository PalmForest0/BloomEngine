using BloomEngine.Utilities;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomEngine.Modules;

public sealed class CustomPopup
{
    public PanelView Panel { get; private set; }

    public TextMeshProUGUI Header { get; private set; }
    public TextMeshProUGUI Subheader { get; private set; }

    public Button FirstButton { get; private set; }
    public Button SecondButton { get; private set; }

    public bool IsVisible { get; private set; }

    internal CustomPopup(string panelId, string panelName)
    {
        // Create the panel and rename it
        var template = UIHelper.GlobalPanels.transform.Find("P_PopUpMessage02").GetComponentInChildren<PanelView>(true);
        Panel = UnityEngine.Object.Instantiate(template, UIHelper.GlobalPanels.transform);
        Panel.gameObject.name = panelName;
        Panel.name = panelName;
        Panel.m_id = panelId;

        // Locate all elements
        Transform window = Panel.transform.Find("Canvas/Layout/Center/Window");
        Header = window.Find("HeaderText").GetComponentInChildren<TextMeshProUGUI>(true);
        Subheader = window.Find("SubheadingText").GetComponentInChildren<TextMeshProUGUI>(true);
        FirstButton = window.Find("Buttons/P_BacicButton_Yes").GetComponentInChildren<Button>(true);
        SecondButton = window.Find("Buttons/P_BacicButton_Ok").GetComponentInChildren<Button>(true);

        // Set defaults
        SetHeader(panelName);
        SetSubheader($"See methods provided by the {nameof(CustomPopup)} class to customize this panel!");
        SetFirstButton(true, "Ok", null);

        // Clean up
        GameObject.Destroy(window.Find("Buttons/P_BacicButton_No").gameObject);
        GameObject.Destroy(window.Find("Buttons/P_BacicButton_Cancel").gameObject);
        UIHelper.RemoveBindersAndLocalizers(window.gameObject);
    }

    /// <summary>
    /// Makes this popup visible.
    /// </summary>
    public void Show()
    {
        Panel.gameObject.SetActive(true);
        IsVisible = true;
    }

    /// <summary>
    /// Hides this popup.
    /// </summary>
    public void Hide()
    {
        Panel.gameObject.SetActive(false);
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
    public void SetFirstButton(bool visible, string text, Action onClick = null, bool hidePopupOnClick = true)
    {
        FirstButton.gameObject.SetActive(visible);
        FirstButton.GetComponentInChildren<TextMeshProUGUI>().text = text;

        FirstButton.onClick = new();
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
    public void SetSecondButton(bool visible, string text, Action onClick = null, bool hidePopupOnClick = true)
    {
        SecondButton.gameObject.SetActive(visible);
        SecondButton.GetComponentInChildren<TextMeshProUGUI>().text = text;

        SecondButton.onClick = new();
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