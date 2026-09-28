using System.Globalization;
using BloomEngine.Helpers;
using BloomEngine.UI;
using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes an <see cref="Enum"/> value using a dropdown.
/// </summary>
public sealed class EnumConfigField<TEnum>(string identifier, string displayName, TEnum defaultValue)
    : ConfigField<TEnum, EnumConfigField<TEnum>>(identifier, displayName, defaultValue) where TEnum : Enum
{
    /// <summary>
    /// The UI dropdown element which corresponds to this config field in the config panel.
    /// </summary>
    public ReloadedDropdown Dropdown { get; private set; } = null!;
    
    /// <summary>
    /// The list of options that gets shown in the dropdown.
    /// </summary>
    private List<TEnum> options = GetEnumOptions<TEnum>().ToList();

    /// <summary>
    /// A function that determines the display names of options.
    /// </summary>
    private Func<TEnum, string?>? nameSelector = opt => StringHelper.StringToReadable(opt.ToString());

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        var wrapper = UIHelper.CreateUIWrapper(parent, name);

        string[] strings = options.Select(opt => nameSelector?.Invoke(opt) ?? opt.ToString()).ToArray();
        int selected = Convert.ToInt32(Value, CultureInfo.InvariantCulture);
        
        Dropdown = UIHelper.CreateDropdown("Dropdown_Internal", wrapper, strings, selected, onValueChanged: (_, _) => HandleInputChanged());
        
        var dropdownRect = Dropdown.GetComponent<RectTransform>();
        UIHelper.SetParentAndStretch(dropdownRect, wrapper);

        dropdownRect.sizeDelta = new Vector2(0, 60);
        dropdownRect.anchoredPosition += new Vector2(0, -15);
        return wrapper.gameObject;
    }

    /// <summary>
    /// Specifies an explicit option order for the dropdown. Any missing options will be appended to the end in numeric order.
    /// </summary>
    /// <param name="order">An array of enum entries in the desired order.</param>
    /// <returns>This config field, with the provided options ordered at the start.</returns>
    public EnumConfigField<TEnum> WithOptionOrder(params TEnum[] order)
    {
        var seen = new HashSet<TEnum>();
        
        // Adds all ordered options, then all remaining options, while excluding duplicates
        options = new List<TEnum>(order.Length);
        options.AddRange(order.Where(seen.Add));
        options.AddRange(GetEnumOptions<TEnum>().Where(seen.Add));
        
        return this;
    }

    /// <summary>
    /// Specifies the text string that should be used for a given option when constructing the dropdown UI. A null string will call ToString() on the option.
    /// </summary>
    /// <param name="selector">A selector that specifies the display string (or null to use the default via ToString) for a given enum option.</param>
    /// <returns>This config field, with the given selector used to construct the options.</returns>
    public EnumConfigField<TEnum> WithOptionNames(Func<TEnum, string?> selector)
    {
        nameSelector = selector;
        return this;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = options[Dropdown.value];

    /// <inheritdoc/>
    protected override void SetDisplayedValue(TEnum value)
    {
        Dropdown.SetValueWithoutNotify(options.IndexOf(value));
        Dropdown.RefreshShownValue();
    }

    /// <summary>
    /// Gets all options of an enum as an enumerable.
    /// </summary>
    /// <typeparam name="T">An enum type.</typeparam>
    /// <returns>Collection containing all enum options.</returns>
    private static IEnumerable<T> GetEnumOptions<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();
}