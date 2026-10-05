using MelonLoader;
using UnityEngine;
using BloomEngine.UI;

namespace BloomEngine.Config.Fields;

/// <summary>
/// Represents a generic config field with a specifically typed <see cref="Value"/>.
/// </summary>
/// <typeparam name="T">The type of value stored within this config field.</typeparam>
/// <typeparam name="TSelf">The type of this config field.</typeparam>
public abstract class ConfigField<T, TSelf> : ConfigFieldBase
    where T : notnull
    where TSelf : ConfigField<T, TSelf>
{
    /// <summary>
    /// Gets or sets the value stored in this config field, invoking <see cref="transformFunc"/> when it is changed.
    /// If the new value is different to the old value, any handlers added with <see cref="WithOnValueApplied"/>
    /// are invoked and the <see cref="MelonEntry"/> value is updated.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if set before the field has been registered with MelonPreferences.
    /// </exception>
    public T Value
    {
        get => storedValue;
        set
        {
            // If the MelonEntry has not been loaded yet, the field's value should not be set
            if (MelonEntry is null)
                throw new InvalidOperationException(
                    $"Cannot set Value on config field '{Identifier}' before it has been registered. " + 
                    "Set the value in the constructor via defaultValue, or wait until after registration.");
            
            // Transform the incoming value
            var incoming = transformFunc is not null ? transformFunc.Invoke(value) : value;
            
            // If the incoming value is invalid, reset input to the stored value
            if (validateFunc is not null && !validateFunc.Invoke(incoming))
            {
                if(InputObjectCreated)
                    UpdateInput();
                return;
            }

            if(InputObjectCreated)
                SetInputValue(incoming);
            
            // If the incoming value is identical, skip saving it
            if (EqualityComparer<T>.Default.Equals(storedValue, incoming))
                return;
            
            storedValue = incoming; 
            MelonEntry.Value = incoming;
            OnValueApplied?.Invoke(incoming);
        }
    }

    /// <summary>
    /// The underlying field containing the value currently stored by this config field.
    /// </summary>
    private T storedValue;
    
    /// <summary>
    /// The default value of this config field. This is also used as a fallback when an unexpected value is encountered.
    /// </summary>
    public T DefaultValue { get; }

    /// <summary>
    /// Contains an old identifier that MelonPreferences will automatically migrate. Set this using <see cref="WithOldIdentifier"/>.
    /// </summary>
    public string? OldIdentifier { get; private set; }
    
    /// <summary>
    /// The <see cref="MelonPreferences_Entry"/> that corresponds to this config field and contains the stored value.
    /// </summary>
    public MelonPreferences_Entry<T>? MelonEntry { get; private set; }
    
    /// <summary>
    /// A function that processes an incoming new value and returns a transformed value.
    /// </summary>
    private Func<T, T>? transformFunc;

    /// <summary>
    /// A function that validated an incoming new value and returns true if it should be assigned to <see cref="Value"/>.
    /// </summary>
    /// <remarks>The validation check occurs after the new value has been transformed by <see cref="transformFunc"/></remarks>
    private Func<T, bool>? validateFunc;

    /// <summary>
    /// An event that is invoked when <see cref="Value"/> is updated, passing the newly set value as an argument.
    /// </summary>
    private event Action<T>? OnValueApplied;

    /// <summary>
    /// An event that is invoked when the UI input is modified by the user, passing this config field as an argument.
    /// </summary>
    private event Action<InputContext<TSelf, T>>? OnInputChanged;
    
    /// <summary>
    /// Creates a new generically typed config field with an internal identifier, display name and a default value.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    /// <param name="description">The description popup shown for this config field in the config panel.</param>
    protected ConfigField(string name, T defaultValue, string? description = null) : base(name, description)
    {
        DefaultValue = defaultValue;
        storedValue = defaultValue;
    }

    /// <summary>
    /// Specifies an old identifier that will be automatically migrated by MelonPreferences to the current identifier.
    /// This method should be chained immediately after the constructor, as it has no effect after the MelonEntry has been created.
    /// </summary>
    /// <param name="oldIdentifier">The old identifier to be passed to MelonPreferences.</param>
    /// <returns>This config field, with an old identifier that will be passed to MelonPreferences set.</returns>
    public TSelf WithOldIdentifier(string oldIdentifier)
    {
        OldIdentifier = oldIdentifier;
        return (TSelf)this;
    }
    
    /// <summary>
    /// Subscribes to an event which is invoked when <see cref="Value"/> is updated. This can be caused by a developer's code,
    /// setting a new value using the in-game panel, or updating the MelonPreferences files manually.
    /// </summary>
    /// <param name="handler">The action to invoke when the value changes, receiving the new value as a parameter.</param>
    /// <returns>This config field, with an added handler for when this field's stored value is changed.</returns>
    public TSelf WithOnValueApplied(Action<T> handler)
    {
        OnValueApplied += handler;
        return (TSelf)this;
    }

    /// <summary>
    /// Subscribes to an event which is invoked every time the UI input is modified by the user.
    /// Depending on the type of field, the UI input element may be accessed to modify the visible value.
    /// </summary>
    /// <param name="handler">The action to invoke when the UI input is changed by the user, with the new input given.</param>
    /// <returns>This config field, with an added handler for when the user interacts with the UI input object.</returns>
    public TSelf WithOnInputChanged(Action<InputContext<TSelf, T>> handler)
    {
        OnInputChanged += handler;
        return (TSelf)this;
    }

    /// <summary>
    /// Sets a function that transforms an incoming value before it is assigned to <see cref="Value"/>.
    /// Be sure that the validator set using <see cref="WithValidation(Func{T, bool})"/> is able to approve the transformed value.
    /// </summary>
    /// <param name="transform">A function that takes the incoming value and returns the transformed value.</param>
    /// <returns>This config field, with a transform function that is used when setting a new value.</returns>
    public TSelf WithTransform(Func<T, T> transform)
    {
        transformFunc = transform;
        return (TSelf)this;
    }

    /// <summary>
    /// Sets a function that validates an incoming value before it is assigned to <see cref="Value"/>.
    /// Since this validation check occurs after the transform function (added using <see cref="WithTransform(Func{T, T})"/>) runs,
    /// it is important to ensure that the performed transformation can be approved.
    /// </summary>
    /// <param name="validator">A function that returns true if the value should be assigned, or false to reject it.</param>
    /// <returns>This config field, with a validation function that is used to determine whether a new value should be set.</returns>
    public TSelf WithValidation(Func<T, bool> validator)
    {
        validateFunc = validator;
        return (TSelf)this;
    }

    /// <summary>
    /// Gets the value currently held in the UI input object associated with this field.
    /// </summary>
    /// <returns>The displayed value in the input UI.</returns>
    protected abstract T GetInputValue();
    
    /// <summary>
    /// Sets the value currently held in the UI input object associated with this field.
    /// This should be implemented by updating the backing input element without notification.
    /// </summary>
    /// <param name="inputValue">The value to display in the input UI.</param>
    protected abstract void SetInputValue(T inputValue);

    /// <summary>
    /// Creates the UI input object for this config field. <see cref="UIHelper"/> provides useful static methods for creating basic PvZ inputs.
    /// Backing UI elements may be declared non-nullable with <c>= null!;</c>, since this method is guaranteed to run before
    /// <see cref="GetInputValue"/> or <see cref="SetInputValue"/> can be called.
    /// </summary>
    /// <param name="parent">The <see cref="RectTransform"/> under which the UI input object is instantiated.</param>
    /// <param name="name">The name of the created UI input GameObject.</param>
    /// <param name="onInputChanged">A callback that is invoked upon the input object receiving an input changed event, passing in the field itself.</param>
    /// <returns>The created UI input GameObject.</returns>
    protected abstract GameObject CreateInputObject(RectTransform parent, string name, Action<T> onInputChanged);

    /// <inheritdoc/>
    internal sealed override void ApplyInput() => Value = GetInputValue();

    /// <inheritdoc/>
    internal sealed override void ResetInput() => SetInputValue(transformFunc is null ? DefaultValue : transformFunc.Invoke(DefaultValue));

    /// <inheritdoc/>
    internal sealed override void UpdateInput() => SetInputValue(Value);

    /// <inheritdoc/>
    internal sealed override GameObject CreateInput(RectTransform parent, string name) => CreateInputObject(parent, name, onInputChanged: val =>
    {
        if (OnInputChanged is null)
            return;

        var ctx = new InputContext<TSelf, T>((TSelf)this, val);
        OnInputChanged.Invoke(ctx);

        if (ctx.Dirty)
            SetInputValue(ctx.InputValue);
    });

    /// <inheritdoc/>
    internal sealed override void CreateMelonEntry(MelonPreferences_Category melonCategory)
    {
        MelonEntry = melonCategory.CreateEntry(Identifier, DefaultValue, Name, Description, is_hidden: true, oldIdentifier: OldIdentifier);
        MelonEntry.OnEntryValueChanged.Subscribe((_, val) =>
        {
            if (!EqualityComparer<T>.Default.Equals(val, storedValue))
                Value = val;
        });
        
        // Load the value from the MelonEntry, then save it back to sync transformation and validation results
        Value = MelonEntry.Value;
        MelonEntry.Value = storedValue;
    }
}