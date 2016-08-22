using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.Identity
{
    public class Menu
    {
        [Key]
        [Column(Order = 1)]
        public int MenuID { get; set; }

        //public Nullable<int> ParentId { get; set; }

        public Nullable<int> ActionID { get; set; }

        [Required]
        [Display(Name = "TITLE")]
        [StringLength(50)]
        public string Title { get; set; }

        public virtual ActionSystem actionSystem { get; set; }

        /*Puede tener un padre*/
        public virtual Menu Parent { get; set; }

        /*Puede tener submenus*/
        public virtual ICollection<Menu> ChildMenus { get; set; } 
    }
}