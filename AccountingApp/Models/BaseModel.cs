using System;
using System.ComponentModel.DataAnnotations;
using EntityFramework.DbDescriptionHelper;

namespace AccountingApp.Models
{
    public abstract class BaseModel
    {
        [ColumnDescription("自增主键")]
        public int PKID { get; set; }

        [Display(Name = "创建人")]
        [ColumnDescription("创建人")]
        public string CreatedBy { get; set; }

        [Display(Name = "创建时间")]
        [ColumnDescription("创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新人")]
        [ColumnDescription("更新人")]
        public string UpdatedBy { get; set; }

        [Display(Name ="更新时间")]
        [ColumnDescription("更新时间")]
        public DateTime UpdatedTime { get; set; }

        [ColumnDescription("是否已删除")]
        public bool IsDeleted { get; set; }
    }
}