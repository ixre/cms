using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JR.DevFw.Template.Compiler.Text
{
    internal class Regx
    {
        public static List<string> GetMatchStrings(String text, String regx,
            bool ignoreCase)
        {
            List<string> output = new List<string>();

            Regex reg;

            int index = 0;
            int begin = 0;
            index = regx.IndexOf("(.+)");
            if (index < 0)
            {
                index = regx.IndexOf("(.+?)");
                if (index >= 0)
                {
                    begin = index + 5;
                }
            }
            else
            {
                begin = index + 4;
            }

            if (index >= 0)
            {
                String endText = regx.Substring(begin);

                if (GetMatch(text, endText, ignoreCase) == "")
                {
                    return output;
                }
            }

            if (ignoreCase)
            {
                reg = new Regex(regx, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            else
            {
                reg = new Regex(regx, RegexOptions.Singleline);
            }

            MatchCollection m = reg.Matches(text);

            if (m.Count == 0)
            {
                return output;
            }

            for (int j = 0; j < m.Count; j++)
            {
                int count = m[j].Groups.Count;

                for (int i = 1; i < count; i++)
                {
                    output.Add(m[j].Groups[i].Value.Trim());
                }
            }

            return output;
        }

        public static String GetMatch(String text, String regx, bool ignoreCase)
        {
            Regex reg;

            int index = 0;
            int begin = 0;
            index = regx.IndexOf("(.+)");
            if (index < 0)
            {
                index = regx.IndexOf("(.+?)");
                if (index >= 0)
                {
                    begin = index + 5;
                }
            }
            else
            {
                begin = index + 4;
            }

            if (index >= 0)
            {
                String endText = regx.Substring(begin);

                if (endText != "")
                {
                    if (GetMatch(text, endText, ignoreCase) == "")
                    {
                        return "";
                    }
                }
            }

            if (ignoreCase)
            {
                reg = new Regex(regx, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            else
            {
                reg = new Regex(regx, RegexOptions.Singleline);
            }

            String ret = "";
            Match m = reg.Match(text);

            if (m.Groups.Count > 0)
            {
                ret = m.Groups[m.Groups.Count - 1].Value;
            }

            return ret;
        }
    }
}