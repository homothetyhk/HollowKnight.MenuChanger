using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using Modding.Patches;
using Newtonsoft.Json;

namespace MenuChanger
{
    public static class SaveGameDataExtensions
    {
        public static bool TryGetModSettings<U, T>(this SaveGameData data, out T settings) where U : Mod where T : ModSettings
        {
            bool exists = data.PolymorphicModData.TryGetValue(typeof(U).Name, out string json);
            settings = default;

            if (exists)
            {
                try
                {
                    settings = (T)JsonConvert.DeserializeObject(json, typeof(T),
                        new JsonSerializerSettings
                        {
                            ContractResolver = ShouldSerializeContractResolver.Instance,
                            TypeNameHandling = TypeNameHandling.Auto,
                            ObjectCreationHandling = ObjectCreationHandling.Replace,
                            Converters = JsonConverterTypes.ConverterTypes
                        });
                }
                catch (Exception e)
                {
                    MenuChanger.instance.LogWarn("Error in TryGetModSettings:\n" + e);
                }
            }
            
            return exists;
        }
    }
}
