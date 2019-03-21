using System;
using OPS.Web;

namespace OPSoft.WebControlCenter.Handler
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class Valid:Attribute,IWebExecute
    {
        public void PreExecuting()
        {
        }
    }
}