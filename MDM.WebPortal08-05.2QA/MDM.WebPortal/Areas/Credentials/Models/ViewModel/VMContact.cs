using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMContact
    {
        public int ContactID { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Provider Name.")]
        [StringLength(50)]
        public string FName { get; set; }

        [Required]
        [Display(Name = "LAST NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Provider Name.")]
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
        //[RegularExpression(@"[0-9-()]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool active { get; set; }

        [Required]
        [UIHint("ContactTypeEditorNew")]
        [Display(Name = "CONTACT TYPE")]
        public IEnumerable<VMContactType> ContactTypes { get; set; }

        public VMContact()
        {
            ContactTypes = new List<VMContactType>();
        }
    }
}