using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels
{
    public class VMBI_DB_FvP
    {
        public int BIDbFvPID { get; set; } //PK

        //public int ManagerID { get; set; } //FK from Manager_Master table

        public int ManagerID { get; set; }

        public int DB_ID { get; set; }  //FK from DBList table

        public int FvPID { get; set; } //FK from FvPList table

        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
}