using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MenuChanger.Extensions;
using MenuChanger.Attributes;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// Class which converts an object to a menu representation, consisting of MenuItems and EntryFields that access and modify its fields and properties.
    /// <br/>Can be easily used to automatically generate a menu by first creating a MenuElementFactory, and then passing its Elements array to a MenuPanel.
    /// </summary>
    public class MenuElementFactory<T>
    {
        public readonly MenuPage Parent;
        public readonly Dictionary<string, ToggleButton> BoolFields = new();
        public readonly Dictionary<string, MenuItem<Enum>> EnumFields = new();
        public readonly Dictionary<string, LongEntryField> LongFields = new();
        public readonly Dictionary<string, IntEntryField> IntFields = new();
        public readonly Dictionary<string, ShortEntryField> ShortFields = new();
        public readonly Dictionary<string, ByteEntryField> ByteFields = new();
        public readonly IMenuElement[] Elements;
        private readonly MemberInfo[] Members;

        /// <summary>
        /// Creates an MEF with elements on the given page and bound to the specified object.
        /// </summary>
        public MenuElementFactory(MenuPage page, T obj)
        {
            Parent = page;
            Members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsValidForMenu()).ToArray();
            List<IMenuElement> elements = new();

            foreach (MemberInfo mi in Members)
            {
                Type U = mi.GetValueType();

                if (U == typeof(long))
                {
                    LongEntryField entryField = new(Parent, ToDisplayName(mi.Name));
                    LongFields[mi.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (long)mi.GetValue(obj);
                    entryField.Changed += value => mi.SetValue(obj, value);
                }
                else if (U == typeof(int))
                {
                    IntEntryField entryField = new(Parent, ToDisplayName(mi.Name));
                    IntFields[mi.Name] = entryField;
                    elements.Add(entryField);
                    entryField.Bind(mi, obj);
                }
                else if (U == typeof(short))
                {
                    ShortEntryField entryField = new(Parent, ToDisplayName(mi.Name));
                    ShortFields[mi.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (short)mi.GetValue(obj);
                    entryField.Changed += value => mi.SetValue(obj, value);
                }
                else if (U == typeof(byte))
                {
                    ByteEntryField entryField = new(Parent, ToDisplayName(mi.Name));
                    ByteFields[mi.Name] = entryField;
                    elements.Add(entryField);
                    entryField.InputValue = (byte)mi.GetValue(obj);
                    entryField.Changed += value => mi.SetValue(obj, value);
                }
                else if (U == typeof(bool))
                {
                    ToggleButton button = new(Parent, ToDisplayName(mi.Name));
                    button.Bind(obj, mi);
                    BoolFields[mi.Name] = button;
                    elements.Add(button);
                    button.SetSelection((bool)mi.GetValue(obj));
                    //button.Changed += (s) => DebugMethods.DumpProperties(obj);
                }
                else if (U.IsEnum)
                {
                    MenuItem<Enum> button = new(Parent, ToDisplayName(mi.Name), Enum.GetValues(U).Cast<Enum>().ToArray());
                    button.Bind(obj, mi);
                    EnumFields[mi.Name] = button;
                    button.Format += (_, p, c, r) => (p, c, ToDisplayName(r));
                    button.SetSelection((Enum)mi.GetValue(obj));
                    elements.Add(button);
                }
            }

            foreach (var mi in Members)
            {
                foreach (var tv in mi.GetCustomAttributes<TriggerValidationAttribute>())
                {
                    if (Members.FirstOrDefault(m => m.Name == tv.memberName) is not MemberInfo m2) continue;
                    Type U = mi.GetValueType();
                    Type V = m2.GetValueType();

                    // surely there is a better way to do this than to enumerate all pairs of types
                    if (U == typeof(int) && V == typeof(int)) 
                    {
                        IntFields[mi.Name].Changed += (_) => IntFields[m2.Name].InvokeModify();
                    }
                }
            }


            Elements = elements.ToArray();
        }

        /// <summary>
        /// For each member tracked by the MEF, fetches the value from source and applies it to the corresponding element, and to target.
        /// </summary>
        public void SetMenuValues(T source, T target)
        {
            foreach (FieldInfo f in Members)
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

        private static string ToDisplayName(string name) => name.FromCamelCase();
    }
}
