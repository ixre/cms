namespace Ops.Cms.DAL
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class SQLRegex
    {
        private static Regex varRegex=new Regex("\\$\\[([A-Za-z_]+)\\]");
        public static string Replace(string sql, MatchEvaluator eval)
        {
            return varRegex.Replace(sql, eval);
        }
    }
}