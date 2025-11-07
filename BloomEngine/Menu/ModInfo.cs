using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomEngine.Menu;

internal struct ModInfo
{
    public MelonMod Mod;
    public ModConfig Config;

    public ModInfo(MelonMod mod, ModConfig config)
    {
        Mod = mod;
        Config = config;
    }
}
