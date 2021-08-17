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

namespace MenuChanger
{
    [Serializable]
    public class Settings
    {
        public string resumeKey;
    }
}
