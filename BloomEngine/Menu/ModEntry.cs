using BloomEngine.Menu.Config;
using MelonLoader;
using UnityEngine;

namespace BloomEngine.Menu;

public class ModEntry
{
    public string Id { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }

    public Texture2D Image { get; private set; }
    public MelonMod Mod { get; private set; }

    public ModConfigBase Config { get; private set; }

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

    public void Register() => ModMenu.Register(this);

    public ModEntry AddConfig(ModConfigBase configInstance)
    {
        Config = configInstance;
        return this;
    }

    //public ModEntry AddConfigProperty<T>
    //    (Expression<Func<T>> propertyExpression,
    //    string name,
    //    Func<T, T> onValueChanged = default,
    //    string placeholder = default,
    //    string description = default,
    //    PropertyInputType inputType = PropertyInputType.Auto)
    //{
    //    if (propertyExpression.Body is not MemberExpression member || member.Member is not PropertyInfo propInfo)
    //    {
    //        Melon<BloomEnginePlugin>.Logger.Warning($"Failed to add config property '{name}' for mod '{DisplayName}': Expression body is not a MemberExpression or isn't a valid property.\nA property access expression must be passed like this: () => obj.SomeProperty");
    //        return this;
    //    }

    //    var targetExpression = member.Expression as ConstantExpression ?? (member.Expression as MemberExpression)?.Expression as ConstantExpression;
    //    object targetObject = targetExpression?.Value ?? Expression.Lambda(member.Expression).Compile().DynamicInvoke();

    //    Type type = propInfo.PropertyType;
    //    if (!IsPropertyTypeSupported(type))
    //    {
    //        Melon<BloomEnginePlugin>.Logger.Warning($"Failed to add config property '{name}' for mod '{DisplayName}': Property type '{type.Name}' is not supported.");
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
    //    Func<object, object> onValueChangedWrapped = onValueChanged is not null ? (val => onValueChanged((T)val)) : null;

    //    if (inputType == PropertyInputType.Auto)
    //        inputType = InferInputType(type);

    //    var property = new ConfigProperty(propInfo.Name, type, getterWrapped, setterWrapped, placeholder, description, onValueChangedWrapped, inputType);
    //    Properties.Add(property);

    //    return this;
    //}
}