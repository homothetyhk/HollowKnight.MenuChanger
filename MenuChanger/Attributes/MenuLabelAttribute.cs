using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which contains the appropriate name of a member for menu display.
    /// </summary>
    public class MenuLabelAttribute : Attribute
    {
        public readonly string text;

        public MenuLabelAttribute(string text)
        {
            this.text = text;
        }

    }
}
