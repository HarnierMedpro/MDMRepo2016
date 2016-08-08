using MDM.WebPortal.Data_Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class Corp_OwnerMeta
    {
        [Required]
        [BiggerThanCero(ErrorMessage ="This field is required.")]
        public int corpID { get; set; }

        [Required]
        [BiggerThanCero(ErrorMessage = "This field is required.")]
        public int OwnersID { get; set; }
    }
    [MetadataType(typeof(Corp_OwnerMeta))]
    public partial class Corp_Owner
    {

    }
}