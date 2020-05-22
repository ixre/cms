namespace JR.Stand.Core.Template
{
    public static class TemplateUtils
    {
        public static string GetFunctionParamValue(string value)
        {
            var len = value.Length;
            if (len == 0) return value;
            if (value[0] == '\'')
            {
                if(len == 1 || value[len-1]!='\'')throw new TemplateException("参数末尾应包含\"'\"");
                return value.Substring(1, len - 2);
            }
            if (value[0] == '\"')
            {
                if(len == 1 || value[len-1]!='\"')throw new TemplateException("参数末尾应包含\"");
                return value.Substring(1, len - 2);
            } 
            if (value[0] == '{')
            {
                if(len == 1 || value[len-1]!='}')throw new TemplateException("参数末尾应包含\"}\"");
                return value.Substring(1, len - 2);
            }
            return value;
        }
    }
}