using System;
using System.Reflection;
using MenuChanger.Attributes;

namespace MenuChanger.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool IsValidForMenu(this MemberInfo mi)
        {
            if (mi.MemberType == MemberTypes.Field)
            {
                FieldInfo fi = (FieldInfo)mi;
                if (fi.IsInitOnly) return false;

                if (fi.IsPublic && !Attribute.IsDefined(fi, typeof(MenuIgnoreAttribute)) || Attribute.IsDefined(fi, typeof(MenuIncludeAttribute))) return true;
                return false;
            }
            else if (mi.MemberType == MemberTypes.Property)
            {
                PropertyInfo pi = (PropertyInfo)mi;
                if (!pi.CanRead || !pi.CanWrite) return false;

                if (pi.GetMethod.IsPublic && !Attribute.IsDefined(pi, typeof(MenuIgnoreAttribute)) || Attribute.IsDefined(pi, typeof(MenuIncludeAttribute))) return true;
                return false;
            }

            return false;
        }

        public static string GetMenuName(this MemberInfo mi)
        {
            if (Attribute.GetCustomAttribute(mi, typeof(MenuLabelAttribute)) is MenuLabelAttribute labelAttribute)
            {
                return labelAttribute.text;
            }
            else
            {
                return mi.Name.FromCamelCase();
            }
        }

        public static bool TryGetMenuDescription(this MemberInfo mi, out string desc)
        {
            if (Attribute.GetCustomAttribute(mi, typeof(MenuDescriptionAttribute)) is MenuDescriptionAttribute descAttribute)
            {
                desc = descAttribute.text;
                return true;
            }
            else
            {
                desc = null;
                return false;
            }
        }

        public static bool TryGetMenuSummary(this MemberInfo mi, out string summary)
        {
            if (Attribute.GetCustomAttribute(mi, typeof(MenuSummaryAttribute)) is MenuSummaryAttribute descAttribute)
            {
                summary = descAttribute.text;
                return true;
            }
            else
            {
                summary = null;
                return false;
            }
        }

        public static object GetValue(this MemberInfo mi, object o)
        {
            return mi.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)mi).GetValue(o),
                MemberTypes.Property => ((PropertyInfo)mi).GetValue(o),
                _ => throw new NotImplementedException(),
            };
        }

        public static void SetValue(this MemberInfo mi, object o, object value)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)mi).SetValue(o, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)mi).SetValue(o, value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Type GetValueType(this MemberInfo mi)
        {
            return mi.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)mi).FieldType,
                MemberTypes.Property => ((PropertyInfo)mi).PropertyType,
                _ => throw new NotImplementedException(),
            };
        }

    }
}
