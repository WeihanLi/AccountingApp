﻿using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "账单标题")]
        public string BillTitle { get; set; }

        /// <summary>
        /// 账单详情
        /// </summary>
        [Display(Name = "账单详情")]
        public string BillDetails { get; set; }

        /// <summary>
        /// 账单总金额
        /// </summary>
        [Display(Name = "账单总金额")]
        public decimal BillFee { get; set; }

        /// <summary>
        /// 账单类型
        /// </summary>
        [Display(Name = "账单类型")]
        public int BillType { get; set; }

        /// <summary>
        /// 账单状态
        /// 0：新建
        /// 1：申请报销
        /// 2：已报销
        /// </summary>
        [Display(Name = "账单状态")]
        public int BillStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public BillType AccountBillType { get; set; }
    }
}