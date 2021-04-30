using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.MenuElements;

namespace MenuChanger.MenuPanels
{
    public class VerticalItemPanel : IMenuPanel
    {
        public MenuPage Parent { get; private set; }
        public List<IMenuElement> Items { get; private set; }
        public bool Hidden { get; private set; } = true;
        Vector2 localTopCenter;
        float vspace;
        

        public VerticalItemPanel(MenuPage page, Vector2 localTopCenter, float vspace, params IMenuElement[] children)
        {
            Parent = page;
            this.localTopCenter = localTopCenter;
            this.vspace = vspace;

            Items = children.ToList();
            Reposition();
        }

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

    }
}
