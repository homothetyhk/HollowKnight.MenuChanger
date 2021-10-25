using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using MenuChanger.Attributes;
using MenuChanger.Extensions;

namespace MenuChanger.MenuElements
{
    public abstract class EntryField<T> : IMenuElement
    {
        public MenuPage Parent { get; }
        public GameObject GameObject { get; }
        public InputField InputField { get; }
        public bool Hidden { get; private set; }
        public MenuLabel Label { get; }
        public Vector2 LabelOffset = new Vector2(0f, 55f);

        public EntryField(MenuPage page, string label, MenuLabel.Style style = MenuLabel.Style.Title)
        {
            switch (style)
            {
                default:
                case MenuLabel.Style.Title:
                    (GameObject, InputField) = PrefabMenuObjects.BuildEntryField();
                    page.Add(GameObject); // whyyy?
                    GameObject.transform.localScale = new Vector2(0.8f, 0.8f);
                    break;
                case MenuLabel.Style.Body:
                    (GameObject, InputField) = PrefabMenuObjects.BuildMultiLineEntryField(page);
                    break;
            }
            
            Parent = page;

            Label = new MenuLabel(page, label, MenuLabel.Style.Body);
            Label.Text.alignment = TextAnchor.UpperCenter;
            MoveTo(Vector2.zero);

            InputField.onValueChanged.AddListener(InvokeModify);
            InputField.onValueChanged.AddListener(InvokeChanged);
        }

        public T InputValue
        {
            get => Read(InputField.text);
            set
            {
                InputField.text = Write(value);
                Changed?.Invoke(InputValue);
            }
        }

        public delegate void ChangedHandler(T self);
        public event ChangedHandler Changed;
        public void InvokeChanged(string _ = null) => Changed?.Invoke(InputValue);

        public delegate T ModifyHandler(T input);
        public event ModifyHandler Modify;
        public void InvokeModify(string _ = null)
        {
            T input = InputValue;
            foreach (Delegate d in Modify?.GetInvocationList() ?? Enumerable.Empty<Delegate>())
            {
                input = (T)d.DynamicInvoke(input);
            }
            if (!InputValue.Equals(input)) InputValue = input;
        }


        public abstract T Read(string input);
        public virtual string Write(T t)
        {
            return t.ToString();
        }


        public virtual void Bind(MemberInfo mi, object o)
        {
            InputValue = (T)mi.GetValue(o);
            Changed += value => mi.SetValue(o, value);
        }

        public void AddValidateInputToTextColorEvent(Func<T, bool> test)
        {
            Changed += (_) =>
            {
                if (test(InputValue)) InputField.textComponent.color = Colors.DEFAULT_COLOR;
                else InputField.textComponent.color = Colors.INVALID_INPUT_COLOR;
            };
        }

        public void MoveTo(Vector2 pos)
        {
            GameObject.transform.localPosition = pos;
            Label.MoveTo(pos + LabelOffset);
        }

        public void Translate(Vector2 delta)
        {
            GameObject.transform.localPosition += (Vector3)delta;
            Label.Translate(delta);
        }

        public void Show()
        {
            Hidden = false;
            GameObject.SetActive(true);
            Label.Show();
        }

        public void Hide()
        {
            Hidden = true;
            GameObject.SetActive(false);
            Label.Hide();
        }

        public void Destroy()
        {
            GameObject.Destroy(GameObject);
        }
    }


    public class CustomEntryField<T> : EntryField<T>
    {
        public Func<string, T> read;
        public Func<T, string> write;

        public CustomEntryField(MenuPage page, string label, Func<string, T> read, Func<T, string> write) : base(page, label)
        {
            this.read = read;
            this.write = write;
        }

        public override T Read(string input) => read(input);
        public override string Write(T t) => write(t);
    }


    public class TextEntryField : EntryField<string>
    {
        public TextEntryField(MenuPage page, string label) : base(page, label, MenuLabel.Style.Body)
        {
            InputField.textComponent.alignment = TextAnchor.UpperCenter;
            LabelOffset = new Vector2(0, 30f);
            MoveTo(Vector2.zero);
        }

        public override string Read(string s)
        {
            return s;
        }
    }

