using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which contains the summary of a member for menu display. Not yet supported by MenuChanger.
    /// </summary>
    public class MenuSummaryAttribute : Attribute
    {
        public readonly string text;
        public MenuSummaryAttribute(string text) => this.text = text;
    }
}
