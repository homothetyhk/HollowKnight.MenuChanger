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

        public MenuPreset(MenuPage page, string prefix, Dictionary<string, T> dict, T obj, bool checkPreset)
            : base(page, prefix, dict.Keys.ToArray())
        {
            Ts = dict;
            Tfields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            Obj = obj;

            Changed += SetPreset;
            if (checkPreset)
            {
                //base.GameObject.AddComponent<Components.Updater>().action += UpdatePreset;
            }
            Changed += (s) => DebugMethods.DumpProperties(Obj);
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
                    toggleButton.Changed += UpdatePreset;
                    Changed += (s) => toggleButton.SetSelection((bool)field.GetValue(Obj), true);
                }
                else if (field.FieldType == typeof(int) && factory.IntFields.TryGetValue(field.Name, out NumericEntryField intField))
                {
                    intField.InputField.onValueChanged.AddListener(UpdatePreset);
                    Changed += (s) => intField.InputValue = (int)field.GetValue(Obj);
                }
                else if (field.FieldType.IsEnum)
                {
                    if (factory.EnumFields.TryGetValue(field.Name, out MenuItem<Enum> enumButton))
                    {
                        enumButton.Changed += UpdatePreset;
                        Changed += (s) => enumButton.TrySetSelection(field.GetValue(Obj), true);
                    }
                }
            }
        }

        public void UpdatePreset(IMenuItem item) => UpdatePreset();
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
