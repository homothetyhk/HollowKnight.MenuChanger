using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    /// <summary>
    /// Attribute which contains the description of a member. Not yet supported by MenuChanger.
    /// </summary>
    public class MenuDescriptionAttribute : Attribute
    {
        public readonly string text;
        public MenuDescriptionAttribute(string text)
        {
            this.text = text;
        }
    }
}
