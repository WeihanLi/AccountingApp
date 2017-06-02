using System.ComponentModel.DataAnnotations;

namespace AccountingApp.Models
{
    /// <summary>
    /// 账单类型
    /// </summary>
    public class BillType : BaseModel
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [Display(Name ="类型名称")]
        [Required]
        public string TypeName { get; set; }

        /// <summary>
        /// 类型介绍
        /// </summary>
        [Display(Name = "类型介绍")]
        [Required]
        public string TypeDesc { get; set; }
    }
}