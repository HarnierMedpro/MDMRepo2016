using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMPosIds
    {
        public int ZoomDBPOSID { get; set; } //PK

        [Display(Name = "POS ID")]
        public long ZoomPos_ID { get; set; } //POSId

        [Display(Name = "EXTRA")]
        public string Extra { get; set; }

        [Display(Name = "POS NAME")]
        public int MasterPOSID { get; set; }

        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        [Display(Name = "ZOOM POS_ID NAME")]
        public string ZoomPos_Name { get; set; }
    }
}