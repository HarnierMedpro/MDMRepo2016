using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMContact
    {
        public int ContactID { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        [StringLength(50)]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Provider Name.")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "LAST NAME")]
        [StringLength(50)]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Provider Name.")]
        public string LName { get; set; }

        [Required]
        [Display(Name = "EMAIL")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "PHONE NUMBER")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(14, MinimumLength = 14)]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "CONTACT TYPE")]
        public int ContactTypeID { get; set; }
    }
}