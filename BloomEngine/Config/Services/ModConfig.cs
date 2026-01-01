using BloomEngine.Config.Inputs;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using MelonLoader;
using System.Reflection;

namespace BloomEngine.Config.Services;

/// <summary>
/// Represents the mod config of a BloomEngine <see cref="ModMenuEntry"/>. When a config is registered,
/// a new MelonPreferences category is created for the mod and the config is saved to it.
/// </summary>
public sealed class ModConfig
{
    /// <summary>
    /// The BloomEngine <see cref="ModMenuEntry"/> to which this config instance belongs.
    /// </summary>
    public ModMenuEntry ModEntry { get; private init; }

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
    internal ModConfig(ModMenuEntry modEntry, BaseConfigInput[] inputs)
    {
        ModEntry = modEntry;
        ConfigInputs = inputs.ToList();

        SetupMelonPreferences();
    }

    /// <summary>
    /// Creates a mod config from a static config class (used by <see cref="ModMenuEntry.AddConfigClass(Type)"/>).
    /// </summary>
    internal ModConfig(ModMenuEntry modEntry, Type staticConfig)
    {
        ModEntry = modEntry;
        ConfigInputs = GetInputsFromStaticClass(staticConfig);

        SetupMelonPreferences();
    }

    /// <summary>
    /// Creates the MelonPreferences category and config entries.
    /// </summary>
    private void SetupMelonPreferences()
    {
        MelonCategory = MelonPreferences.CreateCategory(ModEntry.Identifier, ModEntry.DisplayName);

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
            ConfigService.ConfigLogger.Msg($"Updated mod config for {ModEntry.DisplayName} and saved preferences.");
    }

    /// <summary>
    /// Retrieves all static fields which contain instances of <see cref="BaseConfigInput"/> from a static config class.
    /// </summary>
    /// <param name="configType">The type representing the static config class.</param>
    /// <returns>A list of BaseConfigInput instances found in the static fields of the specified type.</returns>
    private static List<BaseConfigInput> GetInputsFromStaticClass(Type configType)
    {
        List<BaseConfigInput> inputs = new();

        // Use reflection to find all fields and properties that define input fields
        var fields = configType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            // Null instance for static class
            if (field.GetValue(null) is BaseConfigInput inputField)
                inputs.Add(inputField);
        }

        return inputs;
    }
}