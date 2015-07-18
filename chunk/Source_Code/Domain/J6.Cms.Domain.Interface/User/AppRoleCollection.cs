using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace J6.Cms.Domain.Interface.User
{
   public class AppRoleCollection:IDomain<int>
   {
       private IDictionary<int, AppRolePair> _dict;

       public AppRoleCollection(int userId,IDictionary<int,AppRolePair> dict )
       {
           this.Id = userId;
           this._dict = dict;
       }
       public int Id { get; set; }

       public AppRolePair GetRole(int appId)
       {
           return this._dict[appId];
       }

       public ICollection<int> GetSiteIds()
       {
           return this._dict.Keys;
       }
   }
}
