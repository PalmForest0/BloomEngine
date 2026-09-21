using System.Text;
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

    /// <summary>
    /// A function that determines the display names of options.
    /// </summary>
    private Func<TEnum, string?>? nameSelector = opt => StringToReadable(opt.ToString());
    
    internal EnumConfigInput(string name, string description, TEnum defaultValue) : base(name, description, defaultValue) { }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        RectTransform wrapper = UIHelper.CreateUIWrapper(parent, name);

        string[] strings = options.Select(opt => nameSelector?.Invoke(opt) ?? opt.ToString()).ToArray();
        Dropdown = UIHelper.CreateDropdown("Dropdown_Internal", wrapper, strings, Convert.ToInt32(Value), (_, _) => HandleInputChanged());
        
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

    /// <summary>
    /// Specifies the text string that should be used for a given option when constructing the dropdown UI. A null string will call ToString() on the option.
    /// </summary>
    /// <param name="selector">A selector that specifies the display string (or null to use the default via ToString) for a given enum option.</param>
    public EnumConfigInput<TEnum> WithOptionNames(Func<TEnum, string?> selector)
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

    /// <summary>
    /// Converts a string that would show up in code to a readable display string.
    /// </summary>
    /// <param name="input">Input string that should be processed.</param>
    /// <returns>A readable display string.</returns>
    private static string StringToReadable(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder();
        sb.Append(char.ToUpper(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            char current = input[i];
            char prev = input[i - 1];
            bool lastCharIsSpace = sb.Length > 0 && sb[^1] == ' ';

            if (current is '_' or '-')
            {
                if (!lastCharIsSpace)
                    sb.Append(' ');
                continue;
            }

            if (!lastCharIsSpace)
            {
                // Splits words from lowercase to uppercase
                if (char.IsUpper(current) && !char.IsUpper(prev))
                    sb.Append(' ');
                // Splits words between two uppercase
                else if (char.IsUpper(current) && char.IsUpper(prev) && i + 1 < input.Length && char.IsLower(input[i + 1]))
                    sb.Append(' ');
                // Splits digits before
                else if (char.IsDigit(current) && !char.IsDigit(prev))
                    sb.Append(' ');
                // Splits digits after
                else if (!char.IsDigit(current) && char.IsDigit(prev))
                    sb.Append(' ');
            }

            sb.Append(current);
        }

        return sb.ToString().TrimEnd();
    }
}