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
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    [SetPermissions]
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
            ViewData["Type"] = db.Manager_Type.Select(x => new { x.ManagerTypeID, x.Name }).OrderBy(x => x.Name);
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
            if (ModelState.IsValid)
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

                    AuditToStore auditLog = new AuditToStore
                    {
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo { Field_ColumName = "ManagerTypeID", NewValue = toStore.ManagerTypeID.ToString() },
                            new TableInfo { Field_ColumName = "AliasName", NewValue = toStore.AliasName },
                            new TableInfo { Field_ColumName = "Active", NewValue = toStore.Active.ToString() },
                        },
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(), 
                        AuditAction = "I",
                        ModelPKey = toStore.ManagerID,
                        TableName = "Manager_Master"
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

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
                    if (await db.Manager_Master.AnyAsync(x => (x.ManagerTypeID == manager_Master.ManagerTypeID || x.AliasName.Equals(manager_Master.AliasName, StringComparison.CurrentCultureIgnoreCase)) && x.ManagerID != manager_Master.ManagerID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[]{new Manager_Master()}.ToDataSourceResult(request, ModelState));
                    }
                    //var storedInDB = new Manager_Master { ManagerID = manager_Master.ManagerID, AliasName = manager_Master.AliasName, Active = manager_Master.Active, ManagerTypeID = manager_Master.ManagerTypeID };
                    var storedInDB = await db.Manager_Master.FindAsync(manager_Master.ManagerID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDB.AliasName != manager_Master.AliasName)
                    {
                       
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "AliasName", NewValue= manager_Master.AliasName, OldValue = storedInDB.AliasName });
                        storedInDB.AliasName = manager_Master.AliasName;
                    }
                    if (storedInDB.Active != manager_Master.Active)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Active", NewValue = manager_Master.Active.ToString(), OldValue = storedInDB.Active.ToString() });
                        storedInDB.Active = manager_Master.Active;
                    }
                    if (storedInDB.ManagerTypeID != manager_Master.ManagerTypeID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ManagerTypeID", NewValue = manager_Master.ManagerTypeID.ToString(), OldValue = storedInDB.ManagerTypeID.ToString() });
                        storedInDB.AliasName = manager_Master.AliasName;
                    }

                    db.Manager_Master.Attach(storedInDB);
                    db.Entry(storedInDB).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "U",
                        tableInfos = tableColumnInfos, 
                        ModelPKey = storedInDB.ManagerID,
                        TableName = "Manager_Master"
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

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
