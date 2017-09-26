using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WeihanLi.EntityFramework.DbDescriptionHelper;

namespace AccountingApp.Models
{
    /// <summary>
    /// 账单
    /// </summary>
    [TableDescription("Bills","账单")]
    public class Bill : BaseModel
    {
        /// <summary>
        /// 账单标题
        /// </summary>
        [Display(Name = "账单标题")]
        [ColumnDescription("账单标题")]
        public string BillTitle { get; set; }

        /// <summary>
        /// 账单详情
        /// </summary>
        [Display(Name = "账单详情")]
        [ColumnDescription("账单详情")]
        public string BillDetails { get; set; }

        /// <summary>
        /// 账单总金额
        /// </summary>
        [Display(Name = "账单总金额")]
        [ColumnDescription("账单总金额")]
        public decimal BillFee { get; set; }

        /// <summary>
        /// 账单类型
        /// </summary>
        [Display(Name = "账单类型")]
        [ColumnDescription("账单类型")]
        public int BillType { get; set; }

        /// <summary>
        /// 账单状态
        /// 0：新建
        /// 1：申请报销
        /// 2：已报销
        /// 3：取消申请报销
        /// </summary>
        [Display(Name = "账单状态")]
        [ColumnDescription("账单状态(0：新建,1：申请报销,2：已报销,3：取消申请报销)")]
        public int BillStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public BillType AccountBillType { get; set; }
    }
}