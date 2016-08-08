using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class MDM_ADP_ListMeta
    {
        [Required]
        public string ADP_ID { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public string Job_Title { get; set; }

        [Required]
        public string Staff_Manager { get; set; }

        [Required]
        public string User_Active { get; set; }

        [Required]
        public string Type { get; set; }
    }

    [MetadataTypeAttribute(typeof(MDM_ADP_ListMeta))]
    public partial class MDM_ADP_List
    {
    }
}