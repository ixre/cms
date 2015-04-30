

namespace OPSite.WebHandler
{
    using System;
    using Ops.Cms;
    using Ops.Web;


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]

    public class PermissionAttribute :global::Ops.Cms.PermissionAttribute, IWebExecute
    {

        private global::Ops.Cms.PermissionAttribute per;
        public PermissionAttribute()
        {
            per = new PermissionAttribute();
        }
        public PermissionAttribute(string path)
        {
            per = new PermissionAttribute(path);
        }
        public void PreExecuting()
        {
            base.Validate(UserState.Current);
        }
    }
}