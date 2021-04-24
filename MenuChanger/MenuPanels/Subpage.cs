using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.MenuElements;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuPanels
{
    public class Subpage : IMenuPanel
    {
        public MenuPage Parent { get; private set; }
        public List<IMenuElement> Items { get; private set; }

        private Vector2 offset;

        public MenuLabel TitleLabel { get; private set; }
        public bool Hidden { get; private set; }

        public Subpage(MenuPage page, string title, Vector2 titlePos)
        {
            Parent = page;
            TitleLabel = new MenuLabel(page, title, MenuLabel.Style.Title);
            TitleLabel.MoveTo(titlePos);
            Items = new List<IMenuElement>();
        }
        public Subpage(MenuPage page, string title) : this (page, title, new Vector2(0, 400)) { }
        public Subpage(MenuPage page) : this (page, string.Empty, new Vector2(0, 400)) { }

        

        public void Add(IMenuElement obj)
        {
            Items.Add(obj);
            if (Hidden) obj.Hide();
        }

        public bool Remove(IMenuElement obj)
        {
            return Items.Remove(obj);
        }

        public void MoveTo(Vector2 pos)
        {
            Translate(pos - offset);
        }

        public void Translate(Vector2 delta)
        {
            offset += delta;
            foreach (IMenuElement obj in Items)
            {
                obj.Translate(delta);
            }
            TitleLabel?.Translate(delta);
        }

        public void Show()
        {
            Hidden = false;
            foreach (IMenuElement obj in Items)
            {
                obj.Show();
            }
            TitleLabel?.Show();
        }

        public void Hide()
        {
            Hidden = true;
            foreach (IMenuElement obj in Items)
            {
                obj.Hide();
            }
            TitleLabel?.Hide();
        }

        public void Destroy()
        {
            foreach (IMenuElement obj in Items)
            {
                obj.Destroy();
            }
            Items.Clear();
            TitleLabel?.Destroy();
        }

    }
}
