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
        static Dictionary<Type, ReflectionData> Cache = new Dictionary<Type, ReflectionData>();
        class ReflectionData
        {
            public ReflectionData(Type T)
            {
                FieldInfo[] fields = T.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var g = fields
                    .GroupBy(f => f.FieldType)
                    .ToDictionary(i => i.Key, i => i.OrderBy(f => f.Name).ToArray());


                if (!g.TryGetValue(typeof(long), out LongFields)) LongFields = new FieldInfo[0];
                if (!g.TryGetValue(typeof(int), out IntFields)) IntFields = new FieldInfo[0];
                if (!g.TryGetValue(typeof(short), out ShortFields)) ShortFields = new FieldInfo[0];
                if (!g.TryGetValue(typeof(byte), out ByteFields)) ByteFields = new FieldInfo[0];
                if (!g.TryGetValue(typeof(bool), out BoolFields)) BoolFields = new FieldInfo[0];

                EnumFields = fields.Where(f => f.FieldType.IsEnum).OrderBy(f => f.Name).ToArray();
            }

            public FieldInfo[] LongFields;
            public FieldInfo[] IntFields;
            public FieldInfo[] ShortFields;
            public FieldInfo[] ByteFields;
            public FieldInfo[] EnumFields;
            public FieldInfo[] BoolFields;

            public U[] GetValues<U>(object o, FieldInfo[] fields) => fields.Select(f => (U)f.GetValue(o)).ToArray();
            public void SetValues<U>(object o, U[] us, FieldInfo[] fields)
            {
                int len = Math.Min(us.Length, fields.Length);
                for (int i = 0; i < len; i++) fields[i].SetValue(o, us[i]);
            }

            public long[] Longs(object o) => GetValues<long>(o, LongFields);
            public int[] Ints(object o) => GetValues<int>(o, IntFields);
            public short[] Shorts(object o) => GetValues<short>(o, ShortFields);
            public byte[] Bytes(object o) => GetValues<byte>(o, ByteFields);
            public bool[] Bools(object o) => GetValues<bool>(o, BoolFields);
            public byte[] Enums(object o) => GetValues<byte>(o, EnumFields);

        }

        [Obsolete]
        public static string OldSerialize(object o)
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
                    code += $"-{OldSerialize(p)}";
                }
                else
                {
                    MenuChanger.instance.LogWarn($"Unable to serialize null {field.Name} in {o.GetType().Name}");
                }
            }

            //MenuChanger.instance.Log($"Computed Serialization of {o.GetType().Name} as {code}");
            return code;
        }

        public static string Serialize(object o)
        {
            Type T = o.GetType();
            if (!Cache.TryGetValue(T, out ReflectionData rd))
            {
                rd = new ReflectionData(T);
                Cache[T] = rd;
            }

            return SerializeNumericData(
                rd.Longs(o),
                rd.Ints(o),
                rd.Shorts(o),
                rd.Bytes(o),
                rd.Enums(o),
                rd.Bools(o)
                );
        }


        public static string SerializeNumericData(long[] longs, int[] ints, short[] shorts, byte[] bytes, byte[] rawEnums, bool[] bools)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                foreach (long i in longs ?? new long[0]) writer.Write(i);
                foreach (int i in ints ?? new int[0]) writer.Write(i);
                foreach (short i in shorts ?? new short[0]) writer.Write(i);
                foreach (byte i in bytes ?? new byte[0]) writer.Write(i);
                foreach (byte i in rawEnums ?? new byte[0]) writer.Write(i);
                foreach (byte b in ConvertBoolArrayToByteArray(bools)) writer.Write(b);
                writer.Close();

                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static void Deserialize(string code, object o)
        {
            Type T = o.GetType();
            if (!Cache.TryGetValue(T, out ReflectionData rd))
            {
                Cache[T] = rd = new ReflectionData(T);
            }

            byte[] bytes = Convert.FromBase64String(code);
            using (MemoryStream stream = new MemoryStream(bytes))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    foreach (FieldInfo field in rd.LongFields) field.SetValue(o, reader.ReadInt64());
                    foreach (FieldInfo field in rd.IntFields) field.SetValue(o, reader.ReadInt32());
                    foreach (FieldInfo field in rd.ShortFields) field.SetValue(o, reader.ReadInt16());
                    foreach (FieldInfo field in rd.ByteFields) field.SetValue(o, reader.ReadByte());
                    foreach (FieldInfo field in rd.EnumFields) field.SetValue(o, reader.ReadByte());
                    bool[] bools = ConvertByteArrayToBoolArray(reader.ReadBytes(bytes.Length - (int)stream.Position));
                    for (int i = 0; i < bools.Length; i++)
                    {
                        rd.BoolFields[i].SetValue(o, bools[i]);
                    }
                }
                catch (Exception e)
                {
                    MenuChanger.instance.LogError($"Error in deserializing {T.Name}:\n{e}");
                }
            }
        }

        [Obsolete]
        public static object OldDeserialize(string code, object o)
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
