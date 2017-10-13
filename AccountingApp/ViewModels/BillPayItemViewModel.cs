using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApp.ViewModels
{
    public class BillPayItemViewModel
    {
        /// <summary>
        /// 支付人姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayMoney { get; set; }
    }
}
