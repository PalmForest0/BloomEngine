using BloomEngine.Config.Inputs;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using MelonLoader;
using System.Reflection;

namespace BloomEngine.Config.Services;

public sealed class ModConfig
{
    public ModMenuEntry ModEntry { get; private init; }

    public List<BaseConfigInput> ConfigInputs { get; private init; }
    public MelonPreferences_Category MelonCategory { get; private set; }

    public int InputCount => ConfigInputs.Count;
    public bool IsEmpty => InputCount == 0;

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

    private void SetupMelonPreferences()
    {
        MelonCategory = MelonPreferences.CreateCategory(ModEntry.Identifier, ModEntry.DisplayName);

        foreach (var input in ConfigInputs)
            input.CreateMelonEntry(MelonCategory);
    }


    public void ShowPanel() => Panel.ShowPanel();
    public void HidePanel() => Panel.HidePanel();


    internal void UpdateAllFromUI()
    {
        foreach (var input in ConfigInputs)
            input.UpdateFromUI();
    }

    internal void RefreshAllUI()
    {
        foreach (var input in ConfigInputs)
            input.RefreshUI();
    }

    internal void SaveConfig(bool printMessage) => ConfigService.SaveModConfig(this, printMessage);

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