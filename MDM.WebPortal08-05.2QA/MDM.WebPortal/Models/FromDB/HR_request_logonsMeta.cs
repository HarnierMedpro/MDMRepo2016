using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class HR_request_logonsMeta
    {
        [Required]
        public int HR_requestID { get; set; }

        [Required]
        public int logon_ID { get; set; }

        [Required]
        public string Logon_Name { get; set; }

        [Required]
        public int zno { get; set; }
    }

    [MetadataTypeAttribute(typeof(HR_request_logonsMeta))]
    public partial class HR_request_logons
    {
    }
}