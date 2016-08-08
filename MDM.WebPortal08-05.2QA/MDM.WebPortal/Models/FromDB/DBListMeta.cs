using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class DBListMeta
    {
       
        [Required]
        [StringLength(3, ErrorMessage = "This field must have a maximum length of 3.")]
        public string DB { get; set; }

        [Display(Name="NAME")]
        [StringLength(75)]
        public string databaseName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public Nullable<bool> active { get; set; }
    }
    [MetadataTypeAttribute(typeof(DBListMeta))]
    public partial class DBList
    {
    }
}