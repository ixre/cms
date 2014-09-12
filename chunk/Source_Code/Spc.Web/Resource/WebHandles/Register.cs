//
// Register WebHandler
// Copyright 2011 @ OPS Inc,All rights reseved!
// newmin @ 2011/03/16
//
namespace OPSite.WebHandler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Ops.Web;
    using Ops.Cms.BLL;

    [WebExecuteable]
    public class Register
    {
        [Post(AllowRefreshMillliSecond = 1000)]
        public bool DetectUserExit(string username)
        {
           return new MemberBLL().DetectUsernameAvailable(username);
        }
        [Post(AllowRefreshMillliSecond = 1000)]
        public bool DetectNicknameExit(string username)
        {
            return new MemberBLL().DetectNickNameAvailable(username);
        }
    }
}