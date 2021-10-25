using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    public class MenuSummaryAttribute : Attribute
    {
        public readonly string text;
        public MenuSummaryAttribute(string text) => this.text = text;
    }
}
