using Modding;
using Modding.Patches;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace MenuChanger.Extensions
{
    public static class SaveGameDataExtensions
    {
        public class FakeModSavegameData
        {
            public Dictionary<string, string> loadedMods;
            public Dictionary<string, JToken> modData = new();
        }

        public static T ManuallyLoadSettings<U, T>(this U u, int saveSlot) where U : Mod, ILocalSettings<T>
        {
            FakeModSavegameData moddedData;
            try
            {
                MethodInfo method = typeof(GameManager).GetMethod("ModdedSavePath", BindingFlags.NonPublic | BindingFlags.Static);
                string path = method.Invoke(GameManager.instance, new object[] { saveSlot }) as string;
                if (File.Exists(path))
                {
                    using FileStream fileStream = File.OpenRead(path);
                    using StreamReader streamReader = new(fileStream);
                    string text = streamReader.ReadToEnd();
                    moddedData = JsonConvert.DeserializeObject<FakeModSavegameData>(text, new JsonSerializerSettings
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        Converters = JsonConverterTypes.ConverterTypes
                    });
                    if (moddedData == null)
                    {
                        LogError("Loaded mod savegame data deserialized to null: " + text);
                        moddedData = new FakeModSavegameData();
                    }
                }
                else moddedData = new FakeModSavegameData();
            }
            catch (Exception message)
            {
                LogError(message.ToString());
                moddedData = new FakeModSavegameData();
            }

            try
            {
                if (moddedData.modData.TryGetValue(u.GetName(), out JToken jtoken))
                {
                    return jtoken.ToObject<T>(JsonSerializer.Create(new JsonSerializerSettings
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        Converters = JsonConverterTypes.ConverterTypes
                    }));
                }
            }
            catch (Exception message)
            {
                LogError(message.ToString());
            }

            return default;
        }
    }
}
