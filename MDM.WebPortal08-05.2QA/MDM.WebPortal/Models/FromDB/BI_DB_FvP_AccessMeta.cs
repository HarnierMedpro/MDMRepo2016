using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class BI_DB_FvP_AccessMeta
    {
        [Required]
        [Display(Name = "MANAGER")]
        public int ManagerID { get; set; }

        [Required]
        [Display(Name = "DB")]
        public int DB_ID { get; set; }

        [Required]
        [Display(Name = "FvP")]
        public int FvPID { get; set; }
    }
}