using BloomEngine.Utilities;
using Il2CppReloaded.Input;

namespace BloomEngine.Config;

public interface IConfigProperty
{
    string Name { get; set; }
    Type ValueType { get; }


    object GetValue();
    void SetValue(object value);
    bool ValidateValue(object value);
    object TransformValue(object value);


    /// <summary>
    /// Reads the content of the input field as a string and attempts to convert it to the desired type.
    /// </summary>
    /// <param name="field">Input field to read the content of.</param>
    internal void ApplyFromInputField(ReloadedInputField field)
    {
        // Filter input value
        object value = null;
        if (TypeHelper.IsNumericType(ValueType))
            value = TextHelper.ValidateNumericInput(field.m_Text, ValueType);
        else if (ValueType == typeof(string))
            value = field.m_Text;

        // Transform value
        value = TransformValue(value);

        // Validate and apply value
        if (!ValidateValue(value))
            value = GetValue();
        else SetValue(Convert.ChangeType(value, ValueType));

        field.text = value.ToString();
    }

    /// <summary>
    /// Checks if the type T is a valid property type.
    /// Support for more types like boolean and enums can be added later.
    /// </summary>
    public static bool IsValidPropertyType<T>()
        => TypeHelper.IsNumericType(typeof(T)) || typeof(T) == typeof(string);
}