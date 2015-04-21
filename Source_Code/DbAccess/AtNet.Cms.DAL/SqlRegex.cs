using System.Text.RegularExpressions;

namespace AtNet.Cms.DAL
{
    internal static class SQLRegex
    {
        private static Regex varRegex=new Regex("\\$\\[([A-Za-z_]+)\\]");
        public static string Replace(string sql, MatchEvaluator eval)
        {
            return varRegex.Replace(sql, eval);
        }
    }
}