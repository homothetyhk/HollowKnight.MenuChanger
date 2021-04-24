using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MenuChanger.MenuElements
{
    public class RadioSwitch
    {
        public readonly MenuPage Parent;
        public readonly ToggleButton[] Elements;
        public ToggleButton CurrentButton { get; protected set; }
        public string CurrentSelection => CurrentButton?.Name;

        public RadioSwitch(MenuPage page, params string[] ts)
        {
            if (ts is null || ts.Length == 0) throw new ArgumentException();

            Parent = page;
            Elements = ts.Select(t => new ToggleButton(page, t)).ToArray();
            CurrentButton = Elements[0];
            CurrentButton.SetSelection(true, true);

            foreach (ToggleButton b in Elements)
            {
                b.Changed += OnClick;
            }

        }

        public void OnClick(MenuItem<bool> clicked)
        {
            if (CurrentButton == clicked)
            {
                clicked.SetSelection(true, false);
                return;
            }
            if (CurrentButton.Locked)
            {
                clicked.SetSelection(false, false);
                return;
            }

            CurrentButton.SetSelection(false, false);
            CurrentButton = (ToggleButton)clicked;
            InvokeChanged();
        }

        public void Deselect()
        {
            CurrentButton.SetSelection(false, true);
            CurrentButton = null;
        }

        public delegate void RadioSwitchChangedEvent(string selection);
        protected RadioSwitchChangedEvent ChangedInternal;
        public event RadioSwitchChangedEvent Changed
        {
            add => ChangedInternal += value;
            remove => ChangedInternal -= value;
        }
        protected void InvokeChanged() => ChangedInternal?.Invoke(CurrentSelection);
    }
}
