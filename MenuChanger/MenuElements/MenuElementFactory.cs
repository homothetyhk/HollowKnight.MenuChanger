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

        public MenuElementFactory(MenuPage page, T obj)
        {
            Parent = page;
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<IMenuElement> elements = new List<IMenuElement>();

            foreach (FieldInfo f in fields)
            {
                Type U = f.FieldType;

                if (U == typeof(long))
                {
                    LongEntryField entryField = new LongEntryField(Parent, ToDisplayName(f.Name));
                    LongFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(int))
                {
                    IntEntryField entryField = new IntEntryField(Parent, ToDisplayName(f.Name));
                    IntFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(short))
                {
                    ShortEntryField entryField = new ShortEntryField(Parent, ToDisplayName(f.Name));
                    ShortFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(byte))
                {
                    ByteEntryField entryField = new ByteEntryField(Parent, ToDisplayName(f.Name));
                    ByteFields[f.Name] = entryField;
                    elements.Add(entryField);
                    entryField.Changed += value => f.SetValue(obj, value);
                }
                else if (U == typeof(bool))
                {
                    ToggleButton button = new ToggleButton(Parent, ToDisplayName(f.Name));
                    button.Bind(obj, f);
                    BoolFields[f.Name] = button;
                    elements.Add(button);
                    button.Changed += (s) => DebugMethods.DumpProperties(obj);
                }
                else if (U.IsEnum)
                {
                    MenuItem<Enum> button = new MenuItem<Enum>(Parent, ToDisplayName(f.Name), Enum.GetValues(U).Cast<Enum>().ToArray());
                    button.Bind(obj, f);
                    EnumFields[f.Name] = button;
                    button.Format += (_, p, c, r) => (p, c, ToDisplayName(r));
                    elements.Add(button);
                }
            }

            Elements = elements.ToArray();
        }

        public static string ToDisplayName(string name) => name.FromCamelCase();
    }
}
