using BloomEngine.Config.Inputs;
using BloomEngine.Config.UI;
using BloomEngine.ModMenu.Services;
using MelonLoader;
using System.Reflection;

namespace BloomEngine.Config.Services;

public class ModConfig
{
    public ModMenuEntry ModEntry { get; private init; }

    public List<BaseConfigInput> ConfigInputs { get; private init; }
    public MelonPreferences_Category MelonCategory { get; private init; }

    public int InputCount => ConfigInputs.Count;
    public bool IsEmpty => InputCount == 0;

    internal ConfigPanel Panel { get; set; }

    /// <summary>
    /// Creates a mod config from an array of inputs (used by <see cref="ModMenuEntry.AddConfigInputs(BaseConfigInput[])"/>).
    /// </summary>
    internal ModConfig(ModMenuEntry modEntry, BaseConfigInput[] inputs)
    {
        ModEntry = modEntry;
        MelonCategory = MelonPreferences.CreateCategory(modEntry.Identifier, modEntry.DisplayName);
        ConfigInputs = inputs.ToList();

        foreach (var input in ConfigInputs)
            input.CreateMelonEntry(MelonCategory);
    }

    /// <summary>
    /// Creates a mod config from a static config class (used by <see cref="ModMenuEntry.AddConfigClass(Type)"/>).
    /// </summary>
    internal ModConfig(ModMenuEntry modEntry, Type staticConfig)
    {
        ModEntry = modEntry;
        MelonCategory = MelonPreferences.CreateCategory(modEntry.Identifier, modEntry.DisplayName);
        ConfigInputs = GetInputsFromStaticClass(staticConfig);

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

    internal void SaveConfig() => ConfigService.SaveModConfig(this, true);

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