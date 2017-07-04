using System.ComponentModel.DataAnnotations;
using EntityFramework.DbDescriptionHelper;

namespace AccountingApp.Models
{
    /// <summary>
    /// 账单类型
    /// </summary>
    [TableDescription("账单类型")]
    public class BillType : BaseModel
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [Display(Name ="类型名称")]
        [Required]
        [ColumnDescription("账单类型名称")]
        public string TypeName { get; set; }

        /// <summary>
        /// 类型介绍
        /// </summary>
        [Display(Name = "类型介绍")]
        [Required]
        [ColumnDescription("账单类型描述")]
        public string TypeDesc { get; set; }
    }
}