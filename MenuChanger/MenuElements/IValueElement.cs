using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// A MenuElement which represents a value that can be modified and bound to the field or property of an object.
    /// </summary>
    public interface IValueElement : IMenuElement, ISelectable
    {
        /// <summary>
        /// The boxed value of the element.
        /// </summary>
        object Value { get; }
        /// <summary>
        /// The type of Value.
        /// </summary>
        Type ValueType { get; }
        /// <summary>
        /// Sets Value. The result may be modified or interrupted by validation and events.
        /// </summary>
        void SetValue(object o);
        /// <summary>
        /// Changes to Value will subsequently set the value of the field or property MemberInfo.
        /// </summary>
        void Bind(object o, MemberInfo mi);
        /// <summary>
        /// An event invoked when Value is changed, passing in the current instance as the parameter.
        /// </summary>
        event Action<IValueElement> SelfChanged;
    }

    /// <summary>
    /// A strongly typed IValueElement. A MenuElement which represents a value that can be modified and bound to the field or property of an object.
    /// </summary>
    public interface IValueElement<T> : IValueElement
    {
        /// <summary>
        /// The value of the element.
        /// </summary>
        new T Value { get; }
        /// <summary>
        /// Sets Value. The result may be modified or interrupted by validation and events.
        /// </summary>
        void SetValue(T t);
        /// <summary>
        /// An event invoked when Value is changed, passing in the new value as the parameter.
        /// </summary>
        event Action<T> ValueChanged;
    }
}
