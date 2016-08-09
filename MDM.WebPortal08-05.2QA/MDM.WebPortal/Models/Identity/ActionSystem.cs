using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.Identity
{
    public class ActionSystem
    {
        [Key]
        [Column(Order = 1)]
        public int ActionID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "This field does not allow empty strings.")]
        [RegularExpression(@"[a-zA-Z0-9_]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        [Display(Name = "ACTION NAME")]
        [StringLength(50)]
        public string Act_Name { get; set; }

        public int ControllerID { get; set; }

        public virtual ControllerSystem ControllerSystem { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}