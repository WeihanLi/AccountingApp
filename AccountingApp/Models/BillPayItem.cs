using WeihanLi.EntityFramework.DbDescriptionHelper;

namespace AccountingApp.Models
{
    /// <summary>
    /// 单笔账单支付
    /// </summary>
    [TableDescription("BillPayItems", "账单支付详情")]
    public class BillPayItem : BaseModel
    {
        public BillPayItem()
        {
        }

        public BillPayItem(string personName, decimal payMoney)
        {
            PersonName = personName;
            PayMoney = payMoney;
        }

        /// <summary>
        /// 关联账单id
        /// </summary>
        [ColumnDescription("账单关联id")]
        public int BillId { get; set; }

        /// <summary>
        /// 支付人姓名
        /// </summary>
        [ColumnDescription("支付人姓名")]
        public string PersonName { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [ColumnDescription("支付金额")]
        public decimal PayMoney { get; set; }
    }
}