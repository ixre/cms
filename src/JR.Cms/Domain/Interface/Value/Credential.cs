namespace JR.Cms.Domain.Interface.Value
{
    /// <summary>
    /// 用户凭据
    /// </summary>
    public class Credential : IValueObject
    {
        public Credential(int id, int userId, string userName, string password, int enabled)
        {
            Id = id;
            UserId = userId;
            UserName = userName;
            Password = password;
            Enabled = enabled;
        }

        public int UserId { get; set; }

        public int Id { get; set; }

        /// <summary>
        ///用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否使用 
        /// </summary>
        public int Enabled { get; set; }

        public bool Equal(IValueObject that)
        {
            return false;
        }
    }
}