using BloomEngine.Config.Internal;
using BloomEngine.ModMenu.Services;
using MelonLoader;
using System.Reflection;

namespace BloomEngine.Config.Services;

public class ModConfig
{
    public ModMenuEntry ModMenuEntry { get; private init; }

    public List<BaseConfigInput> ConfigInputs { get; private init; }
    public MelonPreferences_Category MelonCategory { get; private init; }

    public int InputCount => ConfigInputs.Count;
    public bool IsEmpty => InputCount == 0;

    internal ModConfig(ModMenuEntry modMenuEntry, Type staticConfigType)
    {
        ModMenuEntry = modMenuEntry;
        MelonCategory = MelonPreferences.CreateCategory(modMenuEntry.Identifier, modMenuEntry.DisplayName);
        ConfigInputs = GetInputsFromStaticClass(staticConfigType);

        foreach(var input in ConfigInputs)
            input.CreateMelonEntry(MelonCategory);
    }

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