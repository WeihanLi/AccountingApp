using EntityFramework.DbDescriptionHelper;

namespace AccountingApp.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    [TableDescription("账单用户表")]
    public class User : BaseModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [ColumnDescription("用户名")]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ColumnDescription("密码哈希")]
        public string PasswordHash { get; set; }

        [ColumnDescription("微信id")]
        public string WechatOpenId { get; set; }

        [ColumnDescription("是否启用")]
        public bool IsActive { get; set; }
    }
}