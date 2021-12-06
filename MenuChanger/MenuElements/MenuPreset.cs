using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MenuChanger.Extensions;

namespace MenuChanger.MenuElements
{
    public interface IMenuPreset
    {
        MenuLabel Label { get; }

        void UpdatePreset();
    }


    public class MenuPreset<T> : MenuItem<string>, IMenuPreset
    {
        public readonly Dictionary<string, T> Ts;
        public readonly MemberInfo[] TMembers;

        public readonly T Obj;
        public MenuLabel Label { get; }
        public readonly Func<T, string> Caption;

        private bool isUpdating = false;
        private bool broadcastPreset = true;

        public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj,
            Func<T, string> caption)
            : base(page, prefix, dict.Keys.ToArray())
        {
            Ts = dict;
            TMembers = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(mi => mi.IsValidForMenu()).ToArray();

            Obj = obj;

            SelfChanged += SetPreset;

            Caption = caption;
            Label = new MenuLabel(page, caption(Ts[Value]), MenuLabel.Style.Body);

            // evil code
            Label.GameObject.transform.SetParent(GameObject.transform);
            Label.GameObject.transform.localPosition = new Vector3(0, -25, 0);
            Label.GameObject.transform.localScale *= 0.7f;

            Label.Text.alignment = TextAnchor.MiddleCenter;
            SelfChanged += s => UpdateCaption();

            UpdatePreset();
        }

        public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj, 
            Func<T, string> caption, MenuElementFactory<T> factory)
            : this(page, prefix, dict, obj, caption)
        {
            Pair(factory);
        }

        public void UpdateCaption()
        {
            Label.Text.text = Caption != null ? Caption(Obj) : string.Empty;
        }

        public void SetValueWithoutBroadcast(string key)
        {
            broadcastPreset = false;
            SetValue(key);
            broadcastPreset = true;
        }

        protected override void SetValueInternal(object o, bool invokeEvent = true)
        {
            isUpdating = true;
            base.SetValueInternal(o, invokeEvent);
            isUpdating = false;
        }

        public void SetPreset(IValueElement self) => SetPreset((string)self.Value);

        public void SetPreset(string key)
        {
            if (!broadcastPreset) return;
            
            if (Ts.TryGetValue(key, out T t))
            {
                foreach (MemberInfo mi in TMembers) mi.SetValue(Obj, mi.GetValue(t));
                OnSetPreset?.Invoke(t);
            }
        }

        public void Pair(IValueElement element, MemberInfo mi)
        {
            element.SelfChanged += _ => UpdatePreset();
            OnSetPreset += t =>
            {
                element.SetValue(mi.GetValue(t));
            };
        }

        public void Pair(RadioSwitch radioSwitch, MemberInfo mi)
        {
            radioSwitch.Changed += _ => UpdatePreset();
            OnSetPreset += t => radioSwitch.TrySelect((string)mi.GetValue(t));
        }

        public void Pair(MenuElementFactory<T> factory)
        {
            foreach (MemberInfo mi in TMembers)
            {
                if (factory.ElementLookup.TryGetValue(mi.Name, out IValueElement e))
                {
                    Pair(e, mi);
                }
            }
        }

        public void UpdatePreset(string s) => UpdatePreset();

        int counter = 0;
        public void UpdatePreset()
        {
            if (isUpdating) return;

            foreach(string key in Items)
            {
                if (CheckPreset(key))
                {
                    if (Value != key) SetValueWithoutBroadcast(key);
                    return;
                }
            }

            if (Value != "Custom") SetValueWithoutBroadcast("Custom");
        }

        public bool CheckPreset(string key)
        {
            return Ts.TryGetValue(key, out T t) && TMembers.All(mi => Equals(mi.GetValue(t), mi.GetValue(Obj)));
        }

        public event Action<T> OnSetPreset;
    }
}
