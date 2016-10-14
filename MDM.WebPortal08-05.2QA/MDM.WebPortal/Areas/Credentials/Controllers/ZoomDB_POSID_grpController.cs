using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
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
            var aux = from m in db.MasterPOS
                join dbl in db.DBLists on m.DBList_DB_ID equals dbl.DB_ID
                join grp in result on m.MasterPOSID equals grp.MasterPOSID
                join fac in db.Facitity_DBs on grp.ZoomPos_ID equals fac.Facility_ID 
                where dbl.DB_ID == fac.DB_ID
                select new VMPosIds
                {
                    ZoomDBPOSID = grp.ZoomDBPOSID, //PK
                    ZoomPos_ID = grp.ZoomPos_ID,
                    Active = grp.Active,
                    MasterPOSID = grp.MasterPOSID,
                    Extra = grp.Extra,
                    ZoomPos_Name = fac.Fac_NAME
                };
            return Json(aux.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
            
            //return Json(result.ToDataSourceResult(request, x => new VMPosIds
            //{
            //    ZoomDBPOSID = x.ZoomDBPOSID, //PK
            //    ZoomPos_ID = x.ZoomPos_ID,
            //    Active = x.Active,
            //    MasterPOSID = x.MasterPOSID,
            //    Extra = x.Extra
            //}), JsonRequestBehavior.AllowGet);
           
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
