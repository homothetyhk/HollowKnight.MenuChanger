using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger
{
    public class InterceptEventArgs<T> : EventArgs
    {
        public readonly T orig;
        public T current;
        public bool cancelChange;

        public InterceptEventArgs(T orig)
        {
            this.orig = this.current = orig;
        }

        public InterceptEventArgs(T orig, T current)
        {
            this.orig = orig;
            this.current = current;
        }

    }
}
