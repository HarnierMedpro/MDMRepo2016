using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMMasterPOS_MPServ
    {
        public int MasterPosMPServID { get; set; }

        [Display(Name = "SERVICE")]
        public int MPServices_MPServID { get; set; }

        [Display(Name = "LOCATION POS")]
        public int MasterPOS_MasterPOSID { get; set; }
    }
}