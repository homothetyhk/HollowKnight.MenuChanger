using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    public class TriggerValidationAttribute : Attribute
    {
        public string memberName;
        public TriggerValidationAttribute(string memberName) => this.memberName = memberName;
    }
}
