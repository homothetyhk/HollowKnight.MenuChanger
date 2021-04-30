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
        static Dictionary<Type, (FieldInfo[] numerics, FieldInfo[] bools)> FieldCache = 
            new Dictionary<Type, (FieldInfo[] numerics, FieldInfo[] bools)>();

        static Type[] NumericTypes = new Type[]
        {
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(byte),
        };

        public static void WriteNumeric(this BinaryWriter writer, Type F, object box)
        {
            if (F == typeof(int))
            {
                writer.Write((int)box);
            }
            else if (F == typeof(long))
            {
                writer.Write((long)box);
            }
            else if (F == typeof(short))
            {
                writer.Write((short)box);
            }
            else if (F == typeof(byte))
            {
                writer.Write((byte)box);
            }
        }

        public static object ReadNumeric(this BinaryReader reader, Type F)
        {
            if (F == typeof(int))
            {
                return reader.ReadInt32();
            }
            else if (F == typeof(long))
            {
                return reader.ReadInt64();
            }
            else if (F == typeof(short))
            {
                return reader.ReadInt16();
            }
            else if (F == typeof(byte))
            {
                return reader.ReadByte();
            }
            return null;
        }

        public static string Serialize(object o)
        {
            Type T = o.GetType();

            FieldInfo[] numericFields;
            FieldInfo[] boolFields;
            if (FieldCache.TryGetValue(T, out var pair))
            {
                numericFields = pair.numerics;
                boolFields = pair.bools;
            }
            else
            {
                FieldInfo[] fields = T.GetFields(BindingFlags.Public | BindingFlags.Instance);
                numericFields = fields.Where(f => NumericTypes.Contains(f.FieldType) || f.FieldType.IsEnum).OrderBy(f => f.Name).ToArray();
                boolFields = fields.Where(f => f.FieldType == typeof(bool)).OrderBy(f => f.Name).ToArray();
                FieldCache[T] = (numericFields, boolFields);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                foreach (FieldInfo f in numericFields)
                {
                    Type F = f.FieldType;
                    if (F.IsEnum)
                    {
                        F = Enum.GetUnderlyingType(F);
                    }
                    object box = f.GetValue(o);
                    writer.WriteNumeric(F, box);
                }

                bool[] boolValues = boolFields.Select(f => (bool)f.GetValue(o)).ToArray();
                foreach (byte b in ConvertBoolArrayToByteArray(boolValues))
                {
                    writer.Write(b);
                }

                writer.Close();
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static void Deserialize(string code, object o)
        {
            Type T = o.GetType();
            FieldInfo[] numericFields;
            FieldInfo[] boolFields;
            if (FieldCache.TryGetValue(T, out var pair))
            {
                numericFields = pair.numerics;
                boolFields = pair.bools;
            }
            else
            {
                FieldInfo[] fields = T.GetFields(BindingFlags.Public | BindingFlags.Instance);
                numericFields = fields.Where(f => NumericTypes.Contains(f.FieldType) || f.FieldType.IsEnum).OrderBy(f => f.Name).ToArray();
                boolFields = fields.Where(f => f.FieldType == typeof(bool)).OrderBy(f => f.Name).ToArray();
                FieldCache[T] = (numericFields, boolFields);
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(code);
            }
            catch (Exception e)
            {
                MenuChanger.instance.LogWarn($"Malformatted Base64 string {{{code}}}\n" + e);
                return;
            }


            using (MemoryStream stream = new MemoryStream(bytes))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    foreach (FieldInfo field in numericFields)
                    {
                        Type F = field.FieldType;
                        if (F.IsEnum)
                        {
                            F = Enum.GetUnderlyingType(F);
                        }
                        field.SetValue(o, reader.ReadNumeric(F));
                    }

                    bool[] boolValues = ConvertByteArrayToBoolArray(reader.ReadBytes(bytes.Length - (int)stream.Position));
                    int cap = Math.Min(boolValues.Length, boolFields.Length);
                    for (int i = 0; i < cap; i++)
                    {
                        boolFields[i].SetValue(o, boolValues[i]);
                    }
                }
                catch (Exception e)
                {
                    MenuChanger.instance.LogError($"Error in deserializing {T.Name}:\n{e}");
                }
            }
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
