using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.DAL;
using MDM.WebPortal.Models.ViewModel;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace MDM.WebPortal.Controllers.APP
{
    public class DBListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       
        //With Kendo UI for ASP.NET MVC
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(db.DBLists.ToDataSourceResult(request, x => new VMDBList
            {
                DB_ID = x.DB_ID,
                DB = x.DB,
                databaseName = x.databaseName,
                active = x.active
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest]DataSourceRequest request, 
            [Bind(Include = "DB_ID,DB,databaseName, active")] VMDBList dBList)
        {
            if (dBList != null && ModelState.IsValid)
            {              
               try
               {
                   if (await db.DBLists.AnyAsync(x => x.DB == dBList.DB && x.DB_ID != dBList.DB_ID))
                   {
                       ModelState.AddModelError("", "Duplicate Database. Please try again!");
                       return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
                   }
                   var storedInDb = new DBList { DB_ID = dBList.DB_ID, DB = dBList.DB, databaseName = dBList.databaseName, active = dBList.active };
                   db.DBLists.Attach(storedInDb);
                   db.Entry(storedInDb).State = EntityState.Modified;
                   await db.SaveChangesAsync();                   
               }
               catch (Exception)
               {
                   ModelState.AddModelError("", "Something Failed. Please try again!");
                   return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
               }
            }          
            return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
           
        }

        public async Task<ActionResult> Create_DBs([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "DB_ID,DB,databaseName, active")] VMDBList dBList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.DBLists.AnyAsync(x => x.DB == dBList.DB))
                    {
                        ModelState.AddModelError("", "Duplicate Database. Please try again!");
                        return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new DBList {DB = dBList.DB, databaseName = dBList.databaseName, active = dBList.active };
                    db.DBLists.Add(toStore);
                    await db.SaveChangesAsync();
                    dBList.DB_ID = toStore.DB_ID;                    
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
                }
                
            }         
            return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
        }
        
        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckDB(string term)
        {
            if (!String.IsNullOrEmpty(term))
            {
                _DALDBList DBs = new _DALDBList();
                var resultSet = DBs.BuscarDB(term.ToLower());
                List<VMDB> result = new List<VMDB>();
                foreach (DataRow row in resultSet.Rows)
                {
                    result.Add(new VMDB
                    {
                        DB = row.ItemArray[0].ToString()
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Error", new { area = "Error" });
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
