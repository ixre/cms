using System.Collections.Generic;

namespace JR.Cms.Domain.Interface.User
{
    public class AppRoleCollection : IDomain<int>
    {
        private IDictionary<int, AppRolePair> _dict;

        public AppRoleCollection(int userId, IDictionary<int, AppRolePair> dict)
        {
            Id = userId;
            _dict = dict;
        }

        public int Id { get; set; }

        public int GetDomainId()
        {
            return Id;
        }

        public AppRolePair GetRole(int appId)
        {
            if (_dict.ContainsKey(appId)) return _dict[appId];
            return null;
        }

        public ICollection<int> GetSiteIds()
        {
            return _dict.Keys;
        }
    }
}