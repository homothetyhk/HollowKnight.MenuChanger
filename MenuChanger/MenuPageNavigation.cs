namespace MenuChanger
{
    public abstract class MenuPageNavigation
    {
        public MenuPageNavigation(MenuPage page)
        {
            Page = page;
        }

        public MenuPage Page { get; }

        public abstract void Add(ISelectable selectable);
        public abstract void Remove(ISelectable selectable);
        public abstract void SelectDefault();
    }
}
