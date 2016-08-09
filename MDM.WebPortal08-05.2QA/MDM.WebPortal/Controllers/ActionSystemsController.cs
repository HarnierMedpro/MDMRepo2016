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
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers
{
    [AllowAnonymous]
    public class ActionSystemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewData["Controllers"] = db.Controllers.Select(x => new VMControllerSystem { ControllerID = x.ControllerID, Cont_Name = x.Cont_Name });
            return View();
        }

        public ActionResult Read_ActionSystems([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Actions.ToDataSourceResult(request, x => new VMActionSystem
            {
                ActionID = x.ActionID,
                Act_Name = x.Act_Name,
                ControllerID = x.ControllerID
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ActionSystems([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ActionID,Act_Name,ControllerID")] VMActionSystem actionSystem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Actions.AnyAsync(x => x.Act_Name == actionSystem.Act_Name && x.ControllerID == actionSystem.ControllerID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { actionSystem }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new ActionSystem 
                    {                       
                        Act_Name = actionSystem.Act_Name, 
                        ControllerID = actionSystem.ControllerID 
                    };
                    db.Actions.Add(toStore);
                    await db.SaveChangesAsync();
                    actionSystem.ActionID = toStore.ActionID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { actionSystem }.ToDataSourceResult(request, ModelState));
                }
            }          
            return Json(new[] { actionSystem }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ActionSystems([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ActionID,Act_Name,ControllerID")] VMActionSystem actionSystem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Actions.AnyAsync(x => x.Act_Name == actionSystem.Act_Name && x.ControllerID == actionSystem.ControllerID && x.ActionID != actionSystem.ActionID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { actionSystem }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = new ActionSystem 
                    { 
                        ActionID = actionSystem.ActionID, 
                        Act_Name = actionSystem.Act_Name, 
                        ControllerID = actionSystem.ControllerID
                    };
                    db.Actions.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();                   
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { actionSystem }.ToDataSourceResult(request, ModelState));
                }
            }         
            return Json(new[] { actionSystem }.ToDataSourceResult(request, ModelState));
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
