using BloomEngine.Config.Inputs.Base;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu;
using MelonLoader;
using System.Reflection;

namespace BloomEngine.Config;

/// <summary>
/// Represents the mod config of a BloomEngine <see cref="ModMenuEntry"/>. When a config is registered,
/// a new MelonPreferences category is created for the mod and the config is saved to it.
/// </summary>
public sealed class ModConfig
{
    /// <summary>
    /// The identifier string of this config, which is used for saving it in MelonPreferences.
    /// This will usually match the identifier of the <see cref="ModMenuEntry"/> this config belongs to.
    /// </summary>
    public string Identifier { get; private init; }

    /// <summary>
    /// The display name of this config, which will be saved in MelonPreferences and shown in the config menu.
    /// This will usually match the display name of the <see cref="ModMenuEntry"/> this config belongs to.
    /// </summary>
    public string DisplayName { get; private init; }

    /// <summary>
    /// A list of all the config inputs contained in this config instance.
    /// </summary>
    public List<BaseConfigInput> ConfigInputs { get; private init; }

    /// <summary>
    /// The <see cref="MelonPreferences"/> category created by this config instance,
    /// to which the config inputs are saved.
    /// </summary>
    public MelonPreferences_Category MelonCategory { get; private set; }

    /// <summary>
    /// Gets the registered input count of this config instance.
    /// </summary>
    public int InputCount => ConfigInputs.Count;

    /// <summary>
    /// Reterns true if this config has zero inputs, meaning it is empty.
    /// </summary>
    public bool IsEmpty => InputCount == 0;

    /// <summary>
    /// The UI panel created for this config.
    /// </summary>
    internal ConfigPanel Panel { get; set; }

    /// <summary>
    /// Creates a mod config from an array of inputs (used by <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>).
    /// </summary>
    internal ModConfig(string identifier, string displayName, BaseConfigInput[] inputs)
    {
        Identifier = identifier;
        DisplayName = displayName;
        ConfigInputs = inputs.ToList();

        SetupMelonPreferences();
    }

    /// <summary>
    /// Creates the MelonPreferences category and config entries.
    /// </summary>
    private void SetupMelonPreferences()
    {
        MelonCategory = MelonPreferences.CreateCategory(Identifier, DisplayName);

        foreach (var input in ConfigInputs)
            input.CreateMelonEntry(MelonCategory);
    }

    /// <summary>
    /// Updates all config inputs to the current values from the UI.
    /// </summary>
    internal void UpdateAllFromUI()
    {
        foreach (var input in ConfigInputs)
            input.UpdateFromUI();
    }

    /// <summary>
    /// Updated the UI with the current stored config values.
    /// </summary>
    internal void RefreshAllUI()
    {
        foreach (var input in ConfigInputs)
            input.RefreshUI();
    }

    /// <summary>
    /// Saves this config category to MelonPreferences with an optional message.
    /// </summary>
    /// <param name="printMessage">Whether to log a message to the console.</param>
    internal void Save(bool printMessage)
    {
        MelonCategory.SaveToFile(false);

        if (printMessage)
            ConfigService.ConfigLogger.Msg($"Updated mod config for {DisplayName} and saved MelonPreferences.");
    }
}