using System;
using System.Collections.Generic;
using System.Data;
using JR.Cms.Domain.Interface.Content.Archive;
using JR.Cms.Domain.Interface.Site;
using JR.Cms.Domain.Interface.User;
using JR.Cms.Domain.Interface.Value;
using JR.Cms.ServiceContract;
using JR.Cms.ServiceRepository.Query;
using LoginResultDto = JR.Cms.ServiceDto.LoginResultDto;
using SiteDto = JR.Cms.ServiceDto.SiteDto;
using UserDto = JR.Cms.ServiceDto.UserDto;

namespace JR.Cms.ServiceImpl
{
    public class UserService:IUserServiceContract
    {
        private readonly IUserRepository _userRepository;
        private readonly UserQuery _userQuery;
        private readonly ISiteRepo _siteRepository;
        private IArchiveRepository _archiveRep;

        public UserService(IUserRepository rep,ISiteRepo siteRepository,IArchiveRepository archiveRep)
        {
            this._userRepository = rep;
            this._userQuery = new UserQuery();
            this._siteRepository = siteRepository;
            this._archiveRep = archiveRep;
        }

        public LoginResultDto TryLogin(string username, string password)
        {
            Credential cre = this._userRepository.GetCredentialByUserName(username);
            if (cre == null)
            {
                return new LoginResultDto {Tag = -1};
            }

            if (cre.Enabled == 0)
            {
                return new LoginResultDto {Tag = -2};
            }
            if (password != cre.Password)
            {
                return new LoginResultDto {Tag = -1};
            }

            //todo: 有无系统的角色和权限

            return new LoginResultDto
            {
                Uid = cre.UserId,
                Tag = 1,
            };
        }


        public UserDto GetUser(int id)
        {
            IUser user = this._userRepository.GetUser(id);
            if (user != null)
            {
                UserDto u = UserDto.Convert(user);
                u.Credential = user.GetCredential();
                u.Roles = user.GetAppRole();
                return u;
            }
            return null;
        }


        public Credential GetCredentialByUserName(string username)
        {
            return this._userRepository.GetCredentialByUserName(username);
        }



        public IList<UserDto> GetMyUserTableable(int appId, int id)
        {
            throw new NotImplementedException();
        }


        public DataTable GetMyUserTable(int appId, int userId)
        {
            return this._userQuery.GetMyUserTable(appId, userId);
        }


        public int SaveUser(UserDto user)
        {
            if (user.Id > 0)
            {
                return this.UpdateUser(user);
            }
            return this.CreateUser(user);
        }

        public DataTable GetAllUsers()
        {

            return this._userQuery.GetAllUser();
        }

   

        private int CreateUser(UserDto user)
        {
            if (this._userRepository.GetUserIdByUserName(user.Credential.UserName) > 0)
            {
                throw new ArgumentException("用户名已经使用!");
            }
            IUser usr = this._userRepository.CreateUser(0, 1);
            usr.Email = user.Email;
            usr.Phone = user.Phone;
            usr.Avatar = user.Avatar;
            usr.Name = user.Name;
            usr.CreateTime = DateTime.Now;
            usr.LastLoginTime = usr.CreateTime.AddHours(-1);
            user.CheckCode = "";
            if (user.Avatar == "" || String.IsNullOrEmpty(usr.Avatar))
            {
                usr.Avatar = "/public/mui/css/latest/avatar.gif";
            }
            if (user.IsMaster)
            {
                usr.Flag = Role.Master.Flag;
            }

            int userId = usr.Save();
            usr = this._userRepository.GetUser(userId);
            usr.SaveCredential(new Credential(0, userId, user.Credential.UserName, 
                user.Credential.Password,user.Credential.Enabled));
            return userId;
        }

        private int UpdateUser(UserDto user)
        {
            IUser usr = this._userRepository.GetUser(user.Id);
            usr.Email = user.Email;
            usr.Phone = user.Phone;
            usr.Avatar = user.Avatar;
            usr.Name = user.Name;
            int row = usr.Save();
            if (row > 0)
            {
                if (usr.GetCredential().UserName != user.Credential.UserName)
                {
                    if (this._userRepository.GetUserIdByUserName(user.Credential.UserName) > 0)
                    {
                        throw new ArgumentException("用户名已经使用!");
                    }
                }
                user.Credential.UserId = usr.GetAggregaterootId();
                usr.SaveCredential(user.Credential);
            }
            return row;
        }


        public void SaveUserRole(int userId, int appId, int[] flags)
        {
            IUser usr = this._userRepository.GetUser(userId);
            if(usr== null)throw  new Exception("no such user");
            usr.SetRoleFlags(appId, flags);
        }


        public Dictionary<int, int[]> GetUserAppRoles(int userId)
        {
            IUser usr = this._userRepository.GetUser(userId);
            if (usr == null) throw new Exception("no such user");
            Dictionary<int,int[]> dict = new Dictionary<int, int[]>();
           IDictionary<int,AppRolePair> data= this._userRepository.GetUserRoles(userId);
            foreach (int siteId in data.Keys)
            {
                dict.Add(siteId,data[siteId].Flags.ToArray());
            }
            return dict;
        }


        public SiteDto[] GetUserRelationSites(int userId)
        {
            IDictionary<int, AppRolePair> data = this._userRepository.GetUserRoles(userId);
            int len = data.Keys.Count;
            SiteDto[] sites = new SiteDto[len];
            if (len > 0)
            {
                int i = 0;
                foreach (var key in data.Keys)
                {
                    sites[i++] = SiteDto.ConvertFromSite( this._siteRepository.GetSiteById(data[key].AppId));
                }
              
            }
            return sites;
        }

        public string GetUserRealName(int publisherId)
        {
            return this._userQuery.GetUserRealName(publisherId);
        }

        public int DeleteUser(int userId)
        {
            IUser user = this._userRepository.GetUser(userId);
            if (user == null)
            {
                throw new ArgumentException("no such user");
            }

            foreach (var siteId in user.GetAppRole().GetSiteIds())
            {
                var pair = user.GetAppRole().GetRole(siteId);
                if (pair != null && Role.Master.Match(pair.GetFlag()))
                {
                    throw new Exception("Can't delete web master.");
                }
            }

            int firstUserId = this._userQuery.GetFirstUserId();

            int total = this._archiveRep.TransferArchives(userId, firstUserId);

          return  this._userRepository.DeleteUser(user.GetAggregaterootId());
        }
    }
}
