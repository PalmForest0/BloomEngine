using BloomEngine.Config.Inputs.Base;
using BloomEngine.UI;
using Il2CppSource.UI;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="Enum"/> input.<br/>
/// To create an <see cref="EnumConfigInput{TEnum}"/>, use <see cref="ConfigService.CreateEnum{TEnum}(string, string, TEnum)"/>
/// </summary>
public sealed class EnumConfigInput<TEnum> : TypedConfigInput<TEnum, EnumConfigInput<TEnum>> where TEnum : Enum
{
    /// <summary>
    /// The UI dropdown which corresponds to this config input in the config panel.
    /// </summary>
    public ReloadedDropdown Dropdown { get; private set; } = null!;
    
    /// <summary>
    /// The list of options that gets shown in the dropdown.
    /// </summary>
    private List<TEnum> options = GetEnumOptions<TEnum>().ToList();

    internal EnumConfigInput(string name, string description, TEnum defaultValue) : base(name, description, defaultValue) { }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent)
    {
        RectTransform wrapper = UIHelper.CreateUIWrapper(parent, InputObjectName);

        Dropdown = UIHelper.CreateDropdown<TEnum>("Dropdown_Internal", wrapper, Convert.ToInt32(Value), onValueChanged: _ => OnUIChanged());
        RectTransform dropdownRect = Dropdown.GetComponent<RectTransform>();
        UIHelper.SetParentAndStretch(dropdownRect, wrapper);

        dropdownRect.sizeDelta = new Vector2(0, 60);
        dropdownRect.anchoredPosition += new Vector2(0, -15);
        return wrapper.gameObject;
    }

    /// <summary>
    /// Specifies an explicit option order for the dropdown. Any missing options will be appended to the end in numeric order.
    /// </summary>
    /// <param name="order">An array of enum entries in the desired order.</param>
    public EnumConfigInput<TEnum> WithOptionOrder(params TEnum[] order)
    {
        var seen = new HashSet<TEnum>();
        
        // Adds all ordered options, then all remaining options, while excluding duplicates
        options = new List<TEnum>(order.Length);
        options.AddRange(order.Where(seen.Add));
        options.AddRange(GetEnumOptions<TEnum>().Where(seen.Add));
        
        return this;
    }

    /// <inheritdoc/>
    protected internal override void UpdateFromUI() => Value = options[Dropdown.value];

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