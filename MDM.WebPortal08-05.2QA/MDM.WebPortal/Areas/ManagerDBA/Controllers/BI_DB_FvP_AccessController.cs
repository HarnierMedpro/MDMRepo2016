using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    [SetPermissions]
    public class BI_DB_FvP_AccessController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            ViewData["Manager"] = db.Manager_Master.Select(manager => new {manager.ManagerID, manager.AliasName});
            ViewData["DB"] = db.DBLists.Select(dB => new {dB.DB_ID, dB.DB});
            ViewData["FvP"] = db.FvPLists.Select(fvp => new {fvp.FvPID, fvp.FvPName});
            return View();
        }

        public ActionResult Read_GroupByManager([DataSourceRequest] DataSourceRequest request)
        {
            //var result = db.BI_DB_FvP_Access.Include(manager => manager.Manager_Master).Select(manager => manager.Manager_Master).Distinct();
            var result = db.Manager_Master;
            return Json(result.ToDataSourceResult(request, x => new VMManager_BI
            {
                ManagerID = x.ManagerID, //PK from Manager_Master table
                AliasName = x.AliasName,
                Active = x.Active, 
                Classification = x.Manager_Type.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_BI_DB_FvPByManager([DataSourceRequest] DataSourceRequest request, int? ManagerID)
        {
            var result =
                db.BI_DB_FvP_Access.Include(manager => manager.Manager_Master)
                    .Include(dB => dB.DBList)
                    .Include(fvp => fvp.FvPList);

            if (ManagerID != null)
            {
                result = result.Where(manager => manager.ManagerID == ManagerID);
            }

            return Json(result.ToDataSourceResult(request, x => new VMBI_DB_FvP
            {
                BIDbFvPID = x.BIDbFvPID, //PK
                ManagerID = x.ManagerID, //FK here shows the AliasName
                DB_ID = x.DB_ID, //FK here shows the DB#
                FvPID = x.FvPID, //FK here shows the FvPName
                Active = x.Active 
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_BI_DB_FvP([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "BIDbFvPID,ManagerID,DB_ID,FvPID,Active")] VMBI_DB_FvP bI_DB_FvP_Access)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.BI_DB_FvP_Access.AnyAsync(x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID && x.BIDbFvPID != bI_DB_FvP_Access.BIDbFvPID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb =  new BI_DB_FvP_Access
                    {
                        BIDbFvPID = bI_DB_FvP_Access.BIDbFvPID,
                        ManagerID = bI_DB_FvP_Access.ManagerID,
                        DB_ID = bI_DB_FvP_Access.DB_ID,
                        FvPID = bI_DB_FvP_Access.FvPID,
                        Active = bI_DB_FvP_Access.Active,
                    };
                    db.BI_DB_FvP_Access.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] {bI_DB_FvP_Access}.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_BI_DB_FvP([DataSourceRequest] DataSourceRequest request, [Bind(Include = "BIDbFvPID,ManagerID,DB_ID,FvPID,Active")] VMBI_DB_FvP bI_DB_FvP_Access, int ParentID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.BI_DB_FvP_Access.AnyAsync(x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                    }
                    var newOb = new BI_DB_FvP_Access
                    {
                        BIDbFvPID = bI_DB_FvP_Access.BIDbFvPID,
                        FvPID = bI_DB_FvP_Access.FvPID, DB_ID = bI_DB_FvP_Access.DB_ID, 
                        Active = bI_DB_FvP_Access.Active, 
                        ManagerID = ParentID
                    };
                    db.BI_DB_FvP_Access.Add(newOb);
                    await db.SaveChangesAsync();
                    bI_DB_FvP_Access.BIDbFvPID = newOb.BIDbFvPID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
        }

        //------------------------------------------------------------------ AUTOCOMPLETE ACTIONS: --------------------------------------------------------------------------------------\\
 
        public JsonResult GetManagers([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Manager_Master.ToDataSourceResult(request, x => new VMManager_BI
            {
                ManagerID = x.ManagerID, //PK from Manager_Master table
                AliasName = x.AliasName,
                Active = x.Active
            }), JsonRequestBehavior.AllowGet);  
        }

        public JsonResult GetDbs([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.DBLists.Select(dB => new { dB.DB_ID, dB.DB }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet); 
        }

        public JsonResult SearchDBs(string text)
        {
            var database = db.DBLists.Select(x => new 
            {
               x.DB_ID,
               x.DB
            });

            if (!string.IsNullOrEmpty(text))
            {
                database = database.Where(p => p.DB.Contains(text));
            }

            return Json(database, JsonRequestBehavior.AllowGet);
        }

        //-------------------------------------------------------------------- END AUTOCOMPLETE ACTIONS -----------------------------------------------------------------------------------\\

      

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
