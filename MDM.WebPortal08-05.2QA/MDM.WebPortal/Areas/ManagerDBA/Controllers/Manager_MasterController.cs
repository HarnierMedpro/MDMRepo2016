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
using MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    //[SetPermissions]
    public class Manager_MasterController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Test()
        {
            var manager_Master = db.Manager_Master.Include(m => m.Manager_Type);
            return View( manager_Master.Select(x => new VMManager_Master
            {
                ManagerID = x.ManagerID,
                AliasName = x.AliasName,
                Active = x.Active
            }).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Test(string ManagerID)
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewData["Type"] = db.Manager_Type.Select(x => new { x.ManagerTypeID, x.Name });
            return View();
        }

        public ActionResult Read_Manager([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Manager_Master.ToDataSourceResult(request, m => new VMManager_Master
            {
                ManagerID = m.ManagerID, //PK
                AliasName = m.AliasName,
                Active = m.Active,
                ManagerTypeID = m.ManagerTypeID //FK from dbo.Manager_Type table
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Manager([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ManagerID,ManagerTypeID,AliasName,Active")] VMManager_Master manager_Master)
        {
            if (manager_Master != null && ModelState.IsValid)
            {
                try
                {
                    if (await db.Manager_Master.AnyAsync(x => x.ManagerTypeID == manager_Master.ManagerTypeID || x.AliasName.Equals(manager_Master.AliasName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new Manager_Master
                    {
                        ManagerTypeID = manager_Master.ManagerTypeID,
                        AliasName = manager_Master.AliasName,
                        Active = manager_Master.Active
                    };
                    db.Manager_Master.Add(toStore);
                    await db.SaveChangesAsync();
                    manager_Master.ManagerID = toStore.ManagerID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Manager([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "ManagerID,ManagerTypeID,AliasName,Active")] VMManager_Master manager_Master)
        {
            if (ModelState.IsValid)
            {
                
                try
                {
                    if (await db.Manager_Master.AnyAsync(x => x.ManagerTypeID == manager_Master.ManagerTypeID || x.AliasName.Equals(manager_Master.AliasName, StringComparison.CurrentCultureIgnoreCase) && x.ManagerID != manager_Master.ManagerID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[]{new Manager_Master()}.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDB = new Manager_Master { ManagerID = manager_Master.ManagerID, AliasName = manager_Master.AliasName, Active = manager_Master.Active, ManagerTypeID = manager_Master.ManagerTypeID };
                    db.Manager_Master.Attach(storedInDB);
                    db.Entry(storedInDB).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
        }

       

        public async Task<ActionResult> GetAvailablesManager_Type([DataSourceRequest] DataSourceRequest request, int? ManagerTypeID)
        {
            var result = new List<Manager_Type>();
            if (ManagerTypeID != null && await db.Manager_Type.FindAsync(ManagerTypeID) != null)
            {
                result.Add(await db.Manager_Type.FindAsync(ManagerTypeID));
            }
            var storedInDb = db.Manager_Master.Include(x => x.Manager_Type).Select(x => x.Manager_Type);
            var availables = db.Manager_Type.Except(storedInDb);

            result.AddRange(availables);

            return Json(result.ToDataSourceResult(request, x => new VMManager_Type
            {
                ManagerTypeID = x.ManagerTypeID,
                Name = x.Name,
                Active = x.Active
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
