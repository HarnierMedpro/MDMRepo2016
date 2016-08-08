using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class MDM_EdgeMED_ADP_UserListMeta
    {
        [Required]
        public string ADP_ID { get; set; }

        [Required]
        public long ZoomServerNo { get; set; }

        [Required]
        public long EdgeMed_ID { get; set; }

        [Required]
        public int MDM_ADP_ListID { get; set; }

        [Required]
        public string Edgemed_UserName { get; set; }
    }
    [MetadataTypeAttribute(typeof(MDM_EdgeMED_ADP_UserListMeta))]
    public partial class MDM_EdgeMED_ADP_UserList
    {
    }
}