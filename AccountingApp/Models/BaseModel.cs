using System;
using System.ComponentModel.DataAnnotations;

namespace AccountingApp.Models
{
    public abstract class BaseModel
    {
        public int PKID { get; set; }

        [Display(Name = "创建人")]
        public string CreatedBy { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新人")]
        public string UpdatedBy { get; set; }

        [Display(Name ="更新时间")]
        public DateTime UpdatedTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}