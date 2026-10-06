using BloomEngine.Config.Events;
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
    /// Gets or sets the value stored in this config field, invoking <see cref="Transform"/> when it is changed.
    /// If the new value is different to the old value, the <see cref="ValueChanged"/> event is raised.
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
                    "Set defaultValue when constructing the field, or wait until it has been registered.");
            
            // Transform the incoming value
            var incoming = Transform is not null ? Transform.Invoke(value) : value;
            var old = storedValue;
            
            // If the incoming value is invalid, reset input to the stored value
            if (Validate is not null && !Validate.Invoke(incoming))
            {
                if(InputObjectCreated)
                    UpdateInput();
                return;
            }

            if(InputObjectCreated)
                SetInputValue(incoming);
            
            // If the incoming value is identical, skip saving it
            if (EqualityComparer<T>.Default.Equals(old, incoming))
                return;
            
            storedValue = incoming; 
            MelonEntry.Value = incoming;
            ValueChanged?.Invoke((TSelf)this, new ValueChangedEventArgs<T>(old, incoming));
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
    /// Contains an old name for this field that will be converted to an identifier which MelonPreferences will automatically migrate.
    /// </summary>
    public string? OldName { get; init; }
    
    /// <summary>
    /// An old identifier that will be passed to MelonPreferences to be automatically migrated.
    /// </summary>
    private string? OldIdentifier => OldName is null ? null : GetIdentifierFromName(OldName);
    
    /// <summary>
    /// The <see cref="MelonPreferences_Entry"/> that corresponds to this config field and contains the stored value.
    /// </summary>
    public MelonPreferences_Entry<T>? MelonEntry { get; private set; }
    
    /// <summary>
    /// A function that processes an incoming new value and returns a transformed value.
    /// </summary>
    public Func<T, T>? Transform { get; init; }

    /// <summary>
    /// A function that validates an incoming value and returns true if it should be assigned to <see cref="Value"/>.
    /// The validation check occurs after the new value has been transformed by <see cref="Transform"/>.
    /// </summary>
    public Func<T, bool>? Validate { get; init; }
    
    /// <summary>
    /// An event that is invoked when <see cref="Value"/> is updated, passing the newly set value as an argument.
    /// </summary>
    public event ConfigFieldEventHandler<TSelf, ValueChangedEventArgs<T>>? ValueChanged;
    
    /// <summary>
    /// Adds a handler to an event that is invoked when <see cref="Value"/> is updated, passing the newly set value as an argument.
    /// </summary>
    public ConfigFieldEventHandler<TSelf, ValueChangedEventArgs<T>> OnValueChanged { init => ValueChanged += value; }
    
    /// <summary>
    /// An event that is invoked when the UI input is modified by the user, passing this config field as an argument.
    /// </summary>
    public event ConfigFieldEventHandler<TSelf, InputChangedEventArgs<T>>? InputChanged;
    
    /// <summary>
    /// Adds a handler to an event that is invoked when the UI input is modified by the user, passing this config field as an argument.
    /// </summary>
    public ConfigFieldEventHandler<TSelf, InputChangedEventArgs<T>> OnInputChanged { init => InputChanged += value; }
    
    /// <summary>
    /// Creates a new generically typed config field with an internal identifier, display name and a default value.
    /// </summary>
    /// <param name="name">String literal that is shown on a label next to this field in the config panel.</param>
    /// <param name="defaultValue">A default value that this field initially stores and can be reset to.</param>
    protected ConfigField(string name, T defaultValue) : base(name)
    {
        DefaultValue = defaultValue;
        storedValue = defaultValue;
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
    internal sealed override void ResetInput() => SetInputValue(Transform is null ? DefaultValue : Transform.Invoke(DefaultValue));

    /// <inheritdoc/>
    internal sealed override void UpdateInput() => SetInputValue(Value);

    /// <inheritdoc/>
    internal sealed override GameObject CreateInput(RectTransform parent, string name) => CreateInputObject(parent, name, onInputChanged: val =>
    {
        if (InputChanged is null)
            return;

        var args = new InputChangedEventArgs<T>(val);
        InputChanged.Invoke((TSelf)this, args);

        if (args.Dirty)
            SetInputValue(args.InputValue);
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