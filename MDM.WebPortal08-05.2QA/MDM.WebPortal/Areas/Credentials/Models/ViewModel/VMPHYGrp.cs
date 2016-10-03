using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMPHYGrp
    {
        public int MasterPOSID { get; set; }

        public int PHYGrpID { get; set; }

        [Required]
        [Display(Name = "PHYSICIAN GROUP NAME")]
        public string PHYGroupName { get; set; }

        [Required]
        [Display(Name = "PHYSICIAN GROUP NPI NUMBER")]
        public string PHYGrpNPI_Num { get; set; }

        [Required(ErrorMessage = "You have to select at least one Physician. Please try again!")]
        [Display(Name = "PHYSICIANS")]
        [UIHint("ProvidersEditor")]
        public IEnumerable<VMProvider> Physicians { get; set; }

        public VMPHYGrp()
        {
            Physicians = new List<VMProvider>();
        }
    }
}