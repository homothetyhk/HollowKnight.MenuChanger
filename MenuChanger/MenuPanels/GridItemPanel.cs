using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.MenuElements;
using UnityEngine.UI;

namespace MenuChanger.MenuPanels
{
    /// <summary>
    /// A MenuPanel which groups its items in a grid with a fixed number of columns and fixed horizontal and vertical spacing.
    /// </summary>
    public class GridItemPanel : IMenuPanel
    {
        public MenuPage Parent { get; }
        public List<IMenuElement> Items { get; }
        public bool Hidden { get; private set; }
        Vector2 localTopCenter;

        float vspace;
        float hspace;
        int columns;

        /// <summary>
        /// Creates a panel which groups its elements in a grid with a fixed number of columns.
        /// </summary>
        /// <param name="page">The page containing the panel.</param>
        /// <param name="localTopCenter">The center of the top row of the panel, in MenuPage coordinates.</param>
        /// <param name="columns">The number of columns in the grid.</param>
        /// <param name="vspace">The vertical space between consecutive rows.</param>
        /// <param name="hspace">The horizontal space between consecutive columns.</param>
        /// <param name="rootLevel">True if the panel's navigation should be controlled by the MenuPage. False if it will be nested within another panel.</param>
        /// <param name="children">The items of the panel.</param>
        public GridItemPanel(MenuPage page, Vector2 localTopCenter, int columns, float vspace, float hspace, bool rootLevel, params IMenuElement[] children)
        {
            Parent = page;
            this.localTopCenter = localTopCenter;

            this.columns = columns;
            this.vspace = vspace;
            this.hspace = hspace;

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
            Vector2 topLeft = localTopCenter - new Vector2((columns / 2f - 0.5f) * hspace, 0f);

            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].MoveTo(topLeft + new Vector2(i % columns * hspace, i / columns * (-vspace)));
            }

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
            if (Hidden) item.Hide();
        }

        public void AddRange(IEnumerable<IMenuElement> items)
        {
            Items.AddRange(items);
            Reposition();
            if (Hidden) foreach (IMenuElement item in items) item.Hide();
        }

        /// <summary>
        /// Inserts the item into the specified row-major index of the panel.
        /// </summary>
        public void Insert(int index, IMenuElement item)
        {
            Items.Insert(index, item);
            Reposition();
            if (Hidden) item.Hide();
        }

        /// <summary>
        /// Inserts the item into the specified row and column of the panel.
        /// </summary>
        public void Insert(int row, int column, IMenuElement item)
        {
            Insert(row * columns + column, item);
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

        public void Clear()
        {
            foreach (IMenuElement item in Items)
            {
                item.Hide();
            }
            Items.Clear();
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

        private IEnumerable<IMenuElement> GetColumn(int c) => Items.Where((e, i) => i % columns == c);
        private IEnumerable<IMenuElement> GetRow(int r) => Items.Skip(r * columns).Take(columns);

        public void ResetNavigation()
        {
            foreach (ISelectableGroup isg in Items.OfType<ISelectableGroup>())
            {
                isg.ResetNavigation();
            }

            for (int r = 0; r * columns < Items.Count; r++)
            {
                ISelectable previous = null;
                foreach (ISelectable current in GetRow(r).OfType<ISelectable>())
                {
                    if (previous != null)
                    {
                        current.SetNeighbor(Neighbor.Left, previous);
                        previous.SetNeighbor(Neighbor.Right, current);
                    }
                    previous = current;
                }
            }

            for (int c = 0; c < columns; c++)
            {
                ISelectable previous = null;
                foreach (ISelectable current in GetColumn(c).OfType<ISelectable>())
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

        public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
        {
            switch (neighbor)
            {
                case Neighbor.Up:
                    for (int c = 0; c < columns; c++) GetColumn(c).OfType<ISelectable>().FirstOrDefault()?.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Down:
                    for (int c = 0; c < columns; c++) GetColumn(c).OfType<ISelectable>().LastOrDefault()?.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Left:
                    for (int r = 0; r * columns < Items.Count; r++) GetRow(r).OfType<ISelectable>().FirstOrDefault()?.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Right:
                    for (int r = 0; r * columns < Items.Count; r++) GetRow(r).OfType<ISelectable>().LastOrDefault()?.SetNeighbor(neighbor, selectable);
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
    }
}
