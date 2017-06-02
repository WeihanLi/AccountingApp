namespace AccountingApp.Models
{
    /// <summary>
    /// 账单
    /// </summary>
    public class Bill : BaseModel
    {
        /// <summary>
        /// 账单标题
        /// </summary>
        public string BillTitle { get; set; }

        /// <summary>
        /// 账单详情
        /// </summary>
        public string BillDetails { get; set; }

        /// <summary>
        /// 账单总金额
        /// </summary>
        public decimal BillFee { get; set; }

        /// <summary>
        /// 账单类型
        /// </summary>
        public int BillType { get; set; }

        /// <summary>
        /// 账单状态
        /// 0：新建
        /// 1：申请报销
        /// 2：已报销
        /// </summary>
        public int BillStatus { get; set; }
    }
}