using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// Interface for interacting with MenuItems of unknown type.
    /// </summary>
    public interface IMenuItem : IMenuElement, ILockable
    {
        /// <summary>
        /// The current selection of the MenuItem.
        /// </summary>
        object CurrentSelection { get; }
        /// <summary>
        /// Returns true if the MenuItem's selection was successfully set to the object.
        /// </summary>
        bool TrySetSelection(object obj);
        /// <summary>
        /// Advances the MenuItem to its next item.
        /// </summary>
        void MoveNext();
        /// <summary>
        /// Sets the MenuItem to update the object's member with the new value when its selection changes.
        /// </summary>
        void Bind(object obj, MemberInfo mi);
    }
}
