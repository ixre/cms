using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtNet.Cms.Domain.Interface.User;

namespace AtNet.Cms.Domain.Implement.User
{
    public class Role:IRole
    {
        private IUserRepository _rep;
        internal Role(IUserRepository rep, int appId, int id)
        {
            this.AppId = appId;
            this.Id = id;
            this._rep = rep;
        }
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public long AppId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Flag
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int RightFlag
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Id
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public int Save()
        {
            return this._rep.SaveRole(this);
        }
    }
}
