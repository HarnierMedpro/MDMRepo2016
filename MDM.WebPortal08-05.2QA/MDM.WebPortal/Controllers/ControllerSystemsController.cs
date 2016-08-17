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
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers
{
//    [SetPermissions]
    [AllowAnonymous]
    public class ControllerSystemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewData["Area"] = db.Areas.OrderBy(x => x.AreaName).Select(x => new VMAreaSystems {AreaID = x.AreaID, AreaName = x.AreaName});
            return View();
        }

        public ActionResult Read_ControllerSystem([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Controllers.ToDataSourceResult(request, x => new VMControllerSystem
            {
                ControllerID = x.ControllerID,
                Cont_Name = x.Cont_Name,
                AreaID = x.AreaID
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ControllerSystem([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ControllerID,Cont_Name,AreaID")] VMControllerSystem controllerSystem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Controllers.AnyAsync(x => x.Cont_Name == controllerSystem.Cont_Name))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { controllerSystem }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new ControllerSystem 
                    {                       
                        Cont_Name = controllerSystem.Cont_Name,
                        AreaID = controllerSystem.AreaID 
                    };
                    db.Controllers.Add(toStore);
                    await db.SaveChangesAsync();
                    controllerSystem.ControllerID = toStore.ControllerID;                    
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again.");
                    return Json(new[] { controllerSystem }.ToDataSourceResult(request, ModelState));
                }
            }           
            return Json(new[] { controllerSystem }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ControllerSystem([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ControllerID,Cont_Name")] VMControllerSystem controllerSystem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Controllers.AnyAsync(x => x.Cont_Name == controllerSystem.Cont_Name && x.ControllerID != controllerSystem.ControllerID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { controllerSystem }.ToDataSourceResult(request, ModelState));
                    }
                    //var storedInDb = await db.Controllers.FindAsync(controllerSystem.ControllerID);
                    //storedInDb.Cont_Name = controllerSystem.Cont_Name;
                    //db.Entry(storedInDb).State = EntityState.Modified;
                    //await db.SaveChangesAsync();
                    //return Json(new[] { controllerSystem }.ToDataSourceResult(request));
                    var storeInDb = new ControllerSystem
                    {
                        ControllerID = controllerSystem.ControllerID,
                        Cont_Name = controllerSystem.Cont_Name
                    };

                    db.Controllers.Attach(storeInDb);
                    db.Entry(storeInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again.");
                    return Json(new[] { controllerSystem }.ToDataSourceResult(request, ModelState));
                }
            }           
            return Json(new[] { controllerSystem }.ToDataSourceResult(request, ModelState));
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
