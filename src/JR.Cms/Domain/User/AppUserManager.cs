using System.Collections.Generic;
using JR.Cms.Domain.Interface.User;

namespace JR.Cms.Domain.User
{
    internal class AppUserManager : IAppUserManager
    {
        private readonly IUserRepository rep;
        private readonly int appId;

        public AppUserManager(IUserRepository rep, int appId)
        {
            // TODO: Complete member initialization
            this.rep = rep;
            this.appId = appId;
        }

        public IList<RoleValue> GetAppRoles()
        {
            var list = rep.GetAppRoles(appId);
            //list.Add(Role.Master);
            list.Add(Role.SiteOwner);
            list.Add(Role.Publisher);
            return list;
        }
    }
}