using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger
{
    public static class StringExtensions
    {
        public static string FromCamelCase(this string str)
        {
            StringBuilder uiname = new StringBuilder(str);
            if (str.Length > 0)
            {
                uiname[0] = char.ToUpper(uiname[0]);
            }

            for (int i = 1; i < uiname.Length; i++)
            {
                if (char.IsUpper(uiname[i]) && char.IsLower(uiname[i - 1]))
                {
                    uiname.Insert(i, " ");
                }

                if (char.IsDigit(uiname[i]) && !char.IsDigit(uiname[i - 1]) && !char.IsWhiteSpace(uiname[i - 1]))
                {
                    uiname.Insert(i, " ");
                }
            }

            return uiname.ToString();
        }

    }
}
