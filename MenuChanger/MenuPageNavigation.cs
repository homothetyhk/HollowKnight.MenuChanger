namespace MenuChanger
{
    /// <summary>
    /// Class used to handle navigation between root level elements of a MenuPage.
    /// </summary>
    public abstract class MenuPageNavigation
    {
        public MenuPageNavigation(MenuPage page)
        {
            Page = page;
        }

        public MenuPage Page { get; }

        /// <summary>
        /// Adds the ISelectable to navigation control.
        /// </summary>
        public abstract void Add(ISelectable selectable);
        /// <summary>
        /// Removes the ISelectable from navigation control.
        /// </summary>
        public abstract void Remove(ISelectable selectable);
        /// <summary>
        /// Selects an element of the MenuPage by default. Called when the MenuPage is showed.
        /// </summary>
        public abstract void SelectDefault();
    }
}
