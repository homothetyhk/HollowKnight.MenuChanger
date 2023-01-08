using System.ComponentModel;
using System.Reflection;
using Component = UnityEngine.Component;

namespace MenuChanger
{
    internal static class DebugMethods
    {
        internal static Vector2 MenuButtonRect(MenuButton button)
        {
            float width = 0f;
            float height = 0f;
            foreach (RectTransform rt in button.gameObject.GetComponentsInChildren<RectTransform>())
            {
                MenuChangerMod.instance.Log(rt.gameObject.name + $" {rt.rect.width * rt.localScale.x}, {rt.rect.height * rt.localScale.y}");
                width = Mathf.Max(rt.rect.width * rt.localScale.x, width);
                height = Mathf.Max(rt.rect.height * rt.localScale.y, height);
            }

            return new Vector2(width, height);
        }

        public static string BuildFullName(Transform t)
        {
            return $"{(t.parent == null ? string.Empty : BuildFullName(t.parent))}.{t.gameObject.name}";
        }

        public static void DumpComponents(GameObject obj, bool verbose = true, int indentLevel = 0)
        {
            MenuChangerMod.instance.Log(BuildFullName(obj.transform));
            foreach (Component c in obj.transform.GetComponents<Component>())
            {
                MenuChangerMod.instance.Log
                    ($"{new string(' ', indentLevel)}{c.GetType().Name}: {c.transform.name} at {c.transform.position} (local: {c.transform.localPosition})");
                if (verbose)
                {
                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(c))
                    {
                        string name = descriptor.Name;
                        object value = descriptor.GetValue(c);
                        MenuChangerMod.instance.Log($"{new string(' ', indentLevel + 4)}{descriptor.PropertyType.Name} {name}={value}");
                    }
                }
            }

            foreach (Transform child in obj.transform)
            {
                MenuChangerMod.instance.Log($"{new string(' ', indentLevel)}----");
                MenuChangerMod.instance.Log($"{new string(' ', indentLevel)}{child.gameObject.GetType().Name}: {child.gameObject.name}: {BuildFullName(child.transform)} at {child.transform.position} (local: {child.transform.localPosition})");
                DumpComponents(child.gameObject, verbose, indentLevel + 4);
            }
        }


        public static void DumpProperties(object obj)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                MenuChangerMod.instance.Log($"{descriptor.PropertyType.Name} {name}={value}");
            }
        }


        internal static void CopyRectTransform(RectTransform copyTo, RectTransform copyFrom)
        {
            foreach (PropertyInfo p in
                typeof(RectTransform).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default))
            {
                if (p.CanWrite) p.SetValue(copyTo, p.GetValue(copyFrom, null), null);
            }
        }

        internal static void CopyText(Text copyTo, Text copyFrom)
        {
            foreach (PropertyInfo p in
                typeof(RectTransform).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default))
            {
                if (p.CanWrite) p.SetValue(copyTo, p.GetValue(copyFrom, null), null);
            }
        }



        internal static Type[] badTypes = { typeof(Component), typeof(Transform), typeof(GameObject) };
        internal static void CopyComponent(Type T, Component from, Component to, bool verbose = true)
        {
            if (verbose)
            {
                MenuChangerMod.instance.Log($"Copying component of type {T.Name}");
            }
            foreach (PropertyInfo p in
                T.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default).Where(t => t.CanWrite))
            {
                if (badTypes.Any(t => p.PropertyType.IsAssignableFrom(t)))
                {
                    if (verbose)
                    {
                        MenuChangerMod.instance.Log($"    Skipping property {p.PropertyType.Name} {p.Name} of {T.Name}");
                    }
                }
                else
                {
                    p.SetValue(to, p.GetValue(from, null), null);
                }
            }
        }

        public static void CopyComponents(GameObject from, GameObject to)
        {
            MenuChangerMod.instance.Log($"Copying {from.name}");
            foreach (Component c in from.transform.GetComponents<Component>())
            {
                CopyComponent(c.GetType(), c, to.AddComponent(c.GetType()));
            }

            foreach (Transform child in from.transform)
            {
                MenuChangerMod.instance.Log(from.transform.childCount);
                GameObject g = new GameObject(child.gameObject.name);
                g.transform.SetParent(to.transform);
                CopyComponents(child.gameObject, g);
            }
            MenuChangerMod.instance.Log($"Finished copying {from.name}");
        }

    }
}
