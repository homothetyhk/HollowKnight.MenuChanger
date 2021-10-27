using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.MenuElements;
using UnityEngine;

namespace MenuChanger.MenuPanels
{
    /*
    public class AlignedItemPanel : IMenuPanel
    {
        public MenuPage Parent { get; }
        public List<IMenuElement> Items { get; }
        public bool Hidden { get; private set; }
        Vector2 localTopCenter;
        float vspace;
        float hspace;
        int[] rowLengths;
        

        public AlignedItemPanel(MenuPage page, Vector2 localTopCenter, float vspace, float hspace, int[] rowLengths, params IMenuElement[] children)
        {
            Parent = page;
            this.localTopCenter = localTopCenter;
            this.vspace = vspace;
            this.hspace = hspace;
            this.rowLengths = rowLengths;

            Items = children.ToList();
            Reposition();
        }

        public void Reposition()
        {
            if (Items.Count == 0) return;

            int i = 0;
            for (int r = 0; r < rowLengths.Length; r++)
            {
                Vector2 rowStart = localTopCenter + new Vector2(-hspace * (rowLengths[r] / 2f - 0.5f), -vspace * r);
                for (int c = 0; c < rowLengths[r]; c++)
                {
                    Items[i++].MoveTo(rowStart + new Vector2(hspace * c, 0f));
                    if (i >= Items.Count) return;
                }
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

        public void Reshape(int[] rowLengths)
        {
            this.rowLengths = rowLengths;
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

    }
    */
}
