using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
        public readonly FieldInfo[] Tfields;
        public readonly T Obj;
        public MenuLabel Label { get; }
        public readonly Func<T, string> Caption;

        private bool isUpdating = false;

        public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj,
            Func<T, string> caption)
            : base(page, prefix, dict.Keys.ToArray())
        {
            Ts = dict;
            Tfields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            Obj = obj;

            Changed += SetPreset;

            Caption = caption;
            Label = new MenuLabel(page, caption(Ts[CurrentSelection]), MenuLabel.Style.Body);

            // evil code
            Label.GameObject.transform.SetParent(GameObject.transform);
            Label.GameObject.transform.localPosition = new UnityEngine.Vector3(0, -25, 0);
            Label.GameObject.transform.localScale *= 0.7f;

            Label.Text.alignment = UnityEngine.TextAnchor.MiddleCenter;
            Changed += s => UpdateCaption();

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

        protected override void RefreshText(bool invokeEvent = true)
        {
            isUpdating = true;
            base.RefreshText(invokeEvent);
            isUpdating = false;
        }

        public void SetPreset(MenuItem<string> self) => SetPreset(self.CurrentSelection);

        public void SetPreset(string key)
        {
            if (Ts.TryGetValue(key, out T t))
            {
                foreach (FieldInfo field in Tfields) field.SetValue(Obj, field.GetValue(t));
            }
        }

        public void Pair(ToggleButton toggleButton, FieldInfo field)
        {
            toggleButton.Changed += _ => UpdatePreset();
            Changed += _ => toggleButton.SetSelection((bool)field.GetValue(Obj));
        }

        public void Pair(ByteEntryField byteField, FieldInfo field)
        {
            byteField.Changed += _ => UpdatePreset();
            Changed += _ => byteField.InputValue = (byte)field.GetValue(Obj);
        }

        public void Pair(ShortEntryField shortField, FieldInfo field)
        {
            shortField.Changed += _ => UpdatePreset();
            Changed += _ => shortField.InputValue = (short)field.GetValue(Obj);
        }

        public void Pair(IntEntryField intField, FieldInfo field)
        {
            intField.Changed += _ => UpdatePreset();
            Changed += _ => intField.InputValue = (int)field.GetValue(Obj);
        }

        public void Pair(LongEntryField longField, FieldInfo field)
        {
            longField.Changed += _ => UpdatePreset();
            Changed += _ => longField.InputValue = (long)field.GetValue(Obj);
        }

        public void Pair(MenuItem<Enum> enumButton, FieldInfo field)
        {
            enumButton.Changed += _ => UpdatePreset();
            Changed += _ => enumButton.TrySetSelection(field.GetValue(Obj));
        }

        public void Pair<U>(MenuItem<U> menuItem, FieldInfo field)
        {
            menuItem.Changed += _ => UpdatePreset();
            Changed += _ => menuItem.TrySetSelection(field.GetValue(Obj));
        }

        public void Pair(RadioSwitch radioSwitch, FieldInfo field)
        {
            radioSwitch.Changed += _ => UpdatePreset();
            Changed += _ => radioSwitch.TrySelect((string)field.GetValue(Obj));
        }

        public void Pair(MenuElementFactory<T> factory)
        {
            foreach (FieldInfo field in Tfields)
            {
                if (field.FieldType == typeof(bool) && factory.BoolFields.TryGetValue(field.Name, out ToggleButton toggleButton))
                {
                    Pair(toggleButton, field);
                }
                else if (field.FieldType == typeof(byte) && factory.ByteFields.TryGetValue(field.Name, out ByteEntryField byteField))
                {
                    Pair(byteField, field);
                }
                else if (field.FieldType == typeof(short) && factory.ShortFields.TryGetValue(field.Name, out ShortEntryField shortField))
                {
                    Pair(shortField, field);
                }
                else if (field.FieldType == typeof(int) && factory.IntFields.TryGetValue(field.Name, out IntEntryField intField))
                {
                    Pair(intField, field);
                }
                else if (field.FieldType == typeof(long) && factory.LongFields.TryGetValue(field.Name, out LongEntryField longField))
                {
                    Pair(longField, field);
                }
                else if (field.FieldType.IsEnum)
                {
                    if (factory.EnumFields.TryGetValue(field.Name, out MenuItem<Enum> enumButton))
                    {
                        Pair(enumButton, field);
                    }
                }
            }
        }

        public void UpdatePreset(string s) => UpdatePreset();

        public void UpdatePreset()
        {
            if (isUpdating) return;
            
            foreach(string key in Selections)
            {
                if (CheckPreset(key))
                {
                    if (CurrentSelection != key) SetSelection(key);
                    return;
                }
            }

            if (CurrentSelection != "Custom") SetSelection("Custom");
        }

        public bool CheckPreset(string key)
        {
            return Ts.TryGetValue(key, out T t) && Tfields.All(f => Equals(f.GetValue(t), f.GetValue(Obj)));
        }
    }
}
