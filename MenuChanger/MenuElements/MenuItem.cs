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

            _align = Button.gameObject.GetComponentInChildren<FixVerticalAlign>(true);

            Button.ClearEvents();
            Button.AddEvent(MoveNext);

            CurrentIndex = 0;
            if (Selections.Count > 0) CurrentSelection = Selections[0];
            else CurrentSelection = default;
            
            RefreshText();
        }


        public bool TrySetSelection(object obj)
        {
            if (obj is T t)
            {
                SetSelection(t);
                return true;
            }
            return false;
        }

        public void SetSelection(T obj)
        {
            if (Locked) return;

            InterceptEventArgs<T> args = InvokeInterceptChanged(obj);
            if (args.cancelChange) return;

            int i;
            for (i = 0; i < Selections.Count; i++)
            {
                if (Equals(Selections[i], args.current)) break;
            }
            if (i == Selections.Count) return;

            CurrentIndex = i;
            CurrentSelection = Selections[i];
            RefreshText();
        }

        public void MoveNext()
        {
            if (Locked) return;

            int i = CurrentIndex + 1;
            if (i >= Selections.Count) i = 0;

            InterceptEventArgs<T> args = InvokeInterceptChanged(Selections[i]);
            if (args.cancelChange) return;

            if (!Equals(args.orig, args.current))
            {
                for (i = 0; i < Selections.Count; i++)
                {
                    if (Equals(Selections[i], args.current)) break;
                }
                if (i == Selections.Count) return;
            }

            CurrentIndex = i;
            CurrentSelection = Selections[i];
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
            Text.text = InvokeFormat(Name, ": ", CurrentSelection?.ToString() ?? string.Empty);

            _align.AlignText();

            if (invokeEvent)
            {
                InvokeChanged();
            }
        }

        public void Bind(object obj, FieldInfo field)
        {
            Changed += (self) => field.SetValue(obj, self.BoxedCurrentSelection);
        }

        protected readonly FixVerticalAlign _align;
        protected readonly List<T> Selections; 
        

        public event MenuItemChanged Changed;
        public delegate void MenuItemChanged(MenuItem<T> item);
        protected void InvokeChanged() => Changed?.Invoke(this);

        public event EventHandler<InterceptEventArgs<T>> InterceptChanged;
        protected InterceptEventArgs<T> InvokeInterceptChanged(T newValue)
        {
            InterceptEventArgs<T> args = new InterceptEventArgs<T>(CurrentSelection, newValue);
            InterceptChanged?.Invoke(this, args);
            return args;
        }


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
