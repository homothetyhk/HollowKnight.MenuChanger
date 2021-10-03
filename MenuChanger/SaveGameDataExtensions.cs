using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Modding;
using Modding.Patches;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MenuChanger
{
    public static class SaveGameDataExtensions
    {
		public class FakeModSavegameData
        {
			public Dictionary<string, string> loadedMods;
			public Dictionary<string, JToken> modData = new Dictionary<string, JToken>();
        }

        public static T ManuallyLoadSettings<U, T>(this U u, int saveSlot) where U : Mod, ILocalSettings<T>
        {
			FakeModSavegameData moddedData;
			try
			{
				MethodInfo method = typeof(GameManager).GetMethod("ModdedSavePath", BindingFlags.NonPublic | BindingFlags.Static);
				if (method == null) MenuChangerMod.instance.Log("Unable to find ModdedSavePath!");
				string path = method
					.Invoke(GameManager.instance, new object[] { saveSlot }) as string;
				if (File.Exists(path))
				{
					using (FileStream fileStream = File.OpenRead(path))
					{
						using (StreamReader streamReader = new StreamReader(fileStream))
						{
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
								Logger.LogError("Loaded mod savegame data deserialized to null: " + text);
								moddedData = new FakeModSavegameData();
							}
						}
					}
				}
				else moddedData = new FakeModSavegameData();
			}
			catch (Exception message)
			{
				Logger.LogError(message);
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
				Logger.LogError(message);
			}

			return default;
		}
    }
}
