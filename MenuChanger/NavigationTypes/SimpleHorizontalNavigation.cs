using MenuChanger.Extensions;

namespace MenuChanger.NavigationTypes
{
    public class SimpleHorizontalNavigation : MenuPageNavigation
    {
        /// <summary>
        /// A list of selectables. Note: Prefer to use <see cref="Items"/> and the add/remove methods instead
        /// of modifying this list directly
        /// </summary>
        public List<ISelectable> Selectables = new();

        public override IReadOnlyCollection<ISelectable> Items => Selectables.AsReadOnly();

        public SimpleHorizontalNavigation(MenuPage page) : base(page)
        {
        }

        public override void SelectDefault()
        {
            if (Selectables.Count > 0)
            {
                Selectable s = Selectables[0].GetSelectable(Neighbor.Up);
                if (s)
                {
                    s.Select();
                    return;
                }
            }

            if (Page.backButton != null && Page.backButton.Button)
            {
                Page.backButton.Button.Select();
            }
        }

        public override void Add(ISelectable selectable)
        {
            selectable.SetNeighbor(Neighbor.Down, Page.backButton);
            selectable.SetNeighbor(Neighbor.Up, Page.backButton);
            if (Selectables.Any())
            {
                selectable.SetNeighbor(Neighbor.Left, Selectables[Selectables.Count - 1]);
                Selectables[Selectables.Count - 1].SetNeighbor(Neighbor.Right, selectable);
                selectable.SetNeighbor(Neighbor.Right, Selectables[0]);
                Selectables[0].SetNeighbor(Neighbor.Left, selectable);
            }
            else
            {
                Page.backButton.SetNeighbor(Neighbor.Up, selectable);
                Page.backButton.SetNeighbor(Neighbor.Down, selectable);
                selectable.SetNeighbor(Neighbor.Left, selectable);
                selectable.SetNeighbor(Neighbor.Right, selectable);
            }

            Selectables.Add(selectable);
        }

        public override void Remove(ISelectable selectable)
        {
            int i = Selectables.IndexOf(selectable);
            if (i >= 0)
            {
                Selectables.RemoveAt(i);
                if (Selectables.Any())
                {
                    ISelectable previous = Selectables[i > 0 ? i - 1 : Selectables.Count - 1];
                    ISelectable next = Selectables[i < Selectables.Count ? i : 0];
                    previous.SetNeighbor(Neighbor.Right, next);
                    next.SetNeighbor(Neighbor.Left, previous);

                    if (i == 0)
                    {
                        Page.backButton.SetNeighbor(Neighbor.Up, next);
                        Page.backButton.SetNeighbor(Neighbor.Down, next);
                    }
                }
                else
                {
                    Page.backButton.SetNeighbor(Neighbor.Up, null);
                    Page.backButton.SetNeighbor(Neighbor.Down, null);
                }
            }
        }

        public override void ResetNavigation()
        {
            if (Selectables.Count == 0)
            {
                Page.backButton.SetNeighbor(Neighbor.Up, null);
                Page.backButton.SetNeighbor(Neighbor.Down, null);
            }
            else
            {
                foreach (ISelectable selectable in Selectables)
                {
                    if (selectable is ISelectableGroup isg)
                    {
                        isg.ResetNavigation();
                    }
                }

                for (int i = 0; i < Selectables.Count; i++)
                {
                    int j = i > 0 ? i - 1 : Selectables.Count - 1;
                    Selectables[i].SymSetNeighbor(Neighbor.Left, Selectables[j]);
                    Selectables[i].SetNeighbor(Neighbor.Up, Page.backButton);
                    Selectables[i].SetNeighbor(Neighbor.Down, Page.backButton);
                }
                Page.backButton.SetNeighbor(Neighbor.Up, Selectables[0]);
                Page.backButton.SetNeighbor(Neighbor.Down, Selectables[0]);
            }
        }

    }
}
