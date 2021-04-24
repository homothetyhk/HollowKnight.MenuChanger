using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.MenuElements;

namespace MenuChanger.MenuPanels
{
    public class GridItemPanel : IMenuPanel
    {
        public MenuPage Parent { get; private set; }
        public List<IMenuElement> Items { get; private set; }
        public bool Hidden { get; private set; }
        Vector2 localTopCenter;

        float vspace;
        float hspace;
        int columns;
        

        public GridItemPanel(MenuPage page, Vector2 localTopCenter, int columns, float vspace, float hspace, params IMenuElement[] children)
        {
            Parent = page;
            this.localTopCenter = localTopCenter;

            this.columns = columns;
            this.vspace = vspace;
            this.hspace = hspace;

            Items = children.ToList();
            Reposition();
        }

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

        public void Insert(int index, IMenuElement item)
        {
            Items.Insert(index, item);
            Reposition();
            if (Hidden) item.Hide();
        }

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

    }
}
