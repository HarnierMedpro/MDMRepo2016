using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class HR_requestMeta
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Manager { get; set; }
        [Required]
        public string Title { get; set; }
    }

    [MetadataTypeAttribute(typeof(HR_requestMeta))]
    public partial class HR_request
    {
    }
}