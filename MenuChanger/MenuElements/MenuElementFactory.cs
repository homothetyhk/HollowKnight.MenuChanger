using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MenuChanger.MenuElements
{
    public class MenuElementFactory<T>
    {
        public MenuPage Parent;
        public Dictionary<string, ToggleButton> BoolFields = new Dictionary<string, ToggleButton>();
        public Dictionary<string, MenuItem<Enum>> EnumFields = new Dictionary<string, MenuItem<Enum>>();
        public Dictionary<string, LongEntryField> LongFields = new Dictionary<string, LongEntryField>();
        public Dictionary<string, IntEntryField> IntFields = new Dictionary<string, IntEntryField>();
        public Dictionary<string, ShortEntryField> ShortFields = new Dictionary<string, ShortEntryField>();
        public Dictionary<string, ByteEntryField> ByteFields = new Dictionary<string, ByteEntryField>();
        public IMenuElement[] Elements;
        private FieldInfo[] Fields;

        public MenuElementFactory(MenuPage page, T obj)
        {
            Parent = page;
            Fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<IMenuElement> elements = new List<IMenuElement>();

            foreach (FieldInfo f in Fields)
            {
                Type U = f.FieldType;

                if (U == typeof(long))
                {
                    LongEntryField entryField = new LongEntryField(Parent, ToDisplayName(f.Name));
                    LongFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (long)f.GetValue(obj);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(int))
                {
                    IntEntryField entryField = new IntEntryField(Parent, ToDisplayName(f.Name));
                    IntFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (int)f.GetValue(obj);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(short))
                {
                    ShortEntryField entryField = new ShortEntryField(Parent, ToDisplayName(f.Name));
                    ShortFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (short)f.GetValue(obj);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(byte))
                {
                    ByteEntryField entryField = new ByteEntryField(Parent, ToDisplayName(f.Name));
                    ByteFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (byte)f.GetValue(obj);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(bool))
                {
                    ToggleButton button = new ToggleButton(Parent, ToDisplayName(f.Name));
                    button.Bind(obj, f);
                    BoolFields[f.Name] = button;
                    elements.Add(button);
                    button.SetSelection((bool)f.GetValue(obj));
                    //button.Changed += (s) => DebugMethods.DumpProperties(obj);
                }
                else if (U.IsEnum)
                {
                    MenuItem<Enum> button = new MenuItem<Enum>(Parent, ToDisplayName(f.Name), Enum.GetValues(U).Cast<Enum>().ToArray());
                    button.Bind(obj, f);
                    EnumFields[f.Name] = button;
                    button.Format += (_, p, c, r) => (p, c, ToDisplayName(r));
                    button.SetSelection((Enum)f.GetValue(obj));
                    elements.Add(button);
                }
            }

            Elements = elements.ToArray();
        }

        public void SetMenuValues(T source, T target)
        {
            foreach (FieldInfo f in Fields)
            {
                Type U = f.FieldType;

                if (U == typeof(long))
                {
                    if (LongFields.TryGetValue(f.Name, out var val))
                    {
                        long i = (long)f.GetValue(source);
                        val.InputValue = i;
                        f.SetValue(target, i);
                    }
                }
                else if (U == typeof(int))
                {
                    if (IntFields.TryGetValue(f.Name, out var val))
                    {
                        int i = (int)f.GetValue(source);
                        val.InputValue = i;
                        f.SetValue(target, i);
                    }
                }
                else if (U == typeof(short))
                {
                    if (ShortFields.TryGetValue(f.Name, out var val))
                    {
                        short i = (short)f.GetValue(source);
                        val.InputValue = i;
                        f.SetValue(target, i);
                    }
                }
                else if (U == typeof(byte))
                {
                    if (ByteFields.TryGetValue(f.Name, out var val))
                    {
                        byte i = (byte)f.GetValue(source);
                        val.InputValue = i;
                        f.SetValue(target, i);
                    }
                }
                else if (U == typeof(bool))
                {
                    if (BoolFields.TryGetValue(f.Name, out var val))
                    {
                        bool i = (bool)f.GetValue(source);
                        val.SetSelection(i);
                        f.SetValue(target, i);
                    }
                }
                else if (U.IsEnum)
                {
                    if (EnumFields.TryGetValue(f.Name, out var val))
                    {
                        Enum i = (Enum)f.GetValue(source);
                        val.SetSelection(i);
                        f.SetValue(target, i);
                    }
                }
            }
        }

        public static string ToDisplayName(string name) => name.FromCamelCase();
    }
}
