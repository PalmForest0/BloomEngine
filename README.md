[![](https://gamebanana.com/mods/embeddables/640948?type=large)](https://gamebanana.com/mods/640948)

# BloomEngine
[![GameBanana](https://img.shields.io/badge/GameBanana-Visit-orange?logo=gamebanana&logoColor=white)](https://gamebanana.com/mods/640948)
[![Discord](https://img.shields.io/badge/Discord-Join-5865F2?logo=discord&logoColor=white)](https://discord.gg/UfBMKTHN5b)
[![GitHub tag](https://img.shields.io/github/v/tag/PalmForest0/BloomEngine?color=blue&logo=github)](https://github.com/PalmForest0/BloomEngine/releases)

## What is BloomEngine?
BloomEngine is a [MelonLoader](https://github.com/LavaGang/MelonLoader) mod for PvZ Replanted, which adds an in-game mod menu and config manager.
It is also a modding library that makes creating mods for PvZ Replanted easier by providing a variety of utilities and extensions.
To use the BloomEngine mod, first install MelonLoader in your game's directory. Then download `BloomEngine.dll` from the [releases section](https://github.com/PalmForest0/BloomEngine/releases) and place it in the `Mods` folder.

## Developer Usage

### 1. Referencing BloomEngine
Download both `BloomEngine.dll` and `BloomEngine.xml` from the latest release version, found in the [releases section](https://github.com/PalmForest0/BloomEngine/releases).
Place both files somewhere in your mod project and add a reference to `BloomEngine.dll`. As long as the xml file is in the same folder, it will load automatically.

> [!NOTE]
> While downloading `BloomEngine.xml` is technically optional, it is required for library documentation to be visible in your IDE.

### 2. Registering your mod
After referencing BloomEngine, you can now add you mod to the in-game mod menu like this:

```cs
using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using MelonLoader;

namespace BloomEngine;

internal sealed class BloomEngineMod : MelonMod
{
    public const string Name = "BloomEngine";
    public const string Version = "0.3.0-beta";
    public const string Author = "PalmForest";

    public override void OnInitializeMelon()
    {
        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Mod menu and config manager library for PvZ Replanted.")
            .AddIcon(AssetHelper.LoadSprite("BloomEngine.Resources.BloomEngineIcon.png"))
            .Register();
    }
}
```

### 3. Creating config inputs
To create config inputs for your mod, you can use the methods provided by `ConfigService`. BloomEngine currently supports the following types: `string`, `int`, `float`, `bool` and `enum`.

Here is an example definition of a simple config input that sets a **display name**, **description** and **default value**.

```cs
public static BoolConfigInput TestBoolInput = ConfigService.CreateBool("Test Bool", "Cool description.", true);
```

Additionally, you can extend your config inputs with additional functionality through actions and function, for example:

```cs
public static StringConfigInput TestStringInput = ConfigService.CreateString("Test String", "Cooler description.", "ABCDEFG", new()
{
    OnValueChanged = val => Melon<BloomEngine>.Logger.Msg($"Value of {nameof(TestStringInput)} updated to \"{val}\""),
    OnInputChanged = () => StringInput.Textbox.SetTextWithoutNotify(StringInput.Textbox.text.ToUpperInvariant()),
    TransformValue = val => val.ToUpperInvariant(),
    ValidateValue = val => !string.IsNullOrWhiteSpace(val)
});
```

> [!NOTE]
> Using `MelonLogger.Msg()` within one of the above actions will show `[BloomEngine]` as the source in the MelonLoader console.
> Instead, it is recommended you use `Melon<YourMainModClass>.Logger.Msg()` or create a custom logger using `new MelonLogger.Instance()`.

### 4. Registering the config
Finally, to add the config inputs you just created to your mod's config, you must pass them to `AddConfigInputs(StringInput, BoolInput)` when registering your mod:

```cs
ModMenuService.CreateEntry(this)
    .AddDisplayName(Name)
    .AddDescription($"Mod menu and config manager library for PvZ Replanted.")
    .AddIcon(AssetHelper.LoadSprite("BloomEngine.Resources.BloomEngineIcon.png"))
    .AddConfigInputs(TestStringInput, TestBoolInput)
    .Register();
```

# Screenshots
<img width="1919" height="1079" alt="Screenshot_1" src="https://github.com/user-attachments/assets/73a0519e-0565-4cab-bf76-d2a7a2437d04" />
<img width="1919" height="1079" alt="Screenshot_2" src="https://github.com/user-attachments/assets/480d3ee1-0afb-44f8-8cc1-a43ddcb397f9" />
