namespace AccountingApp.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : BaseModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PasswordHash { get; set; }

        public string WechatOpenId { get; set; }

        public bool IsActive { get; set; }
    }
}