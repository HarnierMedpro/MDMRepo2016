using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMPosContact
    {
         public int ContactID { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid First Name.")]
        [StringLength(50)]
        public string FName { get; set; }

        [Required]
        [Display(Name = "LAST NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Last Name.")]
        [StringLength(50)]
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
        [Display(Name = "ACTIVE")]
        public bool active { get; set; }

        [Required]
        [UIHint("ContactType_POSEditor")]
        [Display(Name = "CONTACT TYPE")]
        public IEnumerable<VMContactType> ContactTypes { get; set; }

        public VMPosContact()
        {
            ContactTypes = new List<VMContactType>();
        }
    }
}