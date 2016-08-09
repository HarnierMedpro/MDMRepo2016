using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMOwnerList
    {
        public int OwnersID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "LAST NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Last Name.")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "FIRST NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid First Name.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name="ACTIVE")]
        public Nullable<bool> active { get; set; }
    }
}