using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// A control for managing a group of ToggleButtons, with modes to allow selecting only one at a time or selecting multiple toggles.
    /// </summary>
    public class RadioSwitch
    {
        /// <summary>
        /// Unused enum. ToggleGroupMode.RadioSwitch corresponds to when the Active property of RadioSwitch is true.
        /// </summary>
        public enum ToggleGroupMode
        {
            RadioSwitch,
            MultiSelect,
        }

        public readonly MenuPage Parent;
        public readonly ToggleButton[] Elements;
        /// <summary>
        /// The currently selected ToggleButton, or null.
        /// </summary>
        public ToggleButton CurrentButton { get; protected set; }
        /// <summary>
        /// Determines whether the RadioSwitch event is called when its buttons are clicked.
        /// </summary>
        public bool Active { get; protected set; } = true;

        /// <summary>
        /// The name of the currently selected ToggleButton, or null.
        /// </summary>
        public string CurrentSelection => CurrentButton?.Name;

        /// <summary>
        /// Creates a RadioSwitch on the specified page, with new ToggleButtons for each string in the array.
        /// <br/>The RadioSwitch does not control the layout of its buttons; it is recommended to use the Elements field with a panel.
        /// </summary>
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

        /// <summary>
        /// Selects the first element in the Elements array.
        /// </summary>
        public void SelectFirst()
        {
            Active = true;
            ChangeSelection(Elements[0]);
        }

        /// <summary>
        /// If a ToggleButton exists with the given name and is unlocked, and the CurrentButton is not locked, selects the new button and returns true.
        /// </summary>
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

        /// <summary>
        /// Use with MultiSelect mode to select a subset of buttons, and lock the result.
        /// </summary>
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

        /// <summary>
        /// Unlocks and deselects all of the ToggleButtons in the switch. For each button, if lockPredicate is not null and evaluates true, then the button is locked afterwards.
        /// </summary>
        public void DeselectAll(Func<ToggleButton, bool> lockPredicate = null)
        {
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
        }

        /// <summary>
        /// Deselects the current button, if not null. In RadioSwitch mode, this leaves the RadioSwitch with no buttons selected.
        /// </summary>
        public void DeselectCurrent()
        {
            CurrentButton?.SetSelection(false);
            CurrentButton = null;
        }

        /// <summary>
        /// If true, the RadioSwitch will act in RadioSwitch mode, ensuring that at most one button can be selected at a time, and the selected button cannot be deselected.
        /// <br/>Otherwise, the toggles can be modified freely.
        /// <br/>Note that SetActive changes how the RadioSwitch updates, but if going from MultiSelect to RadioSwitch, the buttons must be deselected separately.
        /// </summary>
        public void SetActive(bool value)
        {
            Active = value;
        }


        public delegate void RadioSwitchChangedEvent(string selection);
        /// <summary>
        /// Invoked in RadioSwitch mode when the current selection is changed, with the new selection.
        /// </summary>
        public event RadioSwitchChangedEvent Changed;
        protected void InvokeChanged() => Changed?.Invoke(CurrentSelection);
    }
}
