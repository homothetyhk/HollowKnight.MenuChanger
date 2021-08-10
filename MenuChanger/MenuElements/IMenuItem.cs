using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MenuChanger.MenuElements
{
    public interface IMenuItem : IMenuElement, ILockable
    {
        object BoxedCurrentSelection { get; }

        bool TrySetSelection(object obj);
        void MoveNext();
        void Bind(object obj, FieldInfo field);

    }
}
