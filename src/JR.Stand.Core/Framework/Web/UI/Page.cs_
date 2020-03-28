/*
 * Name     :   UI.Page
 * Author   :   newmin
 * Date     :   2010/10/22
 */

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using JR.DevFw.Framework.Web.unused;
using JR.DevFw.Framework.Web.unused.Interface;

namespace JR.DevFw.Framework.Web.UI
{
    public class Page : System.Web.UI.Page, ICompressionable
    {
        protected override void Render(HtmlTextWriter writer)
        {
            /* 注:base.Render(writer)应在所有假设性条件后 */
            if (ConfigurationDictionary.EnableCompression) Compression(writer); //是否启用html压缩的话
            else base.Render(writer);
        }

        #region ICompressionable 成员

        public void Compression(System.Web.UI.HtmlTextWriter writer)
        {
            StringWriter sr = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(sr);
            base.Render(tw);
            tw.Flush();
            tw.Dispose();
            StringBuilder sb = new StringBuilder(sr.ToString());
            string outhtml = Regex.Replace(sb.ToString(), "\\r+\\n+\\s+", string.Empty);
            writer.Write(outhtml);
            sr.Dispose();
        }

        #endregion
    }
}