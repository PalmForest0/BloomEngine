using BloomEngine.Config.Services;
using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomEngine.ModMenu.UI;

internal sealed class ModMenuItemUI
{
    private const int IconSize = 225;
    private const int IconBorderSize = 275;

    private static Sprite configIconSprite = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.ConfigIcon.png");
    private static Sprite defaultIconSprite = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.DefaultModIcon.png");
    private static Sprite modIconBorderSprite = AssetHelper.LoadEmbeddedSprite("BloomEngine.Resources.ModIconBorder.png");

    private readonly GameObject itemObject;
    private RectTransform iconContainerRect;
    private Image iconImage;

    private readonly bool isRegistered;
    private readonly ModMenuEntry entry;
    private readonly MelonMod mod;

    private ModMenuItemUI(MelonMod mod, Transform parent, GameObject template)
    {
        this.mod = mod;

        isRegistered = ModMenuService.ModEntries.TryGetValue(mod, out entry);

        // Clone an achievement item for this mod entry
        itemObject = GameObject.Instantiate(template, parent);
        itemObject.SetActive(true);
        itemObject.name = $"ModEntry_{(isRegistered ? entry.DisplayName : mod.Info.Name).Replace(" ", "")}";

        SetupTextLabels();
        SetupModIcon();
        CreateIconBorder();
        CreateConfigButton();
    }

    /// <summary>
    /// Creates a new ModMenu item UI element and populates it with the mod's information.
    /// </summary>
    /// <param name="mod">The <see cref="MelonMod"/> to which this entry belongs.</param>
    /// <param name="parent">The parent transform to place this UI element under.</param>
    /// <param name="template">The achivement object to use as a template.</param>
    internal static void Create(MelonMod mod, Transform parent, GameObject template)
        => new ModMenuItemUI(mod, parent, template);

    /// <summary>
    /// Modifies the mod icon's pivot and size, also creating a container for it.
    /// </summary>
    private void SetupModIcon()
    {
        // Update the icon's pivot and size and set the sprite
        iconImage = itemObject.transform.Find("Icon").GetComponent<Image>();
        RectTransform iconRect = iconImage.GetComponent<RectTransform>();
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(IconSize, IconSize);
        iconImage.sprite = (isRegistered && entry.Icon is not null) ? entry.Icon : defaultIconSprite;

        // Create an icon container to hold the icon, border and config button
        GameObject iconContainerObj = GameObject.Instantiate(iconImage.gameObject, itemObject.transform);
        GameObject.Destroy(iconContainerObj.GetComponent<Image>());
        iconContainerRect = iconContainerObj.GetComponent<RectTransform>();
        iconContainerRect.pivot = new Vector2(0.2f, 0.5f);
        iconRect.SetParent(iconContainerRect);
        iconContainerObj.name = "IconContainer";
    }

    /// <summary>
    /// Finds the text labels and sets their text, position and color.
    /// </summary>
    private void SetupTextLabels()
    {
        // Modify the text rects to fit more lines in the description
        var title = itemObject.FindComponent<TextMeshProUGUI>("Title");
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, 15);
        var subheader = itemObject.FindComponent<TextMeshProUGUI>("Subheader");
        subheader.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, -67);
        subheader.maxVisibleLines = 4;

        if (isRegistered)
        {
            title.text = entry.DisplayName;
            subheader.text = entry.Description;
        }
        else
        {
            title.text = ModMenuEntry.GetDefaultModName(mod);
            title.color = new Color(1f, 0.6f, 0.1f, 1f);
            subheader.text = ModMenuEntry.GetDefaultModDescription(mod);
        }   
    }

    /// <summary>
    /// Clones the mod's icon, resizes it and updates the sprite to the border. 
    /// </summary>
    private void CreateIconBorder()
    {
        Image borderImage = UnityEngine.Object.Instantiate(iconImage, iconContainerRect);
        borderImage.name = "IconBorder";
        borderImage.sprite = modIconBorderSprite;
        borderImage.raycastTarget = false;

        RectTransform borderRect = borderImage.GetComponent<RectTransform>();
        borderRect.pivot = new Vector2(0.5f, 0.5f);
        borderRect.sizeDelta = new Vector2(IconBorderSize, IconBorderSize);
    }

    /// <summary>
    /// Creates a config icon that appears when you hover over the mod icon.
    /// </summary>
    private void CreateConfigButton()
    {
        if (!isRegistered || entry.Config is null || entry.Config.IsEmpty)
            return;

        // Create the config icon
        GameObject configIcon = GameObject.Instantiate(iconImage.gameObject, iconContainerRect);
        configIcon.name = "ConfigIcon";

        RectTransform configIconRect = configIcon.GetComponent<RectTransform>();
        configIconRect.pivot = new Vector2(0.5f, 0.5f);
        configIconRect.sizeDelta = new Vector2(IconSize, IconSize);

        Image configIconImg = configIcon.GetComponent<Image>();
        configIconImg.sprite = configIconSprite;
        configIconImg.raycastTarget = false;

        configIcon.AddComponent<CanvasGroup>().alpha = 0f;

        // Add a button component to the icon object
        Button configButton = iconImage.gameObject.AddComponent<Button>();
        configButton.onClick.AddListener(() => ConfigService.ShowConfigPanel(entry));

        // Adjust the icon's hover colors
        var colors = configButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.fadeDuration = 0.1f;
        configButton.colors = colors;

        // Add event triggers for pointer enter and exit to fade in/out the config icon
        EventTrigger trigger = iconImage.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new Il2CppSystem.Collections.Generic.List<EventTrigger.Entry>();

        // On pointer enter trigger - fade in config icon
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener(_ => UIHelper.FadeUIAlpha(configIconRect, 1f, 0.2f));
        trigger.triggers.Add(pointerEnter);

        // On pointer exit trigger - fade out config icon
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener(_ => UIHelper.FadeUIAlpha(configIconRect, 0f, 0.2f));
        trigger.triggers.Add(pointerExit);
    }
}