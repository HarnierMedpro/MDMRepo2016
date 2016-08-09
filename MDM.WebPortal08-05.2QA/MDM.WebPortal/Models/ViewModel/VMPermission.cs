using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    /*See: C:\Users\hsuarez\Desktop\Documentation\Kendo UI for ASP.NET MVC5\ui-for-aspnet-mvc-examples-master\grid\multiselect-in-grid-popup*/
    public class VMPermission
    {
        public int PermissionID { get; set; }

        //[Required]
        //[Display(Name = "CONTROLLER")]
        //[StringLength(50)]
        //public string ControllerName { get; set; }

        //[Required]
        //[Display(Name = "ACTION")]
        //[StringLength(50)]
        //public string ActionName { get; set; }
        [Display(Name = "CONTROLLER")]
        public int ControllerID { get; set; }

        [Display(Name = "ACTION")]
        public int ActionID { get; set; }

        [Display(Name = "ACTIVE")]
        [Required]
        public bool Active { get; set; }

        [Required]
        [UIHint("RolesEditor")] //Specifies the template or user control that Dynamic Data uses to display a data field.
        public IEnumerable<VMPermissionRole> Roles { get; set; }

        public VMPermission()
        {
            Roles = new List<VMPermissionRole>();
        }
    }
}