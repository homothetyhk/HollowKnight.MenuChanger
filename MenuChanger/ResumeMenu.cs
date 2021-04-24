using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GlobalEnums;
using Modding;
using Modding.Patches;
using Newtonsoft.Json;

namespace MenuChanger
{
    public static class ResumeMenu
    {
        private static Dictionary<string, MenuPage> ResumePages = new Dictionary<string, MenuPage>();
        private static Dictionary<int, SaveGameData> SaveGameData = new Dictionary<int, SaveGameData>();

        public static void AddResumePage(string key, MenuPage page)
        {
            ResumePages[key] = page;
        }


        internal static void Reset()
        {
            ResumePages.Clear();
            SaveGameData.Clear();
        }

        internal static void Hook()
        {
            On.GameManager.LoadGameFromUI += OnLoadGameFromUI;
            On.GameManager.GetSaveStatsForSlot += OnGetSaveStatsForSlot;
        }

        internal static void OnLoadGameFromUI
            (On.GameManager.orig_LoadGameFromUI orig, GameManager self, int saveSlot)
        {
            if (TryGetResumePage(saveSlot, out MenuPage page) && page is MenuPage)
            {
                UIManager.instance.StartCoroutine(UIManager.instance.HideSaveProfileMenu());
                UIManager.instance.menuState = MainMenuState.PLAY_MODE_MENU;
                page.Show();
                return;
            }

            orig(self, saveSlot);
        }

        internal static void OnGetSaveStatsForSlot
            (On.GameManager.orig_GetSaveStatsForSlot orig, GameManager self, int saveSlot, Action<SaveStats> callback)
        {
            RecordSaveGameData(saveSlot);
            orig(self, saveSlot, callback);
        }

        internal static bool TryGetResumePage(int saveSlot, out MenuPage page)
        {
            page = null;
            return SaveGameData.TryGetValue(saveSlot, out SaveGameData data)
                && data.TryGetModSettings<MenuChanger, Settings>(out Settings settings)
                && ResumePages.TryGetValue(settings.resumeKey, out page);
        }

        internal static void RecordSaveGameData(int id)
        {
            if (!Platform.IsSaveSlotIndexValid(id))
            {
                return;
            }

            Platform.Current.ReadSaveSlot(id, fileBytes =>
            {
                if (fileBytes == null) return;

                string json;
                if (GameManager.instance.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    MemoryStream serializationStream = new MemoryStream(fileBytes);
                    string encryptedString = (string)binaryFormatter.Deserialize(serializationStream);
                    json = Encryption.Decrypt(encryptedString);
                }
                else
                {
                    json = Encoding.UTF8.GetString(fileBytes);
                }
                try
                {
                    ResumeMenu.SaveGameData[id] = JsonConvert.DeserializeObject<SaveGameData>(json, new JsonSerializerSettings()
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        Converters = JsonConverterTypes.ConverterTypes
                    });
                }
                catch
                {
                    MenuChanger.instance.LogWarn($"Unable to read save data for slot {id}");
                }
            });
        }
    }
}
