using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuChanger.MenuElements;
using UnityEngine;

namespace MenuChanger
{
    public abstract class ModeMenuConstructor
    {
        public abstract void OnEnterMainMenu(MenuPage modeMenu);
        public abstract void OnExitMainMenu();
        public abstract BigButton GetModeButton(MenuPage modeMenu);
    }

    public abstract class ResumeMenuConstructor
    {
        public abstract void OnEnterMainMenu();
        public abstract void OnExitMainMenu();
    }
}
