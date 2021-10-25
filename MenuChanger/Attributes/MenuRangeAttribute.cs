using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    public class MenuRangeAttribute : Attribute
    {
        public readonly object min;
        public readonly object max;

        public MenuRangeAttribute(object min, object max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
