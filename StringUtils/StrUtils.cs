using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatternToolbox
{
    public static class StrUtils
    {
        static public string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static public string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
                });
        }

        public static string Sanitize(string phrase)
        {
            string removeChars = "*&@()<>’\'\"\t\r";
            string result = phrase;

            foreach (char c in removeChars)
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            result = result.Replace('\n', ' ');

            return result;
        }

        public static string ExtractAndConcatonate(ref string inputstr, char leadingSym = '[', char closingSym = ']')
        {
            string betweens = String.Empty;

            bool go = true;
            while (go)
            {
                int open = inputstr.IndexOf(leadingSym);
                if (open == -1)
                    return betweens;

                int close = inputstr.IndexOf(closingSym, open);

                if (close == -1)
                    return betweens;

                //get it
                string between = inputstr.Substring(open + 1, close - (open + 1));

                //concat
                if (betweens == String.Empty)
                    betweens += between;
                else
                    betweens += ", " + between;

                //remove it
                string temp = inputstr.Substring(0, open);
                string temp2 = inputstr.Substring(close + 1);
                inputstr = temp + temp2;
            }
            return betweens;
        }

        public static string CleanUpNewlines(string str)
        {
            str = str.Replace("\r\n", "\n"); //windows line endings
            str = str.Replace("\n\n", "\n"); //double newlines
            str = str.Replace("\n\n", "\n"); //double newlines again
            return str;
        }


        //trims empty space, and unescaped 
        public static string Unescape(string str)
        {
            while(str.Contains(@"\n"))
                str = str.Replace(@"\n", "\n");

            return str;
        }
    }
}