    public class NumericEntryField<T> : EntryField<T> 
        where T : struct,
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
    {
        public NumericEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterValidation = InputField.CharacterValidation.Integer;
            SetClamp(typeMin, typeMax);
            Modify += InputClamp;
            Changed += CleanInput;
            InputField.onValidateInput += CatchNegation;
        }

        public override T Read(string input)
        {
            if (long.TryParse(input, out long val))
            {
                return (T)Convert.ChangeType(ClampToType(val), typeof(T));
            }
            else if (ulong.TryParse(input, out ulong altVal))
            {
                return (T)Convert.ChangeType(ClampToType(altVal), typeof(T));
            }
            else return default;
        }

        public override void Bind(MemberInfo mi, object o)
        {
            base.Bind(mi, o);

            if (mi.GetCustomAttribute<MenuRangeAttribute>() is MenuRangeAttribute mr)
            {
                SetClamp((T)mr.min, (T)mr.max);
            }

            Type U = o.GetType();
            foreach (var db in mi.GetCustomAttributes<DynamicBoundAttribute>())
            {
                MemberInfo info = U.GetMember(db.memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m =>
                    m.MemberType == MemberTypes.Field
                    || m.MemberType == MemberTypes.Property && ((PropertyInfo)m).CanRead
                    || m.MemberType == MemberTypes.Method && ((MethodInfo)m).GetParameters().Length == 0
                    );

                if (info != null && (info.MemberType == MemberTypes.Field || info.MemberType == MemberTypes.Property))
                {
                    if (db.upper)
                    {
                        Modify += (val) =>
                        {
                            T ub = (T)info.GetValue(o);
                            return val.CompareTo(ub) <= 0 ? val : ub;
                        };
                    }
                    else
                    {
                        Modify += (val) =>
                        {
                            T lb = (T)info.GetValue(o);
                            return val.CompareTo(lb) >= 0 ? val : lb;
                        };
                    }
                }
                else if (info is MethodInfo method)
                {
                    if (db.upper)
                    {
                        Modify += (val) =>
                        {
                            T ub = (T)method.Invoke(o, Array.Empty<object>());
                            return val.CompareTo(ub) <= 0 ? val : ub;
                        };
                    }
                    else
                    {
                        Modify += (val) =>
                        {
                            T lb = (T)method.Invoke(o, Array.Empty<object>());
                            return val.CompareTo(lb) >= 0 ? val : lb;
                        };
                    }
                }
            }
        }

        private void CleanInput(T input)
        {
            switch (InputField.text)
            {
                case "":
                case "-":
                    return;
                default:
                    if (InputField.text != input.ToString())
                    {
                        InputValue = input;
                    }
                    return;
            }
        }

        private T clampMin;
        private T clampMax;

        private T InputClamp(T input)
        {
            return Clamp(input, clampMin, clampMax);
        }

        private char CatchNegation(string input, int index, char newChar)
        {
            if (newChar == '-' && clampMin.CompareTo(default(T)) >= 0) return '\0';
            else return newChar;
        }

        public void SetClamp(T min, T max)
        {
            clampMin = min;
            clampMax = max;
        }

        public static T Clamp(T val, T min, T max)
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }


        readonly static T typeMin = (T)typeof(T).GetField("MinValue", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        readonly static T typeMax = (T)typeof(T).GetField("MaxValue", BindingFlags.Public | BindingFlags.Static).GetValue(null);

        private static long ClampToType(long val)
        {
            return NumericEntryField<long>.Clamp(val, Convert.ToInt64(typeMin), Convert.ToInt64(typeMax));
        }

        private static ulong ClampToType(ulong val)
        {
            return NumericEntryField<ulong>.Clamp(val, Convert.ToUInt64(typeMin), Convert.ToUInt64(typeMax));
        }
    }

    // These types are all useless now
    public class LongEntryField : NumericEntryField<long>
    {
        public LongEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterLimit = 20;
        }
    }

    public class IntEntryField : NumericEntryField<int>
    {
        public IntEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterLimit = 12;
        }
    }

    public class ShortEntryField : NumericEntryField<short>
    {
        public ShortEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterLimit = 7;
        }
    }

    public class ByteEntryField : NumericEntryField<byte>
    {
        public ByteEntryField(MenuPage page, string label) : base(page, label)
        {
            InputField.characterLimit = 4;
        }
    }


}
