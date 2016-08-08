using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ADP.Models
{
    public class VMAdpMaster
    {
        public int ADPMaster_ID { get; set; }

        public string ADP { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        [StringLength(50)]
        public string FName { get; set; }

        [Required]
        [Display(Name = "LAST NAME")]
        [StringLength(50)]
        public string LName { get; set; }

        [Required]
        [Display(Name = "TITLE")]
        [StringLength(80)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "MANAGER")]
        [StringLength(80)]
        public string Manager { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        [Display(Name = "FULL NAME")]
        public string FullName
        {
            get
            {
                return LName + ", " + FName;
            }
            
        }
    }
}