using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Reflection;
using GlobalEnums;
using Modding;
using Modding.Patches;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using MenuChanger.Extensions;

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
            On.UnityEngine.UI.SaveSlotButton.OnSubmit += SaveSlotButton_OnSubmit;
        }

        private static SaveStats GetSaveStats(this SaveSlotButton button)
        {
            return ReflectionHelper.GetField<SaveSlotButton, SaveStats>(button, "saveStats");
        }

        private static int GetSaveSlotIndex(this SaveSlotButton button)
        {
            return (int)typeof(SaveSlotButton)
                .GetProperty("SaveSlotIndex", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(button);
        }

        private static void SaveSlotButton_OnSubmit(On.UnityEngine.UI.SaveSlotButton.orig_OnSubmit orig, SaveSlotButton self, UnityEngine.EventSystems.BaseEventData eventData)
        {
            if (self.saveFileState == SaveSlotButton.SaveFileStates.LoadedStats
                && self.GetSaveStats().permadeathMode != 2)
            {
                try
                {
                    Settings s = MenuChangerMod.instance.ManuallyLoadSettings<MenuChangerMod, Settings>(self.GetSaveSlotIndex());
                    if (s != null && s.resumeKey != null && ResumePages.TryGetValue(s.resumeKey, out MenuPage page) && page is MenuPage)
                    {
                        UIManager.instance.StartCoroutine(UIManager.instance.HideSaveProfileMenu());
                        UIManager.instance.menuState = MainMenuState.PLAY_MODE_MENU;
                        GameManager.instance.StartCoroutine(LoadGameAndDoAction(self.GetSaveSlotIndex(), page.Show));
                        return;
                    }
                }
                catch (Exception e)
                {
                    MenuChangerMod.instance.LogError($"Unable to manually load settings from menu!\n{e}");
                }
            }
            else if (self.saveFileState == SaveSlotButton.SaveFileStates.Empty)
            {
                GameManager.instance.profileID = self.GetSaveSlotIndex();
                UIManager.instance.UIGoToPlayModeMenu();
                return;
            }

            orig(self, eventData);
        }

        private static IEnumerator LoadGameAndDoAction(int slot, Action a)
        {
            bool finishedLoading = false;
            bool successfullyLoaded = false;
            GameManager.instance.LoadGame(slot, (b) =>
            {
                finishedLoading = true;
                successfullyLoaded = b;
            });
            while (!finishedLoading) yield return null;
            if (!successfullyLoaded)
            {
                UIManager.instance.UIReturnToMainMenu();
                yield break;
            }
            else
            {
                a?.Invoke();
            }
        }

    }
}
