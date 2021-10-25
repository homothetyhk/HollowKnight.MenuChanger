using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger.Extensions
{
    internal static class StringExtensions
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
                if (char.IsUpper(uiname[i]))
                {
                    if (char.IsLower(uiname[i - 1]))
                    {
                        uiname.Insert(i++, " ");
                    }
                    else if (i + 1 < uiname.Length && char.IsLower(uiname[i + 1]))
                    {
                        uiname.Insert(i++, " ");
                    }
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
