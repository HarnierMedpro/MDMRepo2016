using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class OwnerListMeta
    {
        [Required]
        [StringLength(50)]
        [Display(Name="Last Name")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Last Name.")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name="First Name")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid First Name.")]
        public string FirstName { get; set; }
    }
    [MetadataType(typeof(OwnerListMeta))]
    public partial class OwnerList
    {

    }
}