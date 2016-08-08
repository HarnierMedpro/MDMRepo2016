using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class Manager_TypeMeta
    {
        [Required]
        [Display(Name = "CLASSIFICATION")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
    [MetadataType(typeof(Manager_TypeMeta))]
    public partial class Manager_Type
    {
    }
}