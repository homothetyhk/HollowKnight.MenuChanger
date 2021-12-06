using System.Collections;
using System.Reflection;
using MenuChanger.Extensions;
using MenuChanger.Attributes;

namespace MenuChanger.MenuElements
{
    public static class MenuElementFactory
    {
        private static readonly Dictionary<(Type gen, Type par), Type> genericTypeLookup = new();

        /// <summary>
        /// Returns true if an IValueElement was successfully created for the given field or property.
        /// </summary>
        public static bool TryGetValueElement(MenuPage page, MemberInfo mi, out IValueElement element)
        {
            Type U = mi.GetValueType();

            if (U == typeof(bool))
            {
                element = new ToggleButton(page, mi.GetMenuName());
                return true;
            }
            else if (U.IsEnum)
            {
                Type M = typeof(MenuEnum<>);
                if (!genericTypeLookup.TryGetValue((M, U), out Type T))
                {
                    T = genericTypeLookup[(M, U)] = M.MakeGenericType(U);
                }
                element = (MenuItem)Activator.CreateInstance(T, page, mi.GetMenuName());
                return true;
            }
            else if (U.IsNumeric())
            {
                Type N = typeof(NumericEntryField<>);
                if (!genericTypeLookup.TryGetValue((N, U), out Type T))
                {
                    T = genericTypeLookup[(N, U)] = N.MakeGenericType(U);
                }
                element = (EntryField)Activator.CreateInstance(T, page, mi.GetMenuName());
                return true;
            }

            element = null;
            return false;
        }
    }


    /// <summary>
    /// Class which converts an object to a menu representation, consisting of MenuItems and EntryFields that access and modify its fields and properties.
    /// <br/>Can be easily used to automatically generate a menu by first creating a MenuElementFactory, and then passing its Elements array to a MenuPanel.
    /// </summary>
    public class MenuElementFactory<T>
    {
        public readonly MenuPage Parent;
        public readonly Dictionary<string, IValueElement> ElementLookup = new();
        public readonly IValueElement[] Elements;
        private readonly MemberInfo[] Members;

        /// <summary>
        /// Creates an MEF with elements on the given page and bound to the specified object.
        /// </summary>
        public MenuElementFactory(MenuPage page, T obj)
        {
            Parent = page;
            Members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsValidForMenu()).ToArray();
            List<IValueElement> elements = new();

            foreach (MemberInfo mi in Members)
            {
                Type U = mi.GetValueType();

                if (MenuElementFactory.TryGetValueElement(page, mi, out IValueElement element))
                {
                    elements.Add(element);
                    ElementLookup.Add(mi.Name, element);
                    element.Bind(obj, mi);
                }
            }

            foreach (var mi in Members)
            {
                foreach (var tv in mi.GetCustomAttributes<TriggerValidationAttribute>())
                {
                    if (Members.FirstOrDefault(m => m.Name == tv.memberName) is not MemberInfo m2
                        || !ElementLookup.TryGetValue(mi.Name, out IValueElement ve1)
                        || !ElementLookup.TryGetValue(m2.Name, out IValueElement ve2)) continue;
                    ve1.SelfChanged += _ => ve2.SetValue(ve2.Value);
                }
            }


            Elements = elements.ToArray();
        }

        /// <summary>
        /// For each member tracked by the MEF, fetches the value from source and applies it to the corresponding element.
        /// </summary>
        public void SetMenuValues(T source)
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                Elements[i].SetValue(Members[i].GetValue(source));
            }
        }
    }
}
