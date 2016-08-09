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
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    public class Manager_TypeController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Type([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Manager_Type.ToDataSourceResult(request, t => new VMManager_Type
            {
                ManagerTypeID = t.ManagerTypeID,
                Name = t.Name,
                Active = t.Active
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_Type([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ManagerTypeID,Name,Active")] VMManager_Type manager_Type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Manager_Type.AnyAsync(x => x.Name.Equals(manager_Type.Name, StringComparison.CurrentCultureIgnoreCase) && x.ManagerTypeID != manager_Type.ManagerTypeID))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {manager_Type}.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = new Manager_Type{ ManagerTypeID = manager_Type.ManagerTypeID, Name = manager_Type.Name, Active = manager_Type.Active};
                    db.Manager_Type.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_Type([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ManagerTypeID,Name,Active")] VMManager_Type manager_Type)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Manager_Type.AnyAsync(x => x.Name.Equals(manager_Type.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = new Manager_Type { Name = manager_Type.Name, Active = manager_Type.Active };
                    db.Manager_Type.Add(storedInDb);
                    await db.SaveChangesAsync();
                    manager_Type.ManagerTypeID = storedInDb.ManagerTypeID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Somthing fail. Please try again!");
                    return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
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
