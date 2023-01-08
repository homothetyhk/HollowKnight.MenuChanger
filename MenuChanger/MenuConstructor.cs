using MenuChanger.MenuElements;

namespace MenuChanger
{
    /// <summary>
    /// Base class for inserting custom mode menus.
    /// </summary>
    public abstract class ModeMenuConstructor
    {
        /// <summary>
        /// Called during UIManager.EditUI to allow the menu to be built.
        /// </summary>
        public abstract void OnEnterMainMenu(MenuPage modeMenu);
        /// <summary>
        /// Called during activeSceneChanged away from Menu_Title to allow any menu resources to be disposed.
        /// </summary>
        public abstract void OnExitMainMenu();
        /// <summary>
        /// Called after OnEnterMainMenu. Returns true if the out parameter should be used as a button in the mode menu.
        /// </summary>
        public abstract bool TryGetModeButton(MenuPage modeMenu, out BigButton button);
    }

    /// <summary>
    /// Base class for inserting custom resume menus.
    /// </summary>
    public abstract class ResumeMenuConstructor
    {
        /// <summary>
        /// Called during UIManager.EditUI to allow the menu to be built.
        /// </summary>
        public abstract void OnEnterMainMenu();
        /// <summary>
        /// Called during activeSceneChanged away from Menu_Title to allow any menu resources to be disposed.
        /// </summary>
        public abstract void OnExitMainMenu();
    }
}
