using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Data_Annotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMMasterPOS
    {
        public int MasterPOSID { get; set; }

        [Required]
        [Display(Name = "POS NAME")]
        public string PosMasterName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool active { get; set; }

        [Required]
        [Display(Name = "POS CLASSIFICATION")]
        [BiggerThanCero(ErrorMessage = "This field needs to be greater than zero.")]
        public int FvPList_FvPID { get; set; }

        [Required]
        [Display(Name = "ACCOUNT MANAGER")]
        [UIHint("ManagersDrpDownEditor")]
        [BiggerThanCero(ErrorMessage = "You need to choose a valid Manager.")]
        //public int Manager_Master_ManagerID { get; set; }
        public int ManagerID { get; set; }

        [Required]
        [Display(Name = "MEDICAL DIRECTOR")]
        [UIHint("MedDDrpDownEditor")]
        [BiggerThanCero(ErrorMessage = "You need to choose a valid Medical Director.")]
        public int ProvID { get; set; }

        [Required]
        [Display(Name = "DATABASE")]
        [UIHint("DatabaseDrpDownEditor")]
        [BiggerThanCero(ErrorMessage = "You need to choose a valid Database.")]
        public int DB_ID { get; set; } //Solo aquellas BD que ya estan asociadas a una corporacion

        [Display(Name = "LEVEL OF CARE")]
        [UIHint("LevOfCMultiselectEditor")]
        public IEnumerable<VMLevelOfCare> LevelOfCares { get; set; }

        [Display(Name = "SERVICES")]
        [UIHint("ServicesMultiselectEditor")]
        public IEnumerable<VMMPService> Services { get; set; }

        [Required]
        [Display(Name = "POS_IDs")]
        [UIHint("POSIDMultiselectEditor")]
        public IEnumerable<VMZoomDB_POSID> PosIDs { get; set; }

        [Display(Name = "FORMS_SENT")]
        [UIHint("FormsSentMultiselectEditor")]
        public IEnumerable<VMFormsDict> Forms_Sents { get; set; }

        [Display(Name = "CORPORATION")]
        public string Corporation { get; set; }

        public VMMasterPOS()
        {
            LevelOfCares = new List<VMLevelOfCare>();
            Services = new List<VMMPService>();
            PosIDs = new List<VMZoomDB_POSID>();
            Forms_Sents = new List<VMFormsDict>();
        }
    }
}