//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
//using MDM.WebPortal.Models.FromDB;
//using MDM.WebPortal.Models.ViewModel;

//namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
//{
//    public class Facitity_DBsController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();

       

//        public ActionResult Index()
//        {
//            ViewData["DBs"] = db.DBLists.OrderBy(x => x.DB).Select(x => new VMDBList{DB = x.DB, DB_ID = x.DB_ID, active = x.active, databaseName = x.databaseName});
//            return View();
//        }

//        public ActionResult Read_POSDBs([DataSourceRequest] DataSourceRequest request)
//        {
//            return Json(db.Facitity_DBs.OrderBy(x => x.DB).ToDataSourceResult(request, fac => new VMFacility_DBs
//            {
//                IDPK = fac.IDPK, //PK
//                DB = fac.DB, //Info from DBList
//                DatabaseName = fac.DatabaseName, //Info from DBList
//                Facility_ID = fac.Facility_ID.Value, //Siempre va a haber un valor pues el VM define este campo como required
//                Fac_NAME = fac.Fac_NAME,
//                Active = fac.Active
//            }), JsonRequestBehavior.AllowGet);
//        }


//        public async Task<ActionResult> Create_POSDBs([DataSourceRequest] DataSourceRequest request,
//             [Bind(Include = "IDPK,DB,Facility_ID,Fac_NAME,Active")] VMFacility_DBs modelToStore)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (await db.Facitity_DBs.AnyAsync(x => x.DB == modelToStore.DB && x.Facility_ID == modelToStore.Facility_ID))
//                    {
//                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
//                        return Json(new[] { modelToStore }.ToDataSourceResult(request, ModelState));
//                    }

                   
//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something failed. Please try again!");
//                    return Json(new[] { modelToStore }.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { modelToStore }.ToDataSourceResult(request, ModelState));
//        }

//        public async Task<ActionResult> Update_FacilityDBs([DataSourceRequest] DataSourceRequest request,
//            [Bind(Include = "IDPK,DB,Facility_ID,Fac_NAME,Active")] VMFacility_DBs modelToStore)
//        {
//            return Json(new[] {modelToStore}.ToDataSourceResult(request, ModelState));
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
