using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMPOSExtraInfo
    {
        public int POSExtraDataID { get; set; } //PK

        public int MasterPOSID { get; set; }

        [Display(Name = "PHONE NUMBER")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string Phone_Number { get; set; }

        [Display(Name = "FAX NUMBER")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string Fax_Number { get; set; }

        [Display(Name = "WEBSITE")]
        [StringLength(50)]
        public string Website { get; set; }

        [Display(Name = "ADMISSION PHONE NUMBER")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string AdmissionPhone { get; set; }

        [Display(Name = "SCHOLARSHIP RATE")]
        [StringLength(100)]
        public string ScholarshipRate { get; set; }

        [Display(Name = "AVERAGE LEHGHT OF STAY")]
        [StringLength(100)]
        public string AverageLenOfStay { get; set; }

        [Display(Name = "UA PANELS")]
        [StringLength(100)]
        public string HowManyUAPanels { get; set; }

        [Display(Name = "PAID TO PATIENT STATE")]
        [StringLength(100)]
        public string PaidToPatientState { get; set; }

        [Display(Name = "MEDICARE NUMBER")]
        public string MedicareNumber { get; set; }

        [Display(Name = "NUMBER OF BEDS")]
        public string Number_of_beds { get; set; }

        [Display(Name = "HOW MANY WEEKDAYS OPEN")]
        public string how_many_days_week_open { get; set; }

        public string Ancillary_outpatient_services { get; set; }

        public string Out_of_Network_In_Network { get; set; }

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

        [Display(Name = "IN SERVICE DATE TIME")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> In_Service_date_time { get; set; }

        [Display(Name = "MANAGE CARE CONTRACTS")]
        public string Manage_care_contracts { get; set; }
    }
}