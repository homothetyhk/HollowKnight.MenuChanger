using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.MenuElements;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuPanels
{
    public class OrderedItemViewer : IMenuPanel
    {
        public List<IMenuElement> Items { get; private set; }
        public MenuPage Parent { get; private set; }
        public SmallButton PrevButton { get; private set; }
        public SmallButton NextButton { get; private set; }
        public MenuLabel IndexLabel { get; private set; }
        public bool Hidden { get; private set; } = true;


        private int _index = 0;
        public int Index 
        { get => _index; set => _index = ((value % Items.Count) + Items.Count) % Items.Count; }
        Vector2 offset;

        public OrderedItemViewer(MenuPage page, params IMenuElement[] items)
            : this(page, new Vector2(-800, -400), new Vector2(800, -400), new Vector2(0, -400), items) { }

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
        }

        protected string ComputeCounterText()
        {
            return $"{Index + 1}/{Items.Count}";
        }

        public void ToPrevious()
        {
            Items[Index--].Hide();
            Items[Index].Show();
            IndexLabel.Text.text = ComputeCounterText();
        }

        public void ToNext()
        {
            Items[Index++].Hide();
            Items[Index].Show();
            IndexLabel.Text.text = ComputeCounterText();
        }

        public void JumpTo(int index)
        {
            Items[Index].Hide();
            Index = index;
            Items[Index].Show();
            IndexLabel.Text.text = ComputeCounterText();
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
        }

        public virtual bool Remove(IMenuElement obj)
        {
            bool val = Items.Remove(obj);
            IndexLabel.Text.text = ComputeCounterText();
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
    }
}
