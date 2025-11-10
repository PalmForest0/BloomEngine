using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace BloomEngine.Menu;

public class ModEntry
{
    public string Id { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }

    public Texture2D Image { get; private set; }
    public MelonMod Mod { get; private set; }

    public List<ConfigProperty> Properties { get; private set; } = new List<ConfigProperty>();

    public ModEntry(MelonMod mod, string id, string displayName = null)
    {
        Mod = mod;
        Id = id;
        DisplayName = displayName ?? mod.Info.Name;
    }

    public ModEntry AddDescription(string description)
    {
        Description = description;
        return this;
    }

    public ModEntry AddImage(Texture2D image)
    {
        Image = image;
        return this;
    }

    public ModEntry AddConfig(ModConfigBase configInstance)
    {
        foreach (var prop in configInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
        {
            var attribute = prop.GetCustomAttribute<ConfigPropertyAttribute>();
            if (attribute is null)
                continue;

            Type type = prop.PropertyType;
            if (!IsPropertyTypeSupported(type))
            {
                ModMenu.Log($"[ModMenu] Failed to add config property '{prop.Name}' for mod '{DisplayName}': Property type '{type.Name}' is not supported.", LogType.Warning);
                continue;
            }

            // Create getter & setter
            Func<object> getter = () => prop.GetValue(configInstance);
            Action<object> setter = prop.CanWrite ? val => prop.SetValue(configInstance, Convert.ChangeType(val, type)) : null;

            var configProperty = new ConfigProperty(
                attribute.Name,
                type,
                getter,
                setter,
                attribute.Placeholder,
                attribute.Description,
                attribute.InputType == PropertyInputType.Auto ? InferInputType(type) : attribute.InputType
            );

            Properties.Add(configProperty);
        }

        return this;
    }

    public void Register() => ModMenu.Register(this);


    private static bool IsPropertyTypeSupported(Type type) =>
        type == typeof(string) ||
        type == typeof(int) ||
        type == typeof(float) ||
        type == typeof(bool) ||
        type == typeof(double) ||
        type == typeof(long) ||
        type == typeof(short) ||
        type == typeof(Action);

    private static PropertyInputType InferInputType(Type type) => type switch
    {
        var t when t == typeof(int) => PropertyInputType.NumberBox,
        var t when t == typeof(float) => PropertyInputType.NumberBox,
        var t when t == typeof(double) => PropertyInputType.NumberBox,
        var t when t == typeof(long) => PropertyInputType.NumberBox,
        var t when t == typeof(short) => PropertyInputType.NumberBox,
        var t when t == typeof(bool) => PropertyInputType.Checkbox,
        var t when t == typeof(Action) => PropertyInputType.Button,
        _ => PropertyInputType.TextBox
    };

    //
    //  FIRST ATTEMPT AT CONFIG PROPERTY REGISTRATION USING EXPRESSIONS
    //
    //public ModEntry AddConfigProperty<T>
    //    (Expression<Func<T>> propertyExpression,
    //    string name,
    //    Action<T> onValueChanged = default,
    //    string placeholder = default,
    //    string description = default)
    //{
    //    if (propertyExpression.Body is not MemberExpression member || member.Member is not PropertyInfo propInfo)
    //    {
    //        Melon<BloomEnginePlugin>.Logger.Warning($"[ModMenu] Failed to add config property '{name}' for mod '{DisplayName}': Expression body is not a MemberExpression or isn't a valid property.\nA property access expression must be passed like this: () => obj.SomeProperty");
    //        return this;
    //    }

    //    var targetExpression = member.Expression as ConstantExpression ?? (member.Expression as MemberExpression)?.Expression as ConstantExpression;
    //    object targetObject = targetExpression?.Value ?? Expression.Lambda(member.Expression).Compile().DynamicInvoke();

    //    InputType type = propInfo.PropertyType;
    //    if (!IsPropertyTypeSupported(type))
    //    {
    //        Melon<BloomEnginePlugin>.Logger.Warning($"[ModMenu] Failed to add config property '{name}' for mod '{DisplayName}': Property type '{type.Name}' is not supported.");
    //        return this;
    //    }

    //    // Create typed getter and setter
    //    Func<T> getter = propertyExpression.Compile();
    //    Action<T> setter = default;

    //    if (propInfo.SetMethod is not null && type != typeof(Action))
    //    {
    //        var valueParam = Expression.Parameter(typeof(T), "val");
    //        var setCall = Expression.Call(
    //            Expression.Constant(targetObject),
    //            propInfo.SetMethod,
    //            valueParam
    //        );
    //        setter = Expression.Lambda<Action<T>>(setCall, valueParam).Compile();
    //    }

    //    // Wrap to object delegates for ConfigProperty
    //    Func<object> getterWrapped = () => getter();
    //    Action<object> setterWrapped = setter is not null ? (val => setter((T)val)) : null;
    //    Action<object> onValueChangedWrapped = onValueChanged is not null ? (val => onValueChanged((T)val)) : null;

    //    var property = new ConfigProperty(propInfo.Name, type, getterWrapped, setterWrapped, onValueChangedWrapped, placeholder, description);
    //    Properties.Add(property);

    //    return this;
    //}
}