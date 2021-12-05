using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.MenuElements;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuPanels
{
    /// <summary>
    /// A panel which does not arrange its elements. Includes an optional title label. Essentially behaves like a MenuPage, including in how it handles navigation.
    /// </summary>
    public class Subpage : IMenuPanel
    {
        public MenuPage Parent { get; }
        public List<IMenuElement> Items { get; }

        private Vector2 offset;

        public MenuLabel TitleLabel { get; }
        public bool Hidden { get; private set; }

        /// <summary>
        /// Creates a Subpage with a title label at the specified position.
        /// </summary>
        public Subpage(MenuPage page, string title, Vector2 titlePos)
        {
            Parent = page;
            TitleLabel = new MenuLabel(page, title, MenuLabel.Style.Title);
            TitleLabel.MoveTo(titlePos);
            Items = new List<IMenuElement>();
        }
        /// <summary>
        /// Creates a Subpage with a title label at (0, 400).
        /// </summary>
        public Subpage(MenuPage page, string title) : this (page, title, new Vector2(0, 400)) { }
        /// <summary>
        /// Creates a Subpage with an empty title label at (0, 400).
        /// </summary>
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
        
        public void SetNeighbor(Neighbor neighbor, ISelectable selectable) // defaults to horizontally linking elements
        {
            IEnumerable<ISelectable> selectables = Items.OfType<ISelectable>();
            if (!selectables.Any()) return;
            switch (neighbor)
            {
                case Neighbor.Up:
                    foreach (var s in selectables) s.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Down:
                    foreach (var s in selectables) s.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Left:
                    selectables.First().SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Right:
                    selectables.Last().SetNeighbor(neighbor, selectable);
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
                    current.SetNeighbor(Neighbor.Left, previous);
                    previous.SetNeighbor(Neighbor.Right, current);
                }
                previous = current;
            }
        }
    }
}
