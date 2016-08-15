using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.AudiTrails.Models
{
    public class VMAuditLog
    {
        public int AuditLogID { get; set; }

        [Display(Name = "TABLE NAME")]
        public string TableName { get; set; }

        [Display(Name = "USER")]
        public string UserLogons { get; set; }

        [Display(Name = "DATE_TIME")]
        public DateTime AuditDateTime { get; set; }

        [Display(Name = "COLUMN NAME")]
        public string Field_ColumName { get; set; }

        [Display(Name = "OLD VALUE")]
        public string OldValue { get; set; }

        [Display(Name = "NEW VALUE")]
        public string NewValue { get; set; }

        [Display(Name = "ACTION")]
        public string AuditAction { get; set; }

        [Display(Name = "MODEL PRIMARY KEY")]
        public int ModelPKey { get; set; }
    }
}