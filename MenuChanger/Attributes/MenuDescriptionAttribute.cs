using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuChanger.Attributes
{
    public class MenuDescriptionAttribute : Attribute
    {
        public readonly string text;
        public MenuDescriptionAttribute(string text)
        {
            this.text = text;
        }
    }
}
