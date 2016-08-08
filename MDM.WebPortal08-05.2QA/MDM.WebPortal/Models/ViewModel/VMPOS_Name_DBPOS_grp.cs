using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMPOS_Name_DBPOS_grp
    {
        //PK for MDM_POS_Name_DBPOS_grp
        public int MDMPOS_NameID { get; set; }

        //FK for DBList
        public int DB_ID { get; set; }

        //FK for Fac in Pos_Tab
        public long FacilityID { get; set; }

        public string Extra { get; set; }

        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        //FK for MDM_POS_ListName
        public int MDMPOS_ListNameID { get; set; }
    }
}