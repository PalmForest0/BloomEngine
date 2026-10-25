namespace BloomEngine.Config.Events;

/// <summary>
/// Provides a typed event handler which passes a specifically typed config field as the sender.
/// </summary>
/// <typeparam name="TField">The type of config field sending this event.</typeparam>
/// <typeparam name="TEventArgs">The type of event args provided by this event.</typeparam>
public delegate void ConfigFieldEventHandler<in TField, in TEventArgs>(TField field, TEventArgs e);