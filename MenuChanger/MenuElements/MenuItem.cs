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
        public int CurrentIndex { get; protected set; }
        public T CurrentSelection { get; protected set; }
        public int Count => Selections.Count;
        public object BoxedCurrentSelection => CurrentSelection as object;

        public string Name { get; private set; }
        public bool Locked { get; protected set; }

        public MenuItem(MenuPage page, string name, params T[] values) : this(page, name, values?.ToList() ?? new List<T>()) { }

        // For maintaining reference to original list
        public MenuItem(MenuPage page, string name, List<T> values) : base(page, name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Selections = values;
            Name = name;

            _text = Button.transform.Find("Text").GetComponent<Text>();
            _text.fontSize = 45;
            _align = Button.gameObject.GetComponentInChildren<FixVerticalAlign>(true);

            Button.ClearEvents();
            Button.AddEvent(MoveNext);

            CurrentIndex = 0;
            if (Selections.Count > 0) CurrentSelection = Selections[0];
            else CurrentSelection = default;
            
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

            CurrentIndex = -1;
            for (int i = 0; i < Selections.Count; i++)
            {
                if (Selections[i].Equals(obj))
                {
                    CurrentIndex = i;
                    break;
                }
            }
            CurrentSelection = obj;

            RefreshText(invokeChanged);
        }

        public void MoveNext()
        {
            if (Locked) return;

            CurrentIndex++;
            if (CurrentIndex >= Selections.Count)
            {
                CurrentIndex = 0;
            }
            if (Selections.Count > 0)
            {
                CurrentSelection = Selections[CurrentIndex];
            }

            RefreshText();
        }

        public void AddItem(T t)
        {
            Selections.Add(t);
        }

        public void RemoveItem(T t)
        {
            Selections.Remove(t);
            if (CurrentSelection.Equals(t)) MoveNext();
        }

        public void OverwriteCurrent(T t)
        {
            if (Selections.Count > 0)
            {
                CurrentSelection = Selections[CurrentIndex] = t;
                RefreshText();
            }
        }

        public void RemoveCurrent()
        {
            if (Selections.Count > 0)
            {
                Selections.RemoveAt(CurrentIndex--);
                MoveNext();
            }
        }

        protected virtual void RefreshText(bool invokeEvent = true)
        {
            _text.text = InvokeFormat(Name, ": ", CurrentSelection?.ToString() ?? string.Empty);

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
        protected readonly List<T> Selections; 
        protected readonly Text _text;
        

        public event MenuItemChanged Changed;
        public delegate void MenuItemChanged(MenuItem<T> item);
        protected void InvokeChanged() => Changed?.Invoke(this);


        public event MenuItemFormat Format;
        public delegate (string, string, string) MenuItemFormat(T selection, string prefix, string cons, string repr);
        protected string InvokeFormat(string prefix, string cons, string repr)
        {
            foreach (MenuItemFormat format in Format?.GetInvocationList() ?? new Delegate[0])
            {
                (prefix, cons, repr) = format(CurrentSelection, prefix, cons, repr);
            }

            return $"{prefix}{cons}{repr}";
        }
    }
}
