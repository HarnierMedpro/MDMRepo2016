using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.Identity
{
    public class ControllerSystem
    {
        [Key]
        [Column(Order = 1)]
        public int ControllerID { get; set; }

        [Required]
        [Display(Name = "CONTROLLER NAME")]
        [StringLength(50)]
        public string Cont_Name { get; set; }

        public virtual ICollection<ActionSystem> ActionSystems { get; set; } 
    }
}