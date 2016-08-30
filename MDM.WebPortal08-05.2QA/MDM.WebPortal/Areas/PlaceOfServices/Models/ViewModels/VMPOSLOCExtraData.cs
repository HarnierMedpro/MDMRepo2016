using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMPOSLOCExtraData
    {
        public int FACInfoData_FACInfoDataID { get; set; }

        [Display(Name = "PHONE NUMBER")]
        [DataType(DataType.PhoneNumber)]
        public string Phone_Number { get; set; }

        [Display(Name = "FAX NUMBER")]
        [DataType(DataType.PhoneNumber)]
        public string Fax_Number { get; set; }

        [Display(Name = "WEBSITE")]
        public string Website { get; set; }

        public Nullable<bool> W_9_on_File { get; set; }

        [Display(Name = "NUMBER OF BEDS")]
        public string Number_of_beds { get; set; }

        public Nullable<bool> Have_24hrs_nursing { get; set; }

        [Display(Name = "HOW MANY WEEKDAYS OPEN")]
        public string how_many_days_week_open { get; set; }

        public string Out_of_Network_In_Network { get; set; }

        [Display(Name = "LABORATORY NAME")]
        public string Lab_Name { get; set; }

        public Nullable<bool> Licensure_on_File { get; set; }

        public Nullable<bool> Mental_License { get; set; }

        [Display(Name = "BCBS ID NUMBER")]
        public string BCBS_ID_Number { get; set; }

        [Display(Name = "UPIN NUMBER")]
        public string UPIN_Number { get; set; }

        [Display(Name = "MEDICAID NUMBER")]
        public string Medicaid_Number { get; set; }

        [Display(Name = "STATE OF MCD OR PHYGrp")]
        public string State_of_MD_or_PhyGrp { get; set; }

        public Nullable<bool> Regulations_on_File { get; set; }

        public string JACHO_CARF { get; set; }

        public Nullable<bool> Has_facility_billed_ins_before { get; set; }

        [Display(Name = "MANAGE CARE CONTRACTS")]
        public string Manage_care_contracts { get; set; }

        public Nullable<bool> Copies_of_all_managed_care_contracts_on_file { get; set; }

        public Nullable<bool> Forms_created { get; set; }

        public Nullable<bool> In_Service_scheduled { get; set; }

        [Display(Name = "IN SERVICE DATE TIME")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> In_Service_date_time { get; set; }

        public Nullable<bool> Portal_training_setup { get; set; }

        public Nullable<bool> email_regarding_conference_set_up { get; set; }

        public Nullable<bool> Database_set_up { get; set; }

        public Nullable<bool> Availavility_request_sent_out { get; set; }

        public Nullable<bool> Availavility_completed { get; set; }

        public Nullable<bool> Navinet_request_completed { get; set; }

        public Nullable<bool> Fee_schedule_in_binder { get; set; }

        public IEnumerable<SelectListItem> formsDictionaries { get; set; }

        public VMPOSLOCExtraData()
        {
            formsDictionaries = new List<SelectListItem>();
        }

    }
}