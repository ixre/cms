

namespace OPSite.WebHandler
{
    using System;
    using J6.Cms;
    using J6.Web;


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]

    public class PermissionAttribute :global::J6.Cms.PermissionAttribute, IWebExecute
    {

        private global::J6.Cms.PermissionAttribute per;
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