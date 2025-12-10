# Setup
To register your mod with the BloomEngine mod menu, you must first reference it.

### 1. Download the latest version
Go the releases section and download the latest `.dll` and `.xml` files. 
Once you've done that, place these two files somewhere in your mod's project directory (eg. a `References` folder).
The `.dll` file is required for you to access the code and the `.xml` file contains the documentation for it.

### 2. Reference the library
Open your mod's `.csproj` file and add a reference to the newly added library:

```xml
<ItemGroup>
  <Reference Include="BloomEngine">
    <HintPath>References\BloomEngine.dll</HintPath>
    <Private>false</Private>
  </Reference>
</ItemGroup>
```
- This snippet should be between the two `<Project></Project>` tags and outside of all other tags.
- Make sure that the `HintPath` points to the right location for you.

### 3. Register your mod
Finally, you can use the BloomEngine library to register your mod with the mod menu like this:

```cs
using BloomEngine.Menu;
using MelonLoader;
using PvZEnhanced;

[assembly: MelonInfo(typeof(PvZEnhancedMod), PvZEnhancedMod.Name, PvZEnhancedMod.Version, PvZEnhancedMod.Author)]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: MelonOptionalDependencies("BloomEngine")]

namespace PvZEnhanced;

public class PvZEnhancedMod : MelonMod
{
    public const string Name = "PvZ Enhanced";
    public const string Version = "1.0.0";
    public const string Author = "PalmForest";

    public override void OnInitializeMelon()
    {
        ModMenu.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription("test description that will hopefully span across multiple lines.")
            .AddConfig(typeof(Config))
            .Register();
    }
}
```

### 4. Add a config input field
To add an input field to your mod's config, create a static class which looks something like this:

```cs
public static class Config
{
    public static readonly StringInputField StringInput = ConfigMenu.CreateStringInput(
            nameof(StringInput),
            "Default string value.",
            onValueChanged: val => MelonLogger.Msg($"Value of {nameof(StringInput)} updated to \"{val}\""),
            onInputChanged: () => StringInput.Textbox.SetTextWithoutNotify(StringInput.Textbox.text.ToUpperInvariant()),
            transformValue: val => val.Replace("A", "E"),
            validateValue: val => !string.IsNullOrWhiteSpace(val)
        );
}
```

The supported types are `string`, `int`, `float`, `bool` and `enum`. You can create input fields of each type using the other methods provided by the static `ConfigMenu` class.

# Usage
Now, when your mod is used with BloomEngine, it will be registered in the mod menu and your config will be added.

# Screenshots
<img width="1919" height="1079" alt="Screenshot_1" src="https://github.com/user-attachments/assets/81355577-92e1-440b-9760-37576db14a89" />
<img width="1919" height="1079" alt="Screenshot_2" src="https://github.com/user-attachments/assets/37acae08-77aa-406f-bcf7-28eb4f3c74ab" />
<img width="1919" height="1079" alt="Screenshot_3" src="https://github.com/user-attachments/assets/949f843a-85cd-4454-a52f-842d1bad1ce8" />
