namespace JR.Stand.Core.Utils
{
    public static class TypesConv
    {
        /// <summary>
        /// 安装的转换Int
        /// </summary>
        /// <param name="v">值</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static int SafeParseInt(object v, int def)
        {
            if (v == null) return def;
            try
            {
                return (int) v;
            }
            catch
            {
                return def;
            }
        }
    }
}