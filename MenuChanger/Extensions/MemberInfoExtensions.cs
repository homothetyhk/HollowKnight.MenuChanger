using System;
using System.Reflection;
using MenuChanger.Attributes;

namespace MenuChanger.Extensions
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Tests whether the member is a field or property which allows read and write operations, and also is either public with no MenuIgnoreAttribute or has a MenuIncludeAttribute.
        /// </summary>
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

        /// <summary>
        /// Gets the value from the field or property of the member.
        /// </summary>
        /// <exception cref="NotImplementedException">The MemberInfo is not a FieldInfo or a PropertyInfo.</exception>
        public static object GetValue(this MemberInfo mi, object o)
        {
            return mi.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)mi).GetValue(o),
                MemberTypes.Property => ((PropertyInfo)mi).GetValue(o),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Sets the value of the field or property of the member.
        /// </summary>
        /// <exception cref="NotImplementedException">The MemberInfo is not a FieldInfo or a PropertyInfo.</exception>
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

        /// <summary>
        /// Gets the type of the field or property of the member.
        /// </summary>
        /// <exception cref="NotImplementedException">The MemberInfo is not a FieldInfo or a PropertyInfo</exception>
        public static Type GetValueType(this MemberInfo mi)
        {
            return mi.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)mi).FieldType,
                MemberTypes.Property => ((PropertyInfo)mi).PropertyType,
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Returns true if the type is one of the primitive numeric types.
        /// </summary>
        public static bool IsNumeric(this Type T)
        {
            if (T.IsEnum) return false;
            return Type.GetTypeCode(T) switch
            {
                TypeCode.SByte or TypeCode.Byte or TypeCode.Int16
                or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64
                or TypeCode.Single or TypeCode.Double or TypeCode.Decimal => true,
                _ => false
            };
        }

    }
}
