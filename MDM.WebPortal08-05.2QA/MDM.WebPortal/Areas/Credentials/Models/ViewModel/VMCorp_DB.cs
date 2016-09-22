namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMCorp_DB
    {
        public int ID_PK { get; set; }//PK

        public int corpID { get; set; } //FK from CorporateMasterList table

        public int DB_ID { get; set; } //FK from DBList table

        public bool active { get; set; } // DB's active property

        public string databaseName { get; set; } // DB's name property
    }
}