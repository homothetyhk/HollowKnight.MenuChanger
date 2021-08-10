using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MenuChanger.MenuElements
{
    public class RadioSwitch
    {
        public enum ToggleGroupMode
        {
            RadioSwitch,
            MultiSelect,
        }

        public readonly MenuPage Parent;
        public readonly ToggleButton[] Elements;
        public ToggleButton CurrentButton { get; protected set; }
        /// <summary>
        /// Determines whether the RadioSwitch event is called when its buttons are clicked.
        /// </summary>
        public bool Active { get; protected set; } = true;

        public string CurrentSelection => CurrentButton?.Name;

        public RadioSwitch(MenuPage page, params string[] ts)
        {
            if (ts is null || ts.Length == 0) throw new ArgumentException();

            Parent = page;
            Elements = ts.Select(t => new ToggleButton(page, t)).ToArray();
            SelectFirst();

            foreach (ToggleButton b in Elements)
            {
                b.InterceptChanged += OnClick;
            }
        }

        public void OnClick(object sender, InterceptEventArgs<bool> args)
        {
            ToggleButton button = sender as ToggleButton;
            if (!Active) return;
            if (CurrentButton == sender)
            {
                args.cancelChange = true;
                return;
            }
            if (args.orig)
            {
                return;
            }
            if ((args.orig == args.current) || args.cancelChange)
            {
                return;
            }
            MenuChangerMod.instance.Log($"OnClick called with current button {CurrentSelection} ({CurrentButton?.CurrentSelection}) and new button {button.Name} ({button.CurrentSelection})");

            ToggleButton previous = CurrentButton;
            CurrentButton = sender as ToggleButton;
            previous?.SetSelection(false);
            InvokeChanged();
        }

        public void ChangeSelection(ToggleButton newButton)
        {
            if (CurrentButton != null)
            {
                ToggleButton previous = CurrentButton;
                CurrentButton = null;
                previous.SetSelection(false);
            }

            newButton.SetSelection(true);
            CurrentButton = newButton;
            InvokeChanged();
        }

        public void SelectFirst()
        {
            MenuChangerMod.instance.Log("Trying SelectFirst...");
            Active = true;
            ChangeSelection(Elements[0]);
            MenuChangerMod.instance.Log("SelectFirst successful.");
        }

        public bool TrySelect(string name)
        {
            if (Elements.FirstOrDefault(b => b.Name == name) is ToggleButton button && !button.Locked && !(CurrentButton?.Locked ?? false))
            {
                Active = true;
                ChangeSelection(button);
                return true;
            }
            return false;
        }

        public void MatchPredicateAndLock(Func<ToggleButton, bool> predicate)
        {
            Active = false;
            foreach (ToggleButton button in Elements)
            {
                button.Unlock();
                button.SetSelection(predicate(button));
                button.Lock();
            }
        }

        public void DeselectAll(Func<ToggleButton, bool> lockPredicate = null)
        {
            MenuChangerMod.instance.Log("Trying deselect...");
            Active = false;
            foreach (ToggleButton button in Elements)
            {
                button.Unlock();
                if (button.CurrentSelection) button.SetSelection(false);
                if (lockPredicate != null && lockPredicate(button))
                {
                    if (lockPredicate(button)) button.Lock();
                }
            }
            MenuChangerMod.instance.Log("Deselect successful.");
        }

        public void DeselectCurrent()
        {
            CurrentButton?.SetSelection(false);
            CurrentButton = null;
        }

        public void SetActive(bool value)
        {
            Active = value;
        }

        public delegate void RadioSwitchChangedEvent(string selection);
        public event RadioSwitchChangedEvent Changed;
        protected void InvokeChanged() => Changed?.Invoke(CurrentSelection);
    }
}
