namespace J6.Cms.Extend.SSO
{
    /// <summary>
    /// 会话传输结果对象
    /// </summary>
    public struct  SessionResult
    {
        /// <summary>
        /// 结果
        /// </summary>
        public bool Result{get;set;}

        /// <summary>
        /// 消息
        /// </summary>
        public string Message{get;set;}

        /// <summary>
        /// 用户
        /// </summary>
        public Person  Person{get;set;}
    }
}
