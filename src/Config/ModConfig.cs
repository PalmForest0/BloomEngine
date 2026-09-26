using BloomEngine.Config.Fields.Base;
using BloomEngine.Config.UI;
using BloomEngine.Core;
using BloomEngine.ModList;
using MelonLoader;

namespace BloomEngine.Config;

/// <summary>
/// Represents the mod config of a BloomEngine <see cref="ModListEntry"/>. When a config is registered,
/// a new MelonPreferences category is created for the mod and this config is bound to it.
/// </summary>
public sealed class ModConfig
{
    /// <summary>
    /// The identifier string of this config, which is used for saving it in MelonPreferences.
    /// This will usually match the identifier of the <see cref="ModListEntry"/> this config belongs to.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The display name of this config, which will be saved in MelonPreferences and shown in the config panel.
    /// This will usually match the display name of the <see cref="ModListEntry"/> this config belongs to.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// A list of all config fields contained in this config instance.
    /// </summary>
    public List<BaseConfigField> ConfigFields { get; }

    /// <summary>
    /// The <see cref="MelonPreferences"/> category created by this config instance, to which the config fields are saved.
    /// </summary>
    public MelonPreferences_Category MelonCategory { get; private set; } = null!;

    /// <summary>
    /// Gets the amount of registered fields in this config instance.
    /// </summary>
    public int FieldCount => ConfigFields.Count;

    /// <summary>
    /// Returns true if this config has zero fields, meaning it is empty.
    /// </summary>
    public bool IsEmpty => FieldCount == 0;

    /// <summary>
    /// The UI panel created for this config.
    /// </summary>
    internal ConfigPanel? Panel { get; set; }

    /// <summary>
    /// Creates a mod config from an array of fields (used by <see cref="ModListEntry.AddConfigFields"/>).
    /// </summary>
    internal ModConfig(string identifier, string displayName, BaseConfigField[] fields)
    {
        Id = identifier;
        DisplayName = displayName;
        ConfigFields = fields.ToList();

        SetupMelonPreferences();
    }

    /// <summary>
    /// Creates the MelonPreferences category and MelonEntries for each config field.
    /// </summary>
    private void SetupMelonPreferences()
    {
        MelonCategory = MelonPreferences.CreateCategory(Id, DisplayName);

        foreach (var field in ConfigFields)
            field.CreateMelonEntry(MelonCategory);
    }

    /// <summary>
    /// Updates all stored config field values to contain the current values from their corresponding UI input objects.
    /// </summary>
    internal void ApplyInputAll()
    {
        foreach (var field in ConfigFields)
            field.ApplyInput();
    }

    /// <summary>
    /// Updates all UI input objects to display the values currently stored by their corresponding config fields.
    /// </summary>
    internal void RefreshInputAll()
    {
        foreach (var field in ConfigFields)
            field.RefreshInput();
    }

    /// <summary>
    /// Saves this config category to MelonPreferences with an optional message.
    /// </summary>
    /// <param name="printMessage">Whether to log a message to the console.</param>
    internal void Save(bool printMessage)
    {
        MelonCategory.SaveToFile(false);

        if (printMessage)
            BloomLogger.Info($"Updated mod config for {DisplayName} and saved MelonPreferences.", ConfigService.LogPrefix);
    }
}