using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMMasterPOS_LevOfCare
    {
        public int MasterPosLocID { get; set; }

        [Display(Name = "LEVEL OF CARE")]
        public int Lev_of_Care_LevOfCareID { get; set; }

        [Display(Name = "POS")]
        public int MasterPOS_MasterPOSID { get; set; }
    }
}