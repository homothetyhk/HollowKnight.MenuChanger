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
        public static MenuPage ModePage;
        private static MultiGridItemPanel ModeButtonPanel;

        internal static void Reset()
        {
            ModePage = MenuPage.Create("Mode Page");

            ModeButtonPanel = new MultiGridItemPanel(ModePage, 5, 3, 150f, 650f, new Vector2(0, 300),
                new BigButton(ModePage, Mode.Classic),
                new BigButton(ModePage, Mode.Steel),
                new BigButton(ModePage, Mode.Godmaster));
        }

        public static void AddButtonToModeSelect(BigButton button)
        {
            if (button.Parent != ModePage) return;

            ModeButtonPanel.Add(button);
        }
    }
}
