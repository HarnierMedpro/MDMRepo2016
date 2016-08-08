using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class MasterUserListMeta
    {
        [Required]
        public string ADP_ID { get; set; }

        [Required]
        public string EmployeeName { get; set; }
    }

    [MetadataTypeAttribute(typeof(MasterUserListMeta))]
    public partial class MasterUserList
    {
    }
}