using System.Text;

namespace JR.Stand.Core.Framework.Xml.AutoObject
{
    public static class XmlObjectDoc
    {
        public const string DocStyleSheet = @"<style type=""text/css"">
                .ui-xmldoc{margin:20px;font-size:14px; }
                .ui-xmdoc h2{font-size:18px;border-bottom:solid 1px #d0d0d0;padding-bottom:10px;}
                .ui-xmldoc table{background:#d0d0d0;width:100%;line-height:23px;font-size:12px;}
                .ui-xmldoc table th{background:#e0e0e0;padding:5px 10px;text-align:left;}
                .ui-xmldoc table td{background:#f5f5f5;padding:0 10px;border:solid 1px #fff;border-top:none;}
                .ui-xmldoc table td.key{width:100px;}
                .ui-xmldoc table td.name{width:200px;}
                </style>";

        public static string GetGrid(XmlObject obj, int index)
        {
            const string tpl = @"<div class=""ui-xmldoc"" id=""object_%object.key%"">
                                    <h2><a href=""#%object.key%"">%object.index%%object.name%<span class=""key"">(%object.key%)</span></a></h2>
                                    <p class=""describe"">描述：%object.descript%</p>
                                    <p class=""prop"">
                                        <table cellspacing=""1"" cellpadding=""0"">
                                            <tr><th class=""key"">属性键:</th><th class=""name"">属性名称：</th><th class=""describe"">备注:</th></tr>
                                            %object.prop%
                                        </table>
                                    </p>
                               </div>";


            StringBuilder sb = new StringBuilder();

            if (obj.Properties != null)
            {
                foreach (XmlObjectProperty p in obj.Properties)
                {
                    sb.Append("<tr><td class=\"key\">").Append(p.Key)
                        .Append("</td><td class=\"name\">").Append(p.Name).Append("</td><td class=\"descript\">")
                        .Append(p.Descript).Append("&nbsp;</td></tr>");
                }
            }

            return tpl.Replace("%object.key%", obj.Key)
                .Replace("%object.index%", index < 1 ? "" : index.ToString())
                .Replace("%object.name%", obj.Name)
                .Replace("%object.descript%", obj.Descript.Replace("\n", "<br />"))
                .Replace("%object.prop%", sb.ToString());
        }
    }
}