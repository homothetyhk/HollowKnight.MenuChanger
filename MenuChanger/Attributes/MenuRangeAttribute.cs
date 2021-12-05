using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which indicates that a member should be constrained to a fixed range. Use with members bound to a numeric entry field.
    /// </summary>
    public class MenuRangeAttribute : Attribute
    {
        public readonly object min;
        public readonly object max;

        /// <summary>
        /// Indicates that the member should be contrained between min and max.
        /// </summary>
        public MenuRangeAttribute(object min, object max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
