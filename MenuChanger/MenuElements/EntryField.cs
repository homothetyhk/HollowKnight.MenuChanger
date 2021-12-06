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
    /// <summary>
    /// The base class of MenuChanger input fields.
    /// </summary>
    public abstract class EntryField : IMenuElement, ISelectable, IValueElement
    {
        public MenuPage Parent { get; }
        public GameObject GameObject { get; }
        public InputField InputField { get; }
        public bool Hidden { get; private set; }
        public MenuLabel Label { get; }
        public Vector2 LabelOffset = new(0f, 55f);

        public EntryField(Type valueType, MenuPage page, string label, MenuLabel.Style style = MenuLabel.Style.Title)
        {
            ValueType = valueType;
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
            InputField.onEndEdit.AddListener(Validate);
        }

        public string ValidatedInput { get; protected set; }
        public object Value { get; protected set; }
        public Type ValueType { get; }

        protected virtual void Validate(string input)
        {
            object value;
            try
            {
                InvokeModifyInputString(ref input, ValidatedInput);
                value = Read(input, ValueType);
            }
            catch
            {
                InputField.text = ValidatedInput;
                return;
            }
            ValidatedInput = input;
            Value = value;
            try
            {
                InvokeSelfChanged();
            }
            catch
            {
                return;
            }
        }

        public delegate void ModifyInputHandler<T>(ref T newValue, T orig);
        public event ModifyInputHandler<string> ModifyInputString;
        protected void InvokeModifyInputString(ref string newValue, string orig) => ModifyInputString?.Invoke(ref newValue, orig);

        public event Action<IValueElement> SelfChanged;
        protected void InvokeSelfChanged()
        {
            try
            {
                SelfChanged?.Invoke(this);
            }
            catch { return; }
        }

        public virtual object Read(string input, Type type)
        {
            return Convert.ChangeType(input, type);
        }
        public virtual string Write(object t)
        {
            if (t is null) return string.Empty;
            return t.ToString();
        }

        public void SetValue(object o) => Validate(Write(o));

        public virtual void Bind(object o, MemberInfo mi)
        {
            SetValue(mi.GetValue(o));
            SelfChanged += self => mi.SetValue(o, self.Value);
        }

        public void AddValidateInputToTextColorEvent(Func<string, bool> test)
        {
            SelfChanged += (self) =>
            {
                if (test(((EntryField)self).ValidatedInput)) InputField.textComponent.color = Colors.DEFAULT_COLOR;
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
            UObject.Destroy(GameObject);
        }

        public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
        {
            Navigation nv = InputField.navigation;
            switch (neighbor)
            {
                case Neighbor.Up:
                    nv.selectOnUp = selectable?.GetSelectable(Neighbor.Down);
                    break;
                case Neighbor.Down:
                    nv.selectOnDown = selectable?.GetSelectable(Neighbor.Up);
                    break;
                case Neighbor.Right:
                    nv.selectOnRight = selectable?.GetSelectable(Neighbor.Left);
                    break;
                case Neighbor.Left:
                    nv.selectOnLeft = selectable?.GetSelectable(Neighbor.Right);
                    break;
            }
            InputField.navigation = nv;
        }

        public ISelectable GetISelectable(Neighbor neighbor) => this;
        public Selectable GetSelectable(Neighbor neighbor) => InputField;
    }

    /// <summary>
    /// An EntryField of a specified type. EntryFields such as NumericEntryField and TextEntryField derive from this class.
    /// </summary>
    public class EntryField<T> : EntryField, IValueElement<T>
    {
        public EntryField(MenuPage page, string label, MenuLabel.Style style = MenuLabel.Style.Title) : base(typeof(T), page, label, style)
        {
            InputField.onEndEdit.AddListener(Validate);
            SetValue(default);
        }

        new public T Value
        {
            get => (T)base.Value;
            protected set => base.Value = value;
        }

        protected override void Validate(string input)
        {
            T value;
            try
            {
                InvokeModifyInputString(ref input, ValidatedInput);
                value = (T)Read(input, ValueType);
                Modify?.Invoke(ref value, Value);
            }
            catch
            {
                InputField.text = ValidatedInput;
                return;
            }
            Value = value;
            InputField.text = ValidatedInput = Write(value);
            try
            {
                InvokeSelfChanged();
                ValueChanged?.Invoke(value);
            }
            catch { }
        }

        public void SetValue(T t) => Validate(Write(t));

        public event ModifyInputHandler<T> Modify;

        public event Action<T> ValueChanged;

        public void AddValidateInputToTextColorEvent(Func<T, bool> test)
        {
            ValueChanged += (_) =>
            {
                if (test(Value)) InputField.textComponent.color = Colors.DEFAULT_COLOR;
                else InputField.textComponent.color = Colors.INVALID_INPUT_COLOR;
            };
        }
    }

    /// <summary>
    /// An EntryField which allows the Read and Write methods to be overriden by delegate fields.
    /// </summary>
    public class CustomEntryField<T> : EntryField<T>
    {
        public Func<string, T> read;
        public Func<T, string> write;

        public CustomEntryField(MenuPage page, string label, Func<string, T> read, Func<T, string> write) : base(page, label)
        {
            this.read = read;
            this.write = write;
        }

        public override object Read(string input, Type type) => read != null ? read.Invoke(input) : Read(input, ValueType);
        public override string Write(object o) => write != null ? write.Invoke((T)o) : base.Write(o);
    }

    /// <summary>
    /// An EntryField for multiline text.
    /// </summary>
    public class TextEntryField : EntryField<string>
    {
        public TextEntryField(MenuPage page, string label) : base(page, label, MenuLabel.Style.Body)
        {
            InputField.textComponent.alignment = TextAnchor.UpperCenter;
            LabelOffset = new Vector2(0, 30f);
            MoveTo(Vector2.zero);
        }
    }

    /// <summary>
    /// An EntryField for numeric types, which supports operations that clamp the input to a given range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
            InputField.characterValidation = InputField.CharacterValidation.Decimal;
            InputField.characterLimit = Math.Max(typeMin.ToString().Length, typeMax.ToString().Length);
            SetClamp(typeMin, typeMax);
            Modify += InputClamp;
        }

        public override void Bind(object o, MemberInfo mi)
        {
            base.Bind(o, mi);

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
                        Modify += (ref T val, T orig) =>
                        {
                            T ub = (T)info.GetValue(o);
                            val = val.CompareTo(ub) <= 0 ? val : ub;
                        };
                    }
                    else
                    {
                        Modify += (ref T val, T orig) =>
                        {
                            T lb = (T)info.GetValue(o);
                            val = val.CompareTo(lb) >= 0 ? val : lb;
                        };
                    }
                }
                else if (info is MethodInfo method)
                {
                    if (db.upper)
                    {
                        Modify += (ref T val, T orig) =>
                        {
                            T ub = (T)method.Invoke(o, Array.Empty<object>());
                            val = val.CompareTo(ub) <= 0 ? val : ub;
                        };
                    }
                    else
                    {
                        Modify += (ref T val, T orig) =>
                        {
                            T lb = (T)method.Invoke(o, Array.Empty<object>());
                            val = val.CompareTo(lb) >= 0 ? val : lb;
                        };
                    }
                }
            }
        }

        private T clampMin;
        private T clampMax;

        private void InputClamp(ref T input, T orig)
        {
            input = Clamp(input, clampMin, clampMax);
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
    }
}
