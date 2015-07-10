//
// MemberBLL.cs   会员逻辑层
// Copryright 2011 @ S1N1.COM,All rights reseved !
// Create by newmin @ 2011/03/16
//

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using J6.Cms.DAL;
using J6.Cms.Domain.Interface.Models;
using J6.Cms.Domain.Interface._old;
using J6.DevFw.Data.Extensions;

namespace J6.Cms.BLL
{
    /// <summary>
    /// 会员逻辑
    /// </summary>
    public sealed class MemberBLL : Imember
    {
        private MemberDAL dal = new MemberDAL();

        /// <summary>
        /// 检测用户名是否可用
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool DetectUsernameAvailable(string username)
        {
            return dal.DetectUserName(username);
        }

        /// <summary>
        /// 检测昵称是否可用
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public bool DetectNickNameAvailable(string nickname)
        {
            return dal.DetectNickName(nickname);
        }

        /// <summary>
        /// 创建会员密钥
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GenericMemberToken(string username)
        {
            byte[] bytes=Encoding.UTF8.GetBytes(username + "," + DateTime.Now.ToString("{yyyy/MM/dd HH:mm:ss}"));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 验证密钥
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool VerifyToken(string token,string username)
        {
           // char[] Convert.ToBase64CharArray();
            return true;
        }


        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="nickname">昵称</param>
        /// <param name="email">电子邮箱</param>
        /// <param name="needActive">是否需要激活</param>
        /// <returns>返回注册成功的用户编号</returns>
        public int Regsiter(string username, string password, string nickname, string email,string ip,bool needActive)
        {
            //如果用户名和昵称已经被注册，则返回-1
            if (dal.DetectUserAndNickNameExist(username, nickname)) return -1;

            //注册会员
            dal.RegisterMember(
                username,
                password,
                "",
                "UnKnown",
                nickname,
                "",
                email,
                "");

            int uid = dal.GetMemberUid(username);

            dal.InsertDetails(
                uid,
                needActive ? "Active" : "Normal",
                ip,
                GenericMemberToken(username)
            );
            return uid;
        }

        /// <summary>
        /// 根据用户名和密码验证用户是否存在,不存在返回null
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Member VerifyMember(string username, string password)
        {
            Member m = null;
            dal.VerifyMember(username, password, reader =>
            {
                if (reader.HasRows)
                {
                    m = reader.ToEntity<Member>();
                }
            });
            return m;
        }

        /// <summary>
        /// 根据会员ID获取会员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Member GetMember(int memberID)
        {
            Member m = null;
            dal.GetMemberByID(memberID, reader =>
            {
                if (reader.HasRows)
                {
                    m = reader.ToEntity<Member>();
                }
            });
            return m;
        }

        /// <summary>
        /// 根据会员用户名获取会员
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Member GetMember(string username)
        {
            Member m = null;
            dal.GetMemberByUsername(username, reader =>
            {
                if (reader.HasRows)
                {
                    m = reader.ToEntity<Member>();
                }
            });
            return m;
        }


        /// <summary>
        /// 根据会员ID获取会员及详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="member"></param>
        /// <param name="details"></param>
        public void GetMemberDetails(int id, out Member member, out MemberDetails details)
        {
            Member m=null;
            MemberDetails d=null;
            dal.GetMemberDetailsByID(id, rd =>
            {
                if (rd.HasRows)
                {
                    rd.Read();
                    FillMemberDetails(rd, out m, out d);
                }
            });
            member = m;
            details = d;
        }

        /// <summary>
        /// 根据会员用户名获取会员及详细信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="member"></param>
        /// <param name="details"></param>
        public void GetMemberDetails(string username, out Member member, out MemberDetails details)
        {
            Member m = null;
            MemberDetails d = null;
            dal.GetMemberDetailsByUsername(username, rd =>
            {
                if (rd.HasRows)
                {
                    rd.Read();
                    FillMemberDetails(rd, out m, out d);
                }
            });
            member = m;
            details = d;
        }

        /// <summary>
        /// 更新资料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="nickname"></param>
        /// <param name="email"></param>
        /// <param name="sex"></param>
        /// <param name="note"></param>
        public void Update(Member member)
        {
            dal.Update(member.ID, member.Password, member.Nickname, member.Avatar, member.Sex, member.Email, member.TelePhone, member.Note);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户数字ID</param>
        /// <param name="deleteInfo">是否删除用户的文档评论等相关信息</param>
        public void Delete(int id,bool deleteInfo)
        {
            dal.Delete(id);
            if (deleteInfo)
            {
                //
                //TODO:
                //
                // ArchiveBLL bll = new ArchiveBLL();
                //bll.DeleteMemberArchive(id);
                CommentBll cbll = new CommentBll();
                cbll.DeleteMemberComments(id);
            }
        }

        /// <summary>
        /// 获取分页会员
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public DataTable GetPagedMembers(int pageSize, ref int currentPageIndex, out int recordCount, out int pageCount)
        {
            return dal.GetPagedMembers(pageSize, ref currentPageIndex, out recordCount, out pageCount, null);
        }



        /// <summary>
        /// 根据数据行为会员赋值信息
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="member"></param>
        /// <param name="details"></param>
        private void FillMemberDetails(DbDataReader rd, out Member member, out MemberDetails details)
        {
            member = new Member();
            member.ID = rd.GetInt32(0);
            member.Username = rd["username"].ToString();
            member.Password = rd["password"].ToString();
            member.Avatar = rd["avatar"].ToString();
            member.Nickname = rd["nickname"].ToString();
            member.Sex =rd["sex"].ToString();
            member.TelePhone = rd["telephone"].ToString();
            member.Note = rd["note"].ToString();
            member.Email = rd["email"].ToString();

            details = new MemberDetails();
            details.UID = member.ID;
            details.Token = rd["token"].ToString();
            details.Status = rd["status"].ToString();
            details.RegIP = rd["regip"].ToString();
            details.RegTime = Convert.ToDateTime(rd["RegTime"]);
            details.LastLoginTime = Convert.ToDateTime(rd["Lastlogintime"]);
        }
    }
}