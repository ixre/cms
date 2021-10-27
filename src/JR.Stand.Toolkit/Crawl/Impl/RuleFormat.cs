namespace JR.Stand.Toolkit.Crawl.Impl
{
    internal class RuleFormat
    {
        public static string Format(string rule)
        {
            return rule.Replace("$$", "\\s*([\\s\\S]+?)\\s*");
        }
    }
}