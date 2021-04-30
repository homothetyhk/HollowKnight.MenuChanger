using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Modding;
using Modding.Patches;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using MonoMod;
using MenuChanger.Demos;

namespace MenuChanger
{
    [Serializable]
    public class Settings : ModSettings
    {
        public string resumeKey;
    }

    public class GlobalSettings : ModSettings
    {
        public GenerationSettings DefaultMenuSettings = new GenerationSettings();
        public List<MenuProfile> Profiles = new List<MenuProfile>();
    }

    public class MenuProfile
    {
        public string name;
        public GenerationSettings settings;
        public override string ToString()
        {
            int i = MenuChanger.gs.Profiles.IndexOf(this);
            return i >= 0 ? i.ToString() : string.Empty;
        }

    }

}
