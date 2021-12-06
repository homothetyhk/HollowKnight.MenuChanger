using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which indicates that changes to this member should cause validation of another member's value.
    /// </summary>
    public class TriggerValidationAttribute : Attribute
    {
        /// <summary>
        /// The member which should be validated when this member is changed.
        /// </summary>
        public string memberName;
        /// <summary>
        /// Indicates that changes to this member should cause validation of another member's value.
        /// </summary>
        public TriggerValidationAttribute(string memberName) => this.memberName = memberName;
    }
}
