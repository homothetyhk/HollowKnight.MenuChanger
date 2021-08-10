using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger.MenuElements
{
    public interface ILockable
    {
        bool Locked { get; }
        void Lock();
        void Unlock();
    }
}
