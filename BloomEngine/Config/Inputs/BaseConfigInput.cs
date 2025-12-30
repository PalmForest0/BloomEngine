using MelonLoader;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

public abstract class BaseConfigInput
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Type ValueType { get; protected set; }

    protected string InputObjectName => $"ConfigInput_{Name.Trim().Replace(" ", "")}";
    internal abstract GameObject CreateInputObject(RectTransform parent);

    internal abstract void CreateMelonEntry(MelonPreferences_Category melonCategory);

    internal abstract void UpdateFromUI();
    internal abstract void RefreshUI();
    internal abstract void OnUIChanged();
}