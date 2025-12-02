using BloomEngine.Utilities;
using Il2CppReloaded.Input;

namespace BloomEngine.Config;

public interface IConfigProperty
{
    abstract string Name { get; set; }
    abstract Type ValueType { get; }


    abstract object GetValue();
    abstract void SetValue(object value);
    abstract bool ValidateValue(object value);
    abstract object TransformValue(object value);

    /// <summary>
    /// Checks if the type T is a valid property type.
    /// Support for more types like boolean and enums can be added later.
    /// </summary>
    public static bool IsValidPropertyType<T>()
        => TypeHelper.IsNumericType(typeof(T)) || typeof(T) == typeof(string);

    public void ApplyFromInputField(ReloadedInputField field)
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
}