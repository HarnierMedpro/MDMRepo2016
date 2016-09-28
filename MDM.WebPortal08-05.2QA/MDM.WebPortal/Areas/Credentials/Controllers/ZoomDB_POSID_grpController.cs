using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class ZoomDB_POSID_grpController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Read_PosIds([DataSourceRequest] DataSourceRequest request, int? masterPOSID)
        {
            var result = db.ZoomDB_POSID_grp.Include(x => x.MasterPOS);
            if (masterPOSID != null && masterPOSID > 0)
            {
                result = result.Where(x => x.MasterPOSID == masterPOSID);
            }
            return Json(result.ToDataSourceResult(request, x => new VMPosIds
            {
                ZoomDBPOSID = x.ZoomDBPOSID, //PK
                ZoomPos_ID = x.ZoomPos_ID,
                Active = x.Active,
                MasterPOSID = x.MasterPOSID,
                Extra = x.Extra
            }), JsonRequestBehavior.AllowGet);
           
        }
      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
