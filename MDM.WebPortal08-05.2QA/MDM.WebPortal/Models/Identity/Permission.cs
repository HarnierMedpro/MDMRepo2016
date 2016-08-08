using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using IdentitySample.Models;

namespace MDM.WebPortal.Models.Identity
{
    public class Permission
    {
        [Key]
        [Column(Order = 1)]
        public int PermissionID { get; set; }

        public int ActionID { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        /*Navegation Properties*/
        public virtual ActionSystem Action { get; set; }

        public virtual ICollection<ApplicationRole> Roles { get; set; }
    }
}