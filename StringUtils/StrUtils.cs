using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatternToolbox
{
    public static class StrUtils
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ReplaceFirst(this string text, string search, string replace, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            int pos = text.IndexOf(search, comp);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

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

        public static string TrimAfter(string str, string trimAfter, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            int ind = str.LastIndexOf(trimAfter);
            if(ind != -1)
            {
                str = str.Substring(0, ind + trimAfter.Length);
            }

            return str;
        }

        public static string TrimBefore(string str, string trimAfter, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            int ind = str.IndexOf(trimAfter);
            if (ind != -1 && ind != 0)
            {
                str = str.Substring(ind);
            }

            return str;
        }
    }
}
