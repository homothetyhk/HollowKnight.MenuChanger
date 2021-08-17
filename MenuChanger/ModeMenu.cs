using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;

namespace MenuChanger
{
    public static class ModeMenu
    {
        private static MenuPage ModePage;
        private static MultiGridItemPanel ModeButtonPanel;

        private static List<ModeMenuConstructor> constructors = new List<ModeMenuConstructor>
        {
            new DefaultModeConstructor(Mode.Classic),
            new DefaultModeConstructor(Mode.Steel),
            new DefaultModeConstructor(Mode.Godmaster),
        };

        internal static void OnEnterMainMenu()
        {
            MenuChangerMod.instance.Log("Constructing mode menu...");
            ModePage = new MenuPage("Mode Page");
            List<BigButton> buttons = new List<BigButton>();

            for (int i = 0; i < constructors.Count; i++)
            {
                try
                {
                    constructors[i].OnEnterMainMenu(ModePage);
                    buttons.Add(constructors[i].GetModeButton(ModePage));
                    MenuChangerMod.instance.Log($"  Constructed menu from {constructors[i].GetType().AssemblyQualifiedName}");
                }
                catch (Exception e)
                {
                    MenuChangerMod.instance.LogError($"Error during OnEnterMainMenu for constructor of type {constructors[i].GetType().AssemblyQualifiedName}:\n{e}");
                    constructors.RemoveAt(i--);
                    continue;
                }
            }
            ModeButtonPanel = new MultiGridItemPanel(ModePage, 5, 3, 150f, 650f, new Vector2(0, 300), buttons.ToArray());
        }

        internal static void Show()
        {
            ModePage.Show();
        }

        internal static void OnExitMainMenu()
        {
            ModePage = null;
            ModeButtonPanel = null;
            foreach (var c in constructors)
            {
                try
                {
                    c.OnExitMainMenu();
                }
                catch (Exception e)
                {
                    MenuChangerMod.instance.LogError($"Error during OnEnterMainMenu for constructor of type {c.GetType().Name}:\n{e}");
                } 
            }
        }

        public static void AddMode(ModeMenuConstructor constructor)
        {
            constructors.Add(constructor);
        }

        private class DefaultModeConstructor : ModeMenuConstructor
        {
            readonly Mode mode;
            public DefaultModeConstructor(Mode mode)
            {
                this.mode = mode;
            }

            private BigButton button;

            public override void OnEnterMainMenu(MenuPage modeMenu) 
            {
                button = new BigButton(modeMenu, mode);
                button.OnClick += () =>
                {
                    MenuChangerMod.HideAllMenuPages();
                    UIManager.instance.StartNewGame(permaDeath: mode == Mode.Steel, bossRush: mode == Mode.Godmaster);
                };
            }
            public override BigButton GetModeButton(MenuPage modeMenu)
            {
                return button;
            }
            public override void OnExitMainMenu()
            {
                button = null;
            }
        }
    }
}
