using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuPanels
{
    /// <summary>
    /// A panel which shows one element at a time, managed by arrow buttons.
    /// </summary>
    public class OrderedItemViewer : IMenuPanel
    {
        public List<IMenuElement> Items { get; }
        public MenuPage Parent { get; }
        public SmallButton PrevButton { get; }
        public SmallButton NextButton { get; }
        public MenuLabel IndexLabel { get; }
        public bool Hidden { get; private set; } = true;


        private int _index = 0;
        public int Index 
        { get => _index; set => _index = ((value % Items.Count) + Items.Count) % Items.Count; }
        Vector2 offset;

        /// <summary>
        /// Creates a panel which shows one element at a time, managed by arrow buttons placed on either side of the back button.
        /// </summary>
        /// <param name="page">The page containing the panel.</param>
        /// <param name="items">The items of the panel.</param>
        public OrderedItemViewer(MenuPage page, params IMenuElement[] items)
            : this(page, new Vector2(-800, -400), new Vector2(800, -400), new Vector2(0, -400), items) { }

        /// <summary>
        /// Creates a panel which shows one element at a time, managed by arrow buttons placed at the specified positions.
        /// </summary>
        /// <param name="page">The page containing the panel.</param>
        /// <param name="prevPos">The position of the previous button.</param>
        /// <param name="prevPos">The position of the next button.</param>
        /// <param name="prevPos">The position of the label showing the current page index.</param>
        /// <param name="items">The items of the panel.</param>
        public OrderedItemViewer(MenuPage page, Vector2 prevPos, Vector2 nextPos, Vector2 indexLabelPos, params IMenuElement[] items)
        {
            Parent = page;
            Items = items.ToList();

            if (Items.Any())
            {
                foreach (IMenuElement panel in Items) panel.Hide();
                Items[Index].Show();
            }

            PrevButton = new SmallButton(Parent, "<<");
            PrevButton.Button.AddEvent(ToPrevious);
            PrevButton.MoveTo(prevPos);

            IndexLabel = new MenuLabel(Parent, ComputeCounterText());
            IndexLabel.MoveTo(indexLabelPos);

            NextButton = new SmallButton(Parent, ">>");
            NextButton.Button.AddEvent(ToNext);
            NextButton.MoveTo(nextPos);

            ResetNavigation();
            Parent.AddToNavigationControl(this);
        }

        protected string ComputeCounterText()
        {
            return $"{Index + 1}/{Items.Count}";
        }

        /// <summary>
        /// Hides the current item of the OrderedItemViewer and shows the item at the previous index, wrapping around if necessary.
        /// </summary>
        public void ToPrevious()
        {
            Items[Index--].Hide();
            Items[Index].Show();
            IndexLabel.Text.text = ComputeCounterText();
            ResetNavigation();
        }

        /// <summary>
        /// Hides the current item of the OrderedItemViewer and shows the item at the next index, wrapping around if necessary.
        /// </summary>
        public void ToNext()
        {
            Items[Index++].Hide();
            Items[Index].Show();
            IndexLabel.Text.text = ComputeCounterText();
            ResetNavigation();
        }

        /// <summary>
        /// Hides the current item of the OrderedItemViewer and shows the item at the given index.
        /// </summary>
        public void JumpTo(int index)
        {
            Items[Index].Hide();
            Index = index;
            Items[Index].Show();
            IndexLabel.Text.text = ComputeCounterText();
            ResetNavigation();
        }

        public void JumpTo(IMenuElement element)
        {
            int i = Items.IndexOf(element);
            if (i >= 0) JumpTo(i);
        }


        public virtual void Add(IMenuElement obj)
        {
            Items.Add(obj);
            obj.Hide();
            IndexLabel.Text.text = ComputeCounterText();
            ResetNavigation();
        }

        public virtual bool Remove(IMenuElement obj)
        {
            bool val = Items.Remove(obj);
            IndexLabel.Text.text = ComputeCounterText();
            ResetNavigation();
            return val;
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
        }

        public void Show()
        {
            Hidden = false;
            Items[Index].Show();
            PrevButton.Show();
            IndexLabel.Show();
            NextButton.Show();
        }

        public void Hide()
        {
            Hidden = true;
            Items[Index].Hide();
            PrevButton.Hide();
            IndexLabel.Hide();
            NextButton.Hide();
        }

        public void Destroy()
        {
            foreach (IMenuElement obj in Items)
            {
                obj.Destroy();
            }
            Items.Clear();
            PrevButton.Destroy();
            IndexLabel.Destroy();
            NextButton.Destroy();
        }

        private readonly Dictionary<Neighbor, ISelectable> neighbors = new();


        public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
        {
            switch (neighbor)
            {
                case Neighbor.Down:
                    PrevButton.SetNeighbor(neighbor, selectable);
                    NextButton.SetNeighbor(neighbor, selectable);
                    break;
                case Neighbor.Up:
                case Neighbor.Left:
                case Neighbor.Right:
                    neighbors[neighbor] = selectable;
                    if (Items.Count > 0 && Items[Index] is ISelectable s) s.SetNeighbor(neighbor, selectable);
                    break;
            }
        }

        public Selectable GetSelectable(Neighbor neighbor)
        {
            return GetISelectable(neighbor)?.GetSelectable(neighbor);
        }

        public ISelectable GetISelectable(Neighbor neighbor)
        {
            if (Items.Count == 0 || Items[Index] is not ISelectable selectable)
            {
                return neighbor switch
                {
                    Neighbor.Up => PrevButton,
                    Neighbor.Left => PrevButton,
                    Neighbor.Right => NextButton,
                    Neighbor.Down => NextButton,
                    _ => null,
                };
            }

            return neighbor switch
            {
                Neighbor.Down => NextButton,
                _ => selectable,
            };
        }

        public void ResetNavigation()
        {
            PrevButton.SetNeighbor(Neighbor.Right, Parent.backButton);
            Parent.backButton.SetNeighbor(Neighbor.Left, PrevButton);

            NextButton.SetNeighbor(Neighbor.Left, Parent.backButton);
            Parent.backButton.SetNeighbor(Neighbor.Right, NextButton);

            PrevButton.SetNeighbor(Neighbor.Left, NextButton);
            NextButton.SetNeighbor(Neighbor.Right, PrevButton);

            if (Items.Count > 0)
            {
                if (Items[Index] is ISelectableGroup isg) isg.ResetNavigation();
                if (Items[Index] is ISelectable selectable)
                {
                    PrevButton.SetNeighbor(Neighbor.Up, selectable);
                    NextButton.SetNeighbor(Neighbor.Up, selectable);
                    selectable.SetNeighbor(Neighbor.Down, Parent.backButton);
                    foreach (var kvp in neighbors) selectable.SetNeighbor(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
