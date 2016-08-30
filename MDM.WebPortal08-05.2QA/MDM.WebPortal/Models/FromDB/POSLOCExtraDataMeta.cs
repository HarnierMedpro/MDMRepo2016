using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class POSLOCExtraDataMeta
    {
        [Display(Name = "PHONE NUMBER")]
        [DataType(DataType.PhoneNumber)] 
        public string Phone_Number { get; set; }

        [Display(Name = "FAX NUMBER")]
        [DataType(DataType.PhoneNumber)] 
        public string Fax_Number { get; set; }

        [Display(Name = "WEBSITE")]
        public string Website { get; set; }

        [Display(Name = "NUMBER OF BEDS")]
        public string Number_of_beds { get; set; }

        [Display(Name = "HOW MANY WEEKDAYS OPEN")]
        public string how_many_days_week_open { get; set; }

        [Display(Name = "LABORATORY NAME")]
        public string Lab_Name { get; set; }

        [Display(Name = "BCBS ID NUMBER")]
        public string BCBS_ID_Number { get; set; }

        [Display(Name = "UPIN NUMBER")]
        public string UPIN_Number { get; set; }

        [Display(Name = "MEDICAID NUMBER")]
        public string Medicaid_Number { get; set; }

        [Display(Name = "STATE OF MCD OR PHYGrp")]
        public string State_of_MD_or_PhyGrp { get; set; }
       
        
        public string JACHO_CARF { get; set; }
       
        [Display(Name = "MANAGE CARE CONTRACTS")]
        public string Manage_care_contracts { get; set; }
       
        [Display(Name = "IN SERVICE DATE TIME")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> In_Service_date_time { get; set; }
        
    }
    [MetadataType(typeof(POSLOCExtraDataMeta))]
    public partial class POSLOCExtraData
    {
    }
}