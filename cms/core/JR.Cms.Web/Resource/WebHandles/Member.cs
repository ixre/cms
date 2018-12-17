//
// Member HttpHandler
// Copyright 2011 @ OPS Inc,All rights reseved !
// newmin @ 2011/03/20
//
namespace OPSite.WebHandler
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;
    using J6.Data;
    using J6.Web;
    using J6.Cms;
    using J6.Cms.Models;
    using J6.Cms.BLL;

    [WebExecuteable]
    public class Member
    {
        [Logon]
        [Post(AllowRefreshMillliSecond = 1000)]
        public string Update(string module)
        {
            MemberBLL bll = new MemberBLL();
            global::J6.Cms.Models.Member member = bll.GetMember(UserState.Member.Current.ID);
            if (member == null) return "location.href='/app.axd?go:login';";
            HttpRequest request = HttpContext.Current.Request;

            switch (module)
            {
                case "profile":

                    string nickname = request["nickname"],
                           telephone = request["telephone"],
                           note = request["note"],
                           sex = request["sex"],
                           email = request["email"];

                    member.Nickname = nickname;
                    member.TelePhone = telephone;
                    member.Avatar = string.IsNullOrEmpty(member.Avatar) ? "/images/noavatar.gif" : member.Avatar;
                    member.Note = note;
                    member.Sex = sex;
                    member.Email = email;
                    bll.Update(member);
                    return "<script>window.parent.op.tip('修改成功!')</script>";
                case "password":
                    string originalPassword = request["originalPassword"];
                    string password = request["password"];
                    if (member.Password == (member.Username.ToUpper() +originalPassword.ToLower()).EncodeMD5())
                    {
                        if (!String.IsNullOrEmpty(password) && password.Length >= 6)
                            member.Password = (member.Username.ToUpper() + password.ToLower()).EncodeMD5();
                        else
                            return "<script>window.parent.op.tip('密码长度必须大于6!')</script>";
                        bll.Update(member);
                        return "<script>window.parent.op.tip('修改成功!')</script>";
                    }
                    else
                        return "<script>window.parent.op.tip('原密码不正确!')</script>";
            }
            return "<script>window.parent.op.tip('未指定操作!')</script>";
        }
        [Post]
        [Logon]
        //当前堆栈中的方法都被赋予无限制的权限
        //[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        public string UploadAvatar()
        {
            global::J6.Cms.Models.Member m = UserState.Member.Current;
            HttpContext context = HttpContext.Current;
            HttpPostedFile file = context.Request.Files[0];
            if (file.ContentLength < 20) return "<script>window.parent.op.tip('不是有效的图片文件!')</script>";
            else if (file.ContentLength > 2048000) return "<script>window.parent.op.tip('图片大小不能超过2M')</script>";

            string imgFileName = (m.Username.ToString() + DateTime.Now.ToString()).Encode16MD5() + ".jpg";
            string dir = HttpContext.Current.Server.MapPath(Settings.MM_AVATAR_PATH);
            
            // 头像目录不存在则创建
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            float b = 1;                           //缩放比例
            float maxWidth = 100;                  //最大宽度
            float maxHeight = 100;                 //最大高度
            float dpiX;                          //绘图区域水平像素
            float dpiY;                          //绘图区域垂直像素

            using (Bitmap orgImg = new Bitmap(file.InputStream))
            {
                //从orgImg绘图，以获取分辨率
                Graphics g = Graphics.FromImage(orgImg);
                dpiX = g.DpiX;
                dpiY = g.DpiY;

                //检测长宽比例并创建新的图像
                Bitmap img;
                if (orgImg.Width - maxWidth >= orgImg.Height - maxHeight)
                {
                    b = maxWidth / (float)orgImg.Width;
                    img = new Bitmap((int)maxWidth, (int)(orgImg.Height * b));
                }
                else
                {
                    b = maxHeight / (float)orgImg.Height;
                    img = new Bitmap((int)(orgImg.Width * b), (int)maxHeight);
                }
                g.Dispose();

                //从img绘图
                g = Graphics.FromImage(img);
                g.Clear(Color.White);

                //设置原图的分辨率
                //g.DpiX为浮点数，先计算
                orgImg.SetResolution(g.DpiX * orgImg.Width / img.Width, g.DpiY * orgImg.Height / img.Height);
                //设置低质量补值法
                g.InterpolationMode = InterpolationMode.Low;
                //消除锯齿
                g.SmoothingMode = SmoothingMode.AntiAlias;
                //将img上的坐标(0,0)绘出orgImg
                g.DrawImage(orgImg, new Point(0, 0));

                //释放资源
                g.Dispose();
                orgImg.Dispose();

                MemoryStream ms = new MemoryStream();

                //通过编码器及设置参数保存到内存流

                //设置品质,品质参数需要转为long
                EncoderParameters encodeParams = new EncoderParameters(1);
                encodeParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)90);
                //获取JPEG编码器
                ImageCodecInfo[] codes = ImageCodecInfo.GetImageDecoders();
                ImageCodecInfo _code = null;
                foreach (ImageCodecInfo code in codes)
                {
                    if (String.Compare(code.MimeType, "image/jpeg", true) == 0)
                        _code = code;
                }
                img.Save(ms, _code, encodeParams);

                img.Dispose();
                
                using (FileStream fs = new FileStream(dir + imgFileName, FileMode.Create))
                {
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(ms.ToArray());
                    fs.Flush();
                    bw.Close();
                    fs.Close();
                }
                img.Dispose();
            }

            //更新到数据库
            m.Avatar = Settings.MM_AVATAR_PATH + imgFileName;
            new MemberBLL().Update(m);
            return "<script>window.parent.location.href='/member/edit?module=avatar';</script>";
        }

        [Logon]
        public string SendMessage()
        {
            HttpRequest request=HttpContext.Current.Request;

            int uid=int.Parse(request["uid"]);
            string subject=request["subject"];
            string content=request["msgcontent"];
            if (subject.Length < 6 || subject.Length > 50)
                return "<script>window.parent.op.tip('主题长度应为6-50个字符或3-25个汉字!')</script>";
            else if(content.Length<10||content.Length>150)
                return "<script>window.parent.op.tip('内容长度应为10-120个字符!')</script>";

            global::J6.Cms.Models.Member m = UserState.Member.Current;
            MessageBLL bll = new MessageBLL();
            bll.SendMessage(m.ID, uid, subject, content);

            return "<script>window.parent.location.href='message?module=sendbox';</script>";
        }
        [Post]
        public string DelMsg(string id)
        {
            global::J6.Cms.Models.Member m = UserState.Member.Current;
            if (m == null) return "请先登录";
            int msgID = int.Parse(id);
            bool result=new MessageBLL().Delete(m.ID, msgID);

            return result?"":"你没有权限删除本信息!";
        }

        [Post]
        public bool Delete(string id,string deleteRelationInfo)
        {
            MemberBLL bll = new MemberBLL();
            bll.Delete(int.Parse(id),deleteRelationInfo.ToLower()=="true");
            return true;
        }

        public void Exit()
        {
            //清除用户信息
            UserState.Member.Current = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies["member"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
            //跳转到退出之前页面
            HttpContext context = HttpContext.Current;
            string uri = context.Request.Headers["referer"];
            context.Response.Redirect(uri);
        }
    }
}
