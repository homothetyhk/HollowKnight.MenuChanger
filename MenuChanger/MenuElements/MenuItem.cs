using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MenuChanger.MenuElements
{
    public class MenuItem<T> : SmallButton, IMenuItem
    {
        public T CurrentSelection => currentSelection;
        public object BoxedCurrentSelection => currentSelection as object;

        public string Name { get; private set; }
        public bool Locked { get; protected set; }
        
        public event MenuItemChanged Changed
        {
            add => ChangedInternal += value;
            remove => ChangedInternal -= value;
        }

        public event MenuItemFormat Format
        {
            add => FormatInternal += value;
            remove => FormatInternal -= value;
        }

        public MenuItem(MenuPage page, string name, params T[] values) : base(page, name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (values == null || values.Length == 0)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Selections = values;
            Name = name;

            _text = Button.transform.Find("Text").GetComponent<Text>();
            _text.fontSize = 36;
            _align = Button.gameObject.GetComponentInChildren<FixVerticalAlign>(true);

            Button.ClearEvents();
            Button.AddEvent(MoveNext);

            currentIndex = 0;
            currentSelection = Selections[0];
            
            RefreshText();
        }


        public bool TrySetSelection(object obj, bool invokeChanged)
        {
            if (obj is T t)
            {
                SetSelection(t, invokeChanged);
                return true;
            }
            return false;
        }

        public void SetSelection(T obj, bool invokeChanged)
        {
            if (Locked) return;

            currentIndex = -1;
            for (int i = 0; i < Selections.Length; i++)
            {
                if (Selections[i].Equals(obj))
                {
                    currentIndex = i;
                    break;
                }
            }
            currentSelection = obj;

            RefreshText(invokeChanged);
        }

        public void MoveNext()
        {
            if (Locked) return;

            currentIndex++;
            if (currentIndex >= Selections.Length)
            {
                currentIndex = 0;
            }
            currentSelection = Selections[currentIndex];

            RefreshText();
        }

        protected virtual void RefreshText(bool invokeEvent = true)
        {
            _text.text = InvokeFormat(Name, ": ", currentSelection.ToString());

            _align.AlignText();

            if (invokeEvent)
            {
                InvokeChanged();
            }
        }

        public void Bind(object obj, FieldInfo field)
        {
            Changed += (self) => field.SetValue(obj, self.BoxedCurrentSelection);
            //Button.gameObject.AddComponent<Components.Updater>().action += () => TrySetSelection(field.GetValue(obj), true);
        }

        public virtual void Lock()
        {
            Locked = true;
        }

        public virtual void Unlock()
        {
            Locked = false;
        }

        protected readonly FixVerticalAlign _align;
        protected readonly T[] Selections;
        protected readonly Text _text;
        protected int currentIndex;
        protected T currentSelection;

        public delegate void MenuItemChanged(MenuItem<T> item);
        protected event MenuItemChanged ChangedInternal;
        protected void InvokeChanged() => ChangedInternal?.Invoke(this);



        public delegate (string, string, string) MenuItemFormat(T selection, string prefix, string cons, string repr);
        protected event MenuItemFormat FormatInternal;
        protected string InvokeFormat(string prefix, string cons, string repr)
        {
            foreach (MenuItemFormat format in FormatInternal?.GetInvocationList() ?? new Delegate[0])
            {
                (prefix, cons, repr) = format(currentSelection, prefix, cons, repr);
            }

            return $"{prefix}{cons}{repr}";
        }
    }
}
