using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace JR.Cms.Domain.Interface.User
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
           if (this._dict.ContainsKey(appId))
           {
               return this._dict[appId];
           }
           return null;
       }

       public ICollection<int> GetSiteIds()
       {
           return this._dict.Keys;
       }
   }
}
