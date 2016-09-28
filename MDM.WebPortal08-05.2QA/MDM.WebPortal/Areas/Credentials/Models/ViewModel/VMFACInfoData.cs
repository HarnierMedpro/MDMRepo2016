using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMFACInfoData
    {
        public int MasterPOSID { get; set; } //PK from dbo.MasterPOS

        public int InfoDataID { get; set; }

        //[Required]
        //[Display(Name = "MEDICAL DIRECTOR NAME")]
        //[RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Alias Name.")]
        //[StringLength(50)]
        //public string DocProviderName { get; set; }

        [Display(Name = "LICENSE TYPE")]
        public string LicType { get; set; }

        [Display(Name = "LICENSE STATE")]
        [StringLength(2, MinimumLength = 2)]
        public string StateLic { get; set; }

        [Display(Name = "CLIA NUMBER")]
        public string LicNumCLIA_waiver { get; set; }

        [Display(Name = "LICENSE EFFECTIVE DATE")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime LicEffectiveDate { get; set; }

        [Display(Name = "LICENSE EXPIRE DATE")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime LicExpireDate { get; set; }

        [Display(Name = "TAXONOMY")]
        public string Taxonomy { get; set; }

        //[Display(Name = "NPI NUMBER")]
        //[StringLength(10, MinimumLength = 10)]
        //public string FAC_NPI_Num { get; set; }
    }
}