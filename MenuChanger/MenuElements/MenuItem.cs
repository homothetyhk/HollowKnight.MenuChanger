using System.Collections;
using System.Reflection;
using MenuChanger.Extensions;
using UnityEngine.EventSystems;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// Base class for buttons which allow selecting an item from a fixed set of values.
    /// </summary>
    public abstract class MenuItem : SmallButton, IValueElement
    {
        public object Value { get; protected set; }
        public Type ValueType { get; }
        public IList Items { get; }
        public int Index { get; protected set; }
        public string Name { get; }

        protected readonly FixVerticalAlign _align;
        protected MenuItemFormatter _formatter;

        /// <summary>
        /// The object which formats the MenuItem's label. Setting this property triggers a text refresh. Must not be null.
        /// </summary>
        public MenuItemFormatter Formatter
        {
            get
            {
                return _formatter;
            }
            set
            {
                _formatter = value;
                RefreshText();
            }
        }

        public MenuItem(Type valueType, MenuPage page, string name, IList items, MenuItemFormatter formatter) : base(page, name) 
        {
            Name = name;
            _align = GameObject.GetComponentInChildren<FixVerticalAlign>(true);
            _formatter = formatter;
            ValueType = valueType;
            Items = items;

            Button.ClearEvents();
            Button.AddEvent(EventTriggerType.Submit, OnMenuItemClick);
        }

        protected virtual void OnMenuItemClick(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointer && pointer.button != PointerEventData.InputButton.Left)
            {
                MovePrevious();
            }
            else
            {
                MoveNext();
            }
            base.InvokeOnClick();
        }

        public void AddItem(object o)
        {
            if (!IsTypeValid(o)) return;
            Items.Add(o);
        }

        public void InsertItem(int index, object o)
        {
            if (!IsTypeValid(o)) return;
            if (index <= Index) Index++;
            Items.Insert(index, o);
        }
        public bool RemoveItem(object o)
        {
            int i = Items.IndexOf(o);
            if (i < 0) return false;
            Items.RemoveAt(i);
            if (Index == i) MoveNext();
            return true;
        }
        public void OverwriteCurrent(object o)
        {
            if (!IsTypeValid(o)) return;
            Items[Index] = o;
            RefreshText();
        }

        public void RemoveCurrent()
        {
            if (Items.Count > 0)
            {
                Items.RemoveAt(Index--);
                MoveNext();
            }
        }

        public void Bind(object obj, MemberInfo mi)
        {
            SetValue(mi.GetValue(obj));
            SelfChanged += b => mi.SetValue(obj, b.Value);
        }

        public void MoveNext()
        {
            if (Locked || Items.Count == 0) return;

            int i = Index + 1;
            if (i >= Items.Count || i < 0) i = 0;
            SetValueInternal(Items[i]);
        }

        public void MovePrevious()
        {
            if (Locked || Items.Count == 0) return;

            int i = Index - 1;
            if (i >= Items.Count || i < 0) i = Items.Count - 1;
            SetValueInternal(Items[i]);
        }

        public void SetValue(object o)
        {
            if (Locked || !IsTypeValid(o)) return;
            SetValueInternal(o);
        }

        protected virtual void SetValueInternal(object o, bool invokeEvent = true)
        {
            bool cancelChange = false;
            InvokeInterceptChange(ref o, ref cancelChange);
            if (cancelChange) return;
            int i = Items.IndexOf(o);

            if (0 <= i && i < Items.Count)
            {
                Index = i;
                Value = Items[i];
            }
            else
            {
                Index = -1;
                Value = o;
            }

            RefreshText();
            if (invokeEvent)
            {
                InvokeChanged();
            }
        }

        protected virtual void RefreshText()
        {
            Text.text = Formatter.GetText(Name, Value);
            _align.AlignText();
        }

        protected virtual bool IsTypeValid(object value)
        {
            return ValueType.IsInstanceOfType(value) || (!ValueType.IsValueType && value == null);
        }

        public delegate void InterceptChangeHandler(MenuItem self, ref object newValue, ref bool cancelChange);
        /// <summary>
        /// Event called before the MenuItem's value is updated.
        /// </summary>
        public event InterceptChangeHandler InterceptChanged;
        protected virtual void InvokeInterceptChange(ref object newValue, ref bool cancelChange)
        {
            try
            {
                InterceptChanged?.Invoke(this, ref newValue, ref cancelChange);
            }
            catch { }
        }

        public event Action<IValueElement> SelfChanged;
        protected virtual void InvokeChanged()
        {
            try
            {
                SelfChanged?.Invoke(this);
            }
            catch { }
        }
    }


    /// <summary>
    /// Button which allows selecting an item from a fixed set of values of the generic type.
    /// </summary>
    public class MenuItem<T> : MenuItem
    {
        new public T Value 
        {
            get => (T)base.Value;
            protected set => SetValue(value);
        }

        new protected readonly List<T> Items;

        /// <summary>
        /// Creates a MenuItem with given array of values. The current value will be displayed with the button's name as a prefix.
        /// </summary>
        public MenuItem(MenuPage page, string name, params T[] values) : this(page, name, values?.ToList() ?? new List<T>()) { }

        /// <summary>
        /// Creates a MenuItem with given list of values. The current value will be displayed with the button's name as a prefix.
        /// </summary>
        public MenuItem(MenuPage page, string name, List<T> values) : this(page, name, values, new DefaultMenuItemFormatter()) { }

        public MenuItem(MenuPage page, string name, List<T> values, MenuItemFormatter formatter) : base(typeof(T), page, name, values, formatter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Items = values;

            Index = 0;
            if (Items.Count > 0) Value = Items[0];
            else Value = default;

            RefreshText();
        }

        public void SetValue(T obj)
        {
            if (Locked) return;
            SetValueInternal(obj);
        }

        public void AddItem(T t)
        {
            Items.Add(t);
        }

        public void RemoveItem(T t)
        {
            Items.Remove(t);
            if (Value.Equals(t)) MoveNext();
        }

        public void OverwriteCurrent(T t)
        {
            if (Items.Count > 0)
            {
                Value = Items[Index] = t;
                RefreshText();
            }
        }

        public event Action<T> ValueChanged;
        protected override void InvokeChanged()
        {
            base.InvokeChanged();
            try
            {
                ValueChanged?.Invoke(Value);
            }
            catch { }
        }
    }

    /// <summary>
    /// A MenuItem of the Enum type, which has the Enum's values as its items.
    /// </summary>
    public class MenuEnum<T> : MenuItem<T> where T : struct, Enum, IConvertible
    {
        public MenuEnum(MenuPage page, string name) : base(page, name, ((T[])Enum.GetValues(typeof(T))).ToList(), new MenuItemEnumFormatter()) { }
    }

    /// <summary>
    /// Class used to control the text displayed by a MenuItem.
    /// </summary>
    public abstract class MenuItemFormatter
    {
        /// <summary>
        /// Returns the text which should be displayed by the MenuItem parent.
        /// </summary>
        public abstract string GetText(string prefix, object value);
    }

    /// <summary>
    /// Class used to control the text displayed by a MenuItem. Formats text as "{name}: {item}"
    /// </summary>
    public class DefaultMenuItemFormatter : MenuItemFormatter
    {
        public override string GetText(string prefix, object value)
        {
            return $"{prefix}: {value}";
        }
    }

    /// <summary>
    /// Class used to control the text displayed by a ToggleButton. Formats text as "{name}", and does not use the value of the button.
    /// </summary>
    public class ToggleButtonFormatter : MenuItemFormatter
    {
        public override string GetText(string prefix, object value)
        {
            return prefix;
        }
    }

    /// <summary>
    /// Class used to control the text displayed by a MenuEnum. Formats text as "{name}: {display(item)}" where display converts the enum value to readable text by assuming a camel-case value.
    /// <br/>Supports MenuLabelAttribute on enum members.
    /// </summary>
    public class MenuItemEnumFormatter : MenuItemFormatter
    {
        private static readonly Dictionary<string, string> cachedNames = new();

        public override string GetText(string prefix, object value)
        {
            return $"{prefix}: {GetEnumName(value)}";
        }

        public string GetEnumName(object value)
        {
            string repr = value.ToString();
            if (cachedNames.TryGetValue(repr, out string name)) return name;
            else if (value.GetType().GetField(repr) is FieldInfo fi)
            {
                return cachedNames[repr] = fi.GetMenuName();
            }
            else return cachedNames[repr] = repr.FromCamelCase();
        }

    }
}
