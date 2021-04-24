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
        public Dictionary<string, NumericEntryField> IntFields = new Dictionary<string, NumericEntryField>();
        public IMenuElement[] Elements;

        public MenuElementFactory(MenuPage page, T obj)
        {
            Parent = page;
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<IMenuElement> elements = new List<IMenuElement>();

            foreach (FieldInfo f in fields)
            {
                Type U = f.FieldType;
                if (U == typeof(int))
                {
                    NumericEntryField entryField = new NumericEntryField(Parent, ToDisplayName(f.Name));
                    IntFields[f.Name] = entryField;
                    elements.Add(entryField);
                    // bind??
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

        public static string ToDisplayName(string name)
        {
            StringBuilder uiname = new StringBuilder(name);
            if (name.Length > 0)
            {
                uiname[0] = char.ToUpper(uiname[0]);
            }
            
            for (int i = 1; i < uiname.Length; i++)
            {
                if (char.IsUpper(uiname[i]) && char.IsLower(uiname[i - 1]))
                {
                    uiname.Insert(i, " ");
                }

                if (char.IsDigit(uiname[i]) && !char.IsDigit(uiname[i - 1]) && !char.IsWhiteSpace(uiname[i-1]))
                {
                    uiname.Insert(i, " ");
                }
            }

            return uiname.ToString();
        }
    }
}
