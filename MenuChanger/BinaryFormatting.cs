using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace MenuChanger
{
    public static class BinaryFormatting
    {
        /*
        public class Serializer
        {
            FieldInfo[] BoolFields;
            FieldInfo[] IntFields;
            FieldInfo[] EnumFields;
            (FieldInfo, Serializer)[] NestedFields;


            public Serializer(Type T)
            {
                FieldInfo[] Fields = T.GetFields(BindingFlags.Public | BindingFlags.Instance);
                BoolFields = Fields.Where(f => f.FieldType == typeof(bool)).OrderBy(f => f.Name).ToArray();
                IntFields = Fields.Where(f => f.FieldType == typeof(int)).OrderBy(f => f.Name).ToArray();
                EnumFields = Fields.Where(f => f.FieldType.IsEnum).OrderBy(f => f.Name).ToArray();
                NestedFields = Fields.Where(
                f => !PrimitiveType(f.FieldType) && (f.FieldType.Attributes & TypeAttributes.Serializable) != 0)
                    .Select(f => (f, new Serializer(f.FieldType))).ToArray();
            }

            public string Serialize(object t)
            {
                string code = string.Empty;

                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    foreach (int i in intFields) writer.Write(i);
                    foreach (int i in enumFields) writer.Write(i);
                    foreach (byte b in ConvertBoolArrayToByteArray(boolFields)) writer.Write(b);
                    writer.Close();

                    code = Convert.ToBase64String(stream.ToArray());
                }

                foreach (FieldInfo field in Fields.Where(
                    f => !PrimitiveType(f.FieldType) && (f.FieldType.Attributes & TypeAttributes.Serializable) != 0))
                {
                    if (field.GetValue(t) is object p)
                    {
                        code += $"-{Serialize(p)}";
                    }
                    else
                    {
                        MenuChanger.instance.LogWarn($"Unable to serialize null {field.Name} in {t.GetType().Name}");
                    }
                }

                MenuChanger.instance.Log($"Computed Serialization of {t.GetType().Name} as {code}");
                return code;
            }
        }
        */


        // uncached!
        public static string Serialize(object o)
        {
            FieldInfo[] fieldInfos = o.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            int[] intFields = GetIntFields(o, fieldInfos);
            int[] enumFields = GetEnumFields(o, fieldInfos);
            bool[] boolFields = GetBoolFields(o, fieldInfos);

            string code = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                foreach (int i in intFields) writer.Write(i);
                foreach (int i in enumFields) writer.Write(i);
                foreach (byte b in ConvertBoolArrayToByteArray(boolFields)) writer.Write(b);
                writer.Close();

                code = Convert.ToBase64String(stream.ToArray());
            }

            foreach (FieldInfo field in fieldInfos.Where(
                f => !PrimitiveType(f.FieldType) && (f.FieldType.Attributes & TypeAttributes.Serializable) != 0))
            {
                if (field.GetValue(o) is object p)
                {
                    code += $"-{Serialize(p)}";
                }
                else
                {
                    MenuChanger.instance.LogWarn($"Unable to serialize null {field.Name} in {o.GetType().Name}");
                }
            }

            //MenuChanger.instance.Log($"Computed Serialization of {o.GetType().Name} as {code}");
            return code;
        }

        public static object Deserialize(string code, object o)
        {
            FieldInfo[] fieldInfos = o.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] intFields = fieldInfos.Where(f => f.FieldType == typeof(int)).OrderBy(f => f.Name).ToArray();
            FieldInfo[] enumFields = fieldInfos.Where(f => f.FieldType.IsEnum).OrderBy(f => f.Name).ToArray();
            FieldInfo[] boolFields = fieldInfos.Where(f => f.FieldType == typeof(bool)).OrderBy(f => f.Name).ToArray();

            byte[] bytes = Convert.FromBase64String(code);
            using (MemoryStream stream = new MemoryStream(bytes))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                foreach (FieldInfo field in intFields)
                {
                    try
                    {
                        field.SetValue(o, reader.ReadInt32());
                    }
                    catch (Exception e)
                    {
                        MenuChanger.instance.Log("Error in deserializing intFields: " + e);
                        break;
                    }
                }
                foreach (FieldInfo field in enumFields)
                {
                    try
                    {
                        field.SetValue(o, reader.ReadInt32());
                    }
                    catch (Exception e)
                    {
                        MenuChanger.instance.Log("Error in deserializing enumFields: " + e);
                        break;
                    }
                }

                bool[] bools = ConvertByteArrayToBoolArray(reader.ReadBytes(bytes.Length - (int)stream.Position));
                for (int i = 0; i < boolFields.Length; i++)
                {
                    try
                    {
                        boolFields[i].SetValue(o, bools[i]);
                    }
                    catch (Exception e)
                    {
                        MenuChanger.instance.Log("Error in deserializing boolFields: " + e);
                        break;
                    }
                }
            }

            return o;
        }

        public static bool PrimitiveType(Type T)
        {
            return T == typeof(int) || T == typeof(bool) || T.IsEnum;
        }

        public static int[] GetIntFields(object o, FieldInfo[] fieldInfos)
        {
            return fieldInfos.Where(f => f.FieldType == typeof(int))
                .OrderBy(f => f.Name)
                .Select(f => (int)f.GetValue(o))
                .ToArray();
        }

        public static int[] GetEnumFields(object o, FieldInfo[] fieldInfos)
        {
            return fieldInfos.Where(f => f.FieldType.IsEnum)
                .OrderBy(f => f.Name)
                .Select(f => (int)f.GetValue(o)) // we assume all enums are backed by ints!
                .ToArray();
        }

        public static bool[] GetBoolFields(object o, FieldInfo[] fieldInfos)
        {
            return fieldInfos.Where(f => f.FieldType == typeof(bool))
                .OrderBy(f => f.Name)
                .Select(f => (bool)f.GetValue(o))
                .ToArray();
        }

        public static bool[] ConvertByteArrayToBoolArray(byte[] bytes)
        {
            BitArray bits = new BitArray(bytes);
            bool[] bools = new bool[bits.Count];
            bits.CopyTo(bools, 0);
            return bools;
        }

        public static byte[] ConvertBoolArrayToByteArray(bool[] boolArr)
        {
            BitArray bits = new BitArray(boolArr);
            byte[] bytes = new byte[bits.Length / 8 + 1];
            if (bits.Length > 0)
            {
                bits.CopyTo(bytes, 0);
            }
            
            return bytes;
        }

    }
}
