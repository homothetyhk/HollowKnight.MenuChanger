using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.MenuElements;
using UnityEngine;

namespace MenuChanger.MenuPanels
{
    /// <summary>
    /// An ordered item viewer containing grid panels. Allows a grid panel to take an arbitrary number of elements without overflowing.
    /// </summary>
    public class MultiGridItemPanel : OrderedItemViewer
    {
        readonly int Columns;
        readonly int GridCount;

        readonly float Vspace;
        readonly float Hspace;
        Vector2 TopCenter;

        /// <summary>
        /// Creates a panel which can group an arbitrary number of elements on paginated grids.
        /// </summary>
        /// <param name="page">The page containing the panel.</param>
        /// <param name="rows">The number of rows to allow in the grid before starting a new page.</param>
        /// <param name="columns">The number of columns in the grid.</param>
        /// <param name="vspace">The vertical space between consecutive rows.</param>
        /// <param name="hspace">The horizontal space between consecutive columns.</param>
        /// <param name="topCenter">The center of the top row of the panel, in MenuPage coordinates.</param>
        /// <param name="children">The items of the panel.</param>
        public MultiGridItemPanel(MenuPage page,
            int rows, int columns, float vspace, float hspace,
            Vector2 topCenter,
            params IMenuElement[] children)
            : base(page)
        {
            Columns = columns;
            Vspace = vspace;
            Hspace = hspace;
            TopCenter = topCenter;
            GridCount = rows * columns;

            for (int i = 0; i * GridCount < children.Length; i++)
            {
                int span = Math.Min(children.Length - i * GridCount, GridCount);
                if (span <= 0) break;
                IMenuElement[] next = new IMenuElement[span];
                Array.Copy(children, i * GridCount, next, 0, span);
                base.Add(NewPanel(next));
            }

            if (Items.Any())
            {
                foreach (IMenuElement panel in Items) panel.Hide();
                Items[Index].Show();
            }
        }

        /// <summary>
        /// Creates a panel which can group an arbitrary number of elements on paginated grids.
        /// </summary>
        /// <param name="page">The page containing the panel.</param>
        /// <param name="rows">The number of rows to allow in the grid before starting a new page.</param>
        /// <param name="columns">The number of columns in the grid.</param>
        /// <param name="vspace">The vertical space between consecutive rows.</param>
        /// <param name="hspace">The horizontal space between consecutive columns.</param>
        /// <param name="topCenter">The center of the top row of the panel, in MenuPage coordinates.</param>
        /// <param name="prevPos">The position of the previous button.</param>
        /// <param name="nextPos">The position of the next button.</param>
        /// <param name="indexPos">The position of the index label.</param>
        /// <param name="children">The items of the panel.</param>
        public MultiGridItemPanel(MenuPage page, 
            int rows, int columns, float vspace, float hspace, 
            Vector2 topCenter, Vector2 prevPos, Vector2 nextPos, Vector2 indexPos, 
            params IMenuElement[] children)
            : base(page, prevPos, nextPos, indexPos)
        {
            Columns = columns;
            Vspace = vspace;
            Hspace = hspace;
            TopCenter = topCenter;
            GridCount = rows * columns;
            
            for (int i = 0; i * GridCount < children.Length; i++)
            {
                int span = Math.Min(children.Length - i * GridCount, GridCount);
                if (span <= 0) break;
                IMenuElement[] next = new IMenuElement[span];
                Array.Copy(children, i * GridCount, next, 0, span);
                base.Add(NewPanel(next));
            }

            if (Items.Any())
            {
                foreach (IMenuElement panel in Items) panel.Hide();
                Items[Index].Show();
            }
        }

        private GridItemPanel NewPanel(params IMenuElement[] children)
        {
            return new GridItemPanel(Parent, TopCenter, Columns, Vspace, Hspace, false, children);
        }

        public override void Add(IMenuElement obj)
        {
            GridItemPanel last = Items.Last() as GridItemPanel;
            if (last.Items.Count == GridCount)
            {
                last = NewPanel(obj);
                base.Add(last);
            }
            else
            {
                last.Add(obj);
            }
            if (last.Hidden) obj.Hide();
        }

        // Absolutely no rebalancing
        public override bool Remove(IMenuElement obj)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (((GridItemPanel)Items[i]).Items.Remove(obj)) return true;
            }

            return false;
        }

    }
}
