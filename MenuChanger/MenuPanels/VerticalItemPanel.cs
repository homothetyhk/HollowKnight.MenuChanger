using MenuChanger.MenuElements;

namespace MenuChanger.MenuPanels
{
    /// <summary>
    /// A MenuPanel which arranges its elements in a column with a fixed spacing between elements.
    /// </summary>
    public class VerticalItemPanel : IMenuPanel
    {
        public MenuPage Parent { get; }
        public List<IMenuElement> Items { get; }
        public bool Hidden { get; private set; } = true;
        Vector2 localTopCenter;
        float vspace;
        
        /// <summary>
        /// Creates a panel which groups its elements in a column.
        /// </summary>
        /// <param name="page">The page containing the panel.</param>
        /// <param name="localTopCenter">The center of the top item of the panel, in MenuPage coordinates.</param>
        /// <param name="vspace">The space between consecutive elements.</param>
        /// <param name="rootLevel">True if the panel's navigation should be controlled by the MenuPage. False if it will be nested within another panel.</param>
        /// <param name="children">The items of the panel.</param>
        public VerticalItemPanel(MenuPage page, Vector2 localTopCenter, float vspace, bool rootLevel, params IMenuElement[] children)
        {
            Parent = page;
            
            this.localTopCenter = localTopCenter;
            this.vspace = vspace;

            Items = children.ToList();
            Reposition();
            ResetNavigation();
            if (rootLevel) Parent.AddToNavigationControl(this);
        }

        /// <summary>
        /// Reapplies the panel layout to its items.
        /// </summary>
        public void Reposition()
        {
            for (int i = 0; i < Items.Count; i++)  Items[i].MoveTo(localTopCenter + new Vector2(0f, -vspace * i));
        }

        public void MoveTo(Vector2 pos)
        {
            localTopCenter = pos;
            Reposition();
        }

        public void Translate(Vector2 delta)
        {
            localTopCenter += delta;
            Reposition();
        }

        /// <summary>
        /// Updates the panel with the new spacing between rows.
        /// </summary>
        public void Respace(float vspace)
        {
            this.vspace = vspace;
            Reposition();
        }

        public void Add(IMenuElement item)
        {
            Items.Add(item);
            Reposition();
        }

        /// <summary>
        /// Inserts the item into the specified row of the panel.
        /// </summary>
        public void Insert(int index, IMenuElement item)
        {
            Items.Insert(index, item);
            Reposition();
        }

        public bool Remove(IMenuElement item)
        {
            if (Items.Remove(item))
            {
                Reposition();
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
            Reposition();
        }

        public void Hide()
        {
            Hidden = true;
            foreach (IMenuElement item in Items)
            {
                item.Hide();
            }
        }

        public void Show()
        {
            Hidden = false;
            foreach (IMenuElement item in Items)
            {
                item.Show();
            }
        }

        public void Destroy()
        {
            foreach (IMenuElement item in Items) item.Destroy();
            Items.Clear();
        }

        public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
        {
            IEnumerable<ISelectable> selectables = Items.OfType<ISelectable>();
            if (!selectables.Any()) return;
            switch (neighbor)
            {
                case Neighbor.Up:
                    selectables.First().SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Down:
                    selectables.Last().SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Left:
                    foreach (var s in selectables) s.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Right:
                    foreach (var s in selectables) s.SetNeighbor(neighbor, selectable);
                    break;
            }
        }

        public Selectable GetSelectable(Neighbor neighbor)
        {
            return GetISelectable(neighbor)?.GetSelectable(neighbor);
        }

        public ISelectable GetISelectable(Neighbor neighbor)
        {
            IEnumerable<ISelectable> selectables = Items.OfType<ISelectable>();
            if (!selectables.Any()) return null;
            return neighbor switch
            {
                Neighbor.Up => selectables.First(),
                Neighbor.Down => selectables.Last(),
                Neighbor.Left => selectables.First(),
                Neighbor.Right => selectables.Last(),
                _ => null,
            };
        }

        public void ResetNavigation()
        {
            foreach (ISelectableGroup isg in Items.OfType<ISelectableGroup>())
            {
                isg.ResetNavigation();
            }

            ISelectable previous = null;
            foreach (ISelectable current in Items.OfType<ISelectable>())
            {
                if (previous != null)
                {
                    current.SetNeighbor(Neighbor.Up, previous);
                    previous.SetNeighbor(Neighbor.Down, current);
                }
                previous = current;
            }
        }
    }
}
