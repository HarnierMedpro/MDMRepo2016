using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Tools.IQuetable;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class Facitity_DBsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Read_PosIDs([DataSourceRequest] DataSourceRequest request, int? DB_ID)
        {
            List<VMZoomDB_POSID> availables = new List<VMZoomDB_POSID>();
            if (DB_ID != null && await db.DBLists.FindAsync(DB_ID) != null)
            {
                var DB = db.DBLists.Find(DB_ID).DB;
                var allPosIdOfThisDb = db.Facitity_DBs.Where(x => x.DB == DB).Select(z => new VMZoomDB_POSID{ZoomPos_ID = z.Facility_ID, ZoomPos_Name = z.Fac_NAME}).ToList();
                var aux = db.MasterPOS.Include(p => p.ZoomDB_POSID_grp).Where(x => x.DBList_DB_ID == DB_ID).Select(z => z.ZoomDB_POSID_grp).ToList();
                List<VMZoomDB_POSID> posTaken = new List<VMZoomDB_POSID>();
                foreach (var item in aux)
                {
                   var converted = item.Select(c => new VMZoomDB_POSID{ZoomPos_ID = c.ZoomPos_ID, ZoomPos_Name = db.Facitity_DBs.First(g => g.DB == DB && g.Facility_ID == c.ZoomPos_ID).Fac_NAME}).ToList(); 
                   posTaken.AddRange(converted);
                }
               availables = allPosIdOfThisDb.Except(posTaken).ToList();
            }
            return Json(availables.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Read_PosIdsOfThisDB(int? DB_ID)
        {
            List<VMZoomDB_POSID> availables = new List<VMZoomDB_POSID>();
            if (DB_ID != null && await db.DBLists.FindAsync(DB_ID) != null)
            {
                var allPosIdOfThisDb = db.Facitity_DBs.Where(x => x.DB_ID == DB_ID).Select(z => new VMZoomDB_POSID { ZoomPos_ID = z.Facility_ID, ZoomPos_Name = z.Fac_NAME }).ToList();
                var masterPosOfThisDb = db.MasterPOS.Include(p => p.ZoomDB_POSID_grp).Where(x => x.DBList_DB_ID == DB_ID).ToList();
                List<VMZoomDB_POSID> posTaken = new List<VMZoomDB_POSID>();
                foreach (var item in masterPosOfThisDb)
                {
                    var test = from a in item.ZoomDB_POSID_grp
                               join b in db.Facitity_DBs on a.ZoomPos_ID equals b.Facility_ID
                               where b.DB_ID == DB_ID
                               select new VMZoomDB_POSID { ZoomPos_ID = b.Facility_ID, ZoomPos_Name = b.Fac_NAME };
                    posTaken.AddRange(test.ToList());
                }

                availables = allPosIdOfThisDb.Except(posTaken, new POSIDsComparer()).ToList();
               //availables = db.Facitity_DBs.Where(x => x.DB_ID == DB_ID).Select(z => new VMZoomDB_POSID { ZoomPos_ID = z.Facility_ID, ZoomPos_Name = z.Fac_NAME }).ToList();
            }
            return Json(availables, JsonRequestBehavior.AllowGet);
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
