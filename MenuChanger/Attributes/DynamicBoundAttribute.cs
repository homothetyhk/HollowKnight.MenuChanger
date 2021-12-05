using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which indicates that the member it is applied to is bounded above or below by the value of the named member.
    /// </summary>
    public class DynamicBoundAttribute : Attribute
    {
        /// <summary>
        /// The name of the field, property, or parameterless method which bounds the attribute target.
        /// </summary>
        public readonly string memberName;
        /// <summary>
        /// True if the attribute describes an upper bound, False if a lower bound.
        /// </summary>
        public readonly bool upper;

        /// <summary>
        /// Indicates that the member is bounded above or below by the value of the named member.
        /// </summary>
        public DynamicBoundAttribute(string memberName, bool upper)
        {
            this.memberName = memberName;
            this.upper = upper;
        }
    }
}
