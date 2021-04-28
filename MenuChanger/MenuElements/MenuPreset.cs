using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MenuChanger.MenuElements
{
    public class MenuPreset<T> : MenuItem<string>
    {
        public readonly Dictionary<string, T> Ts;
        public readonly FieldInfo[] Tfields;
        public readonly T Obj;
        public readonly MenuLabel Label;
        public readonly Func<T, string> Labeller;

        public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj, 
            MenuElementFactory<T> factory, Func<T, string> labeller)
            : base(page, prefix, dict.Keys.ToArray())
        {
            Ts = dict;
            Tfields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            Obj = obj;

            Changed += SetPreset;

            Pair(factory);
            Labeller = labeller;
            Label = new MenuLabel(page, labeller(Ts[currentSelection]), MenuLabel.Style.Body);

            // evil code
            Label.GameObject.transform.SetParent(GameObject.transform);
            Label.GameObject.transform.localPosition = new UnityEngine.Vector3(0, -25, 0);
            Label.GameObject.transform.localScale *= 0.7f;

            Label.Text.alignment = UnityEngine.TextAnchor.MiddleCenter;
            Changed += s => Label.Text.text = Labeller(Ts[s.CurrentSelection]);

            SetPreset(this);
        }

        public void SetPreset(MenuItem<string> self) => SetPreset(self.CurrentSelection);

        public void SetPreset(string key)
        {
            if (Ts.TryGetValue(key, out T t))
            {
                foreach (FieldInfo field in Tfields) field.SetValue(Obj, field.GetValue(t));
            }
        }

        public void Pair(MenuElementFactory<T> factory)
        {
            foreach (FieldInfo field in Tfields)
            {
                if (field.FieldType == typeof(bool) && factory.BoolFields.TryGetValue(field.Name, out ToggleButton toggleButton))
                {
                    toggleButton.Changed += _ => UpdatePreset();
                    Changed += _ => toggleButton.SetSelection((bool)field.GetValue(Obj), true);
                }
                else if (field.FieldType == typeof(byte) && factory.ByteFields.TryGetValue(field.Name, out ByteEntryField byteField))
                {
                    byteField.Changed += _ => UpdatePreset();
                    Changed += _ => byteField.InputValue = (byte)field.GetValue(Obj);
                }
                else if (field.FieldType == typeof(short) && factory.ShortFields.TryGetValue(field.Name, out ShortEntryField shortField))
                {
                    shortField.Changed += _ => UpdatePreset();
                    Changed += _ => shortField.InputValue = (short)field.GetValue(Obj);
                }
                else if (field.FieldType == typeof(int) && factory.IntFields.TryGetValue(field.Name, out IntEntryField intField))
                {
                    intField.Changed += _ => UpdatePreset();
                    Changed += _ => intField.InputValue = (int)field.GetValue(Obj);
                }
                else if (field.FieldType == typeof(long) && factory.LongFields.TryGetValue(field.Name, out LongEntryField longField))
                {
                    longField.Changed += _ => UpdatePreset();
                    Changed += _ => longField.InputValue = (long)field.GetValue(Obj);
                }
                else if (field.FieldType.IsEnum)
                {
                    if (factory.EnumFields.TryGetValue(field.Name, out MenuItem<Enum> enumButton))
                    {
                        enumButton.Changed += _ => UpdatePreset();
                        Changed += _ => enumButton.TrySetSelection(field.GetValue(Obj), true);
                    }
                }
            }
        }

        public void UpdatePreset(string s) => UpdatePreset();

        public void UpdatePreset()
        {
            foreach(string key in Selections)
            {
                if (CheckPreset(key))
                {
                    if (CurrentSelection != key) SetSelection(key, true);
                    return;
                }
            }

            if (CurrentSelection != "Custom") SetSelection("Custom", true);
        }

        public bool CheckPreset(string key)
        {
            return Ts.TryGetValue(key, out T t) && Tfields.All(f => f.GetValue(t).Equals(f.GetValue(Obj)));
        }

    }
}
