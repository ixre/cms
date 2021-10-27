using System.Collections.Generic;

namespace JR.DevFw.Template.Compiler.Text
{
    internal class ReferenceInCode
    {
        private static readonly string[] _SystemSpecRefs =
        {
            "System.Configuration",
            "System.Configuration.Install",
            "System.Data",
            "System.Data.SqlClient",
            "System.Data.SqlXml",
            "System.Deployment",
            "System.Design",
            "Sysemt.DirecoryServices",
            "System.DirectoryServices.Protocols",
            "System.Drawing",
            "System.Drawing.Design",
            "System.EnterpriseServices",
            "System.Management",
            "System.Messaging",
            "System.Runtime.Remoting",
            "System.Runtime.Serialization.Formatters.Soap",
            "System.Security",
            "System.ServiceProcess",
            "System.Transactions",
            "System.Web",
            "System.Web.Mobile",
            "System.Web.RegularExpressions",
            "System.Web.Services",
            "System.Windows.Forms",
            "System.Xml"
        };

        private static string GetSystemReferenceDllName(string namespaceText)
        {
            foreach (string sysSpecRef in _SystemSpecRefs)
            {
                if (namespaceText.IndexOf(sysSpecRef) == 0)
                {
                    return sysSpecRef + ".dll";
                }
            }

            return "System.dll";
        }

        public static List<string> GetNameSpacesInSourceCode(string code)
        {
            return Regx.GetMatchStrings(code, @"using\s+(.+?)\s*;", false);
        }

        public static string GetDefaultReferenceDllName(string namespaceText)
        {
            namespaceText = namespaceText.Trim();

            if (namespaceText.IndexOf("System") == 0)
            {
                return GetSystemReferenceDllName(namespaceText);
            }
            else
            {
                return namespaceText + ".dll";
            }
        }
    }
}