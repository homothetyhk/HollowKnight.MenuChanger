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
using MenuChanger.MenuElements;

namespace MenuChanger
{
    public static class ResumeMenu
    {
        private static readonly Dictionary<string, MenuPage> _resumePages = new Dictionary<string, MenuPage>();
        private static MenuPage transitionPage;
        private static MenuLabel unloadingLabel;
        private static MenuLabel loadingLabel;
        private static bool insideResumeMenu = false;


        public static void AddResumePage(string key, MenuPage page)
        {
            _resumePages[key] = page;
        }

        internal static void Reset()
        {
            _resumePages.Clear();
            transitionPage = new MenuPage("ResumeMenu Transition Page");
            transitionPage.backButton.Destroy();
            loadingLabel = new MenuLabel(transitionPage, "Please wait while save data loads...");
            loadingLabel.Hide();
            unloadingLabel = new MenuLabel(transitionPage, "Please wait while save data unloads...");
            unloadingLabel.Hide();
            insideResumeMenu = false;
        }

        internal static void Hook()
        {
            On.UnityEngine.UI.SaveSlotButton.OnSubmit += SaveSlotButton_OnSubmit;
            On.UIManager.UIGoToProfileMenu += OnUIGoToProfileMenu;
        }

        private static void OnUIGoToProfileMenu(On.UIManager.orig_UIGoToProfileMenu orig, UIManager self)
        {
            if (insideResumeMenu)
            {
                InputHandler.Instance.StopUIInput();
                MenuChangerMod.HideAllMenuPages();
                transitionPage.Show();
                unloadingLabel.Show();
                self.StartCoroutine(GameManager.instance.ReturnToMainMenu(GameManager.ReturnToMainMenuSaveModes.DontSave));
            }
            else orig(self);
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

        private static IEnumerator GoToResumeMenu(UIManager s, SaveSlotButton button, MenuPage resumePage)
        {
            InputHandler.Instance.StopUIInput();
            yield return s.HideSaveProfileMenu();
            ReflectionHelper.CallMethod(s, "SetMenuState", MainMenuState.PLAY_MODE_MENU);
            transitionPage.Show();
            loadingLabel.Show();
            yield return null;
            yield return LoadGameAndDoAction(button.GetSaveSlotIndex(), () =>
             {
                 insideResumeMenu = true;
                 loadingLabel.Hide();
                 transitionPage.Hide();
                 InputHandler.Instance.StartUIInput();
                 resumePage.Show();
             });
        }

        private static void SaveSlotButton_OnSubmit(On.UnityEngine.UI.SaveSlotButton.orig_OnSubmit orig, SaveSlotButton self, UnityEngine.EventSystems.BaseEventData eventData)
        {
            if (self.saveFileState == SaveSlotButton.SaveFileStates.LoadedStats
                && self.GetSaveStats().permadeathMode != 2)
            {
                try
                {
                    Settings s = MenuChangerMod.instance.ManuallyLoadSettings<MenuChangerMod, Settings>(self.GetSaveSlotIndex());
                    if (s != null && s.resumeKey != null && _resumePages.TryGetValue(s.resumeKey, out MenuPage page) && page is MenuPage)
                    {
                        self.ForceDeselect();
                        UIManager.instance.StartCoroutine(GoToResumeMenu(UIManager.instance, self, page));
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
                self.ForceDeselect();
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
