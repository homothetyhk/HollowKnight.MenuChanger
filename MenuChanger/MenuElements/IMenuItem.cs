using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MenuChanger.MenuElements
{
    public interface IMenuItem : IMenuElement
    {
        object BoxedCurrentSelection { get; }
        bool Locked { get; }

        bool TrySetSelection(object obj, bool invokeChanged);
        void MoveNext();
        void Bind(object obj, FieldInfo field);
        void Lock();
        void Unlock();
    }
}
