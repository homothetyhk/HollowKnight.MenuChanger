using MenuChanger.Extensions;

namespace MenuChanger.NavigationTypes
{
    public class SimpleHorizontalNavigation : MenuPageNavigation
    {
        public List<ISelectable> selectables = new();

        public override IReadOnlyCollection<ISelectable> Selectables => selectables.AsReadOnly();

        public SimpleHorizontalNavigation(MenuPage page) : base(page)
        {
        }

        public override void SelectDefault()
        {
            if (selectables.Count > 0)
            {
                Selectable s = selectables[0].GetSelectable(Neighbor.Up);
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
            if (selectables.Any())
            {
                selectable.SetNeighbor(Neighbor.Left, selectables[selectables.Count - 1]);
                selectables[selectables.Count - 1].SetNeighbor(Neighbor.Right, selectable);
                selectable.SetNeighbor(Neighbor.Right, selectables[0]);
                selectables[0].SetNeighbor(Neighbor.Left, selectable);
            }
            else
            {
                Page.backButton.SetNeighbor(Neighbor.Up, selectable);
                Page.backButton.SetNeighbor(Neighbor.Down, selectable);
                selectable.SetNeighbor(Neighbor.Left, selectable);
                selectable.SetNeighbor(Neighbor.Right, selectable);
            }

            selectables.Add(selectable);
        }

        public override void Remove(ISelectable selectable)
        {
            int i = selectables.IndexOf(selectable);
            if (i >= 0)
            {
                selectables.RemoveAt(i);
                if (selectables.Any())
                {
                    ISelectable previous = selectables[i > 0 ? i - 1 : selectables.Count - 1];
                    ISelectable next = selectables[i < selectables.Count ? i : 0];
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
            if (selectables.Count == 0)
            {
                Page.backButton.SetNeighbor(Neighbor.Up, null);
                Page.backButton.SetNeighbor(Neighbor.Down, null);
            }
            else
            {
                foreach (ISelectable selectable in selectables)
                {
                    if (selectable is ISelectableGroup isg)
                    {
                        isg.ResetNavigation();
                    }
                }

                for (int i = 0; i < selectables.Count; i++)
                {
                    int j = i > 0 ? i - 1 : selectables.Count - 1;
                    selectables[i].SymSetNeighbor(Neighbor.Left, selectables[j]);
                    selectables[i].SetNeighbor(Neighbor.Up, Page.backButton);
                    selectables[i].SetNeighbor(Neighbor.Down, Page.backButton);
                }
                Page.backButton.SetNeighbor(Neighbor.Up, selectables[0]);
                Page.backButton.SetNeighbor(Neighbor.Down, selectables[0]);
            }
        }

    }
}
