using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class StringExtentions
    {
        public static string GetFormated(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
