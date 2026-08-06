using BloomEngine.Extensions;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BloomEngine.Config;
using BloomEngine.UI;
using BloomEngine.Helpers;

namespace BloomEngine.ModMenu.UI;

internal sealed class ModMenuItemUI
{
    // Constants for UI sizes
    private const int IconSize = 225;
    private const int IconBorderSize = 275;


    private readonly GameObject itemObject;
    private readonly RectTransform iconContainer;
    private readonly Image iconImage;

    private readonly ModMenuEntry? entry;
    private readonly MelonMod mod;

    private static readonly Sprite configIconSprite     = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.ConfigIcon.png");
    private static readonly Sprite defaultIconSprite    = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.DefaultModIcon.png");
    private static readonly Sprite modIconBorderSprite  = AssetHelper.LoadSprite<BloomEngineMod>("BloomEngine.Resources.ModIconBorder.png");

    private ModMenuItemUI(MelonMod mod, Transform parent, GameObject template)
    {
        // Store the mod and try get the mod entry
        this.mod = mod;
        ModMenuService.ModEntries.TryGetValue(mod, out entry);

        // Clone an achievement item for this mod entry
        itemObject = GameObject.Instantiate(template, parent);
        itemObject.SetActive(true);
        itemObject.name = $"ModEntry_{(entry?.DisplayName ?? mod.Info.Name).Replace(" ", "")}";

        SetupTextLabels();
        iconContainer = SetupModIcon();
        iconImage = iconContainer.Find("Icon").GetComponent<Image>();

        // Create the icon border and config button as children of the new container
        CreateIconBorder();
        CreateConfigButton();
    }

    /// <summary>
    /// Creates a new ModMenu item UI element and populates it with the mod's information.
    /// </summary>
    /// <param name="mod">The <see cref="MelonMod"/> to which this entry belongs.</param>
    /// <param name="parent">The parent transform to place this UI element under.</param>
    /// <param name="template">The achivement object to use as a template.</param>
    /// <returns>The created mod menu item UI.</returns>
    internal static ModMenuItemUI Create(MelonMod mod, Transform parent, GameObject template) => new ModMenuItemUI(mod, parent, template);

    /// <summary>
    /// Modifies the mod icon's pivot and size, also creating a container for it.
    /// </summary>
    /// <returns>The Image component for the mod icon.</returns>
    private RectTransform SetupModIcon()
    {
        // Update the icon's pivot and size and set the sprite
        var image = itemObject.transform.Find("Icon").GetComponent<Image>();
        RectTransform iconRect = image.GetComponent<RectTransform>();
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(IconSize, IconSize);
        image.sprite = entry?.Icon ?? defaultIconSprite;

        // Create an icon container to hold the icon, border and config button
        GameObject containerObj = GameObject.Instantiate(image.gameObject, itemObject.transform);
        GameObject.Destroy(containerObj.GetComponent<Image>());
        containerObj.name = "IconContainer";

        RectTransform containerRect = containerObj.GetComponent<RectTransform>();
        containerRect.pivot = new Vector2(0.2f, 0.5f);
        iconRect.SetParent(containerRect);

        return containerRect;
    }

    /// <summary>
    /// Finds the text labels and sets their text, position and color.
    /// </summary>
    private void SetupTextLabels()
    {
        // Try find and set up title label
        if (itemObject.TryFindComponent<TextMeshProUGUI>("Title", out var title))
        {
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, 15);
            title.text = entry?.DisplayName ?? ModMenuEntry.GetDefaultModName(mod);

            if(entry is null)
                title.color = new Color(1f, 0.6f, 0.1f, 1f); // Make the mod name yellow if it isn't registered
        }

        // Try find and set up subheader label
        if (itemObject.TryFindComponent<TextMeshProUGUI>("Subheader", out var subheader))
        {
            subheader.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, -67);
            subheader.text = entry?.Description ?? ModMenuEntry.GetDefaultModDescription(mod);

            subheader.maxVisibleLines = 4; // Modify the text rect to fit more lines in the description
        } 
    }

    /// <summary>
    /// Clones the mod's icon, resizes it and updates the sprite to the border. 
    /// </summary>
    private void CreateIconBorder()
    {
        Image borderImage = GameObject.Instantiate(iconImage, iconContainer);
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
        if (entry?.Config is null || entry.Config.IsEmpty)
            return;

        // Create the config icon
        GameObject configIcon = GameObject.Instantiate(iconImage.gameObject, iconContainer);
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