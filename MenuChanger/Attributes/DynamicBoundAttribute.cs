using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    public class DynamicBoundAttribute : Attribute
    {
        public readonly string memberName;
        public readonly bool upper;

        public DynamicBoundAttribute(string memberName, bool upper)
        {
            this.memberName = memberName;
            this.upper = upper;
        }
    }
}
