using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections;
using GlobalEnums;
using MenuChanger.MenuPanels;
using MenuChanger.MenuElements;
//using MenuChanger.Demos;

namespace MenuChanger
{
    public class MenuChangerMod : Mod, ILocalSettings<Settings> //,IGlobalSettings<GlobalSettings>,
    {
        internal static MenuChangerMod instance;
        internal static List<MenuPage> displayedPages = new List<MenuPage>();

        public static void HideAllMenuPages()
        {
            while (displayedPages.FirstOrDefault() is MenuPage page) page.Hide();
        }

        private void EditUI(Scene from, Scene to)
        {
            if (to.name == "Menu_Title") EditUI();
        }

        private void EditUI()
        {
            ResumeMenu.Reset();
            ModeMenu.OnEnterMainMenu();
        }

        public override int LoadPriority() => -10;

        public override void Initialize()
        {
            instance = this;
            ThreadSupport.Setup();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += EditUI;
            SpriteManager.LoadEmbeddedPngs("MenuChanger.Resources.");
            PrefabMenuObjects.Setup();
            EditUI();
            
            On.UIManager.UIGoToProfileMenu += (o, s) =>
            {
                o(s);
                HideAllMenuPages();
            };
            On.UIManager.UIGoToPlayModeMenu += (o, s) => 
            {
                s.StartCoroutine(s.HideSaveProfileMenu());
                s.menuState = MainMenuState.PLAY_MODE_MENU;

                ModeMenu.Show();
            };

            ResumeMenu.Hook();
        }

        public Settings Settings = new Settings();
        //public static GlobalSettings gs = new GlobalSettings();

        public int MakeAssemblyHash()
        {
            SHA1 sha1 = SHA1.Create();
            FileStream stream = File.OpenRead(Assembly.GetExecutingAssembly().Location);
            byte[] hash = sha1.ComputeHash(stream).ToArray();
            stream.Dispose();
            sha1.Clear();

            unchecked
            {
                int val = 0;
                for (int i = 0; i < hash.Length - 1; i += 4)
                {
                    val = 17 * val + 31 * BitConverter.ToInt32(hash, i);
                }
                return val;
            }
        }

        public override string GetVersion()
        {
            return $"1.0 ({ Math.Abs(MakeAssemblyHash() % 997)})";
        }

        public void OnLoadLocal(Settings s)
        {
            Settings = s;
        }

        public Settings OnSaveLocal()
        {
            return Settings;
        }

        /*
        public void OnLoadGlobal(GlobalSettings s)
        {
            gs = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return gs;
        }
        */
    }
}
