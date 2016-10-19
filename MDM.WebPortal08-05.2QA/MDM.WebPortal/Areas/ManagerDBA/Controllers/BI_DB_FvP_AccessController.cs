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
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    [SetPermissions]
    public class BI_DB_FvP_AccessController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            ViewData["Type"] = db.Manager_Type.Select(x => new { x.ManagerTypeID, x.Name }).OrderBy(x => x.Name);
            //ViewData["Manager"] = db.Manager_Master.Select(manager => new {manager.ManagerID, manager.AliasName});
            ViewData["DB"] = db.DBLists.Select(dB => new {dB.DB_ID, dB.DB});
            ViewData["FvP"] = db.FvPLists.Select(fvp => new {fvp.FvPID, fvp.FvPName});
            return View();
        }

        public ActionResult Read_GroupByManager([DataSourceRequest] DataSourceRequest request)
        {
            //var result = db.BI_DB_FvP_Access.Include(manager => manager.Manager_Master).Select(manager => manager.Manager_Master).Distinct();
            var result = db.Manager_Master.Include(x => x.Manager_Type).OrderBy(x => x.AliasName).ToList();
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
            var result = db.BI_DB_FvP_Access.Include(manager => manager.Manager_Master)
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
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.BI_DB_FvP_Access.AnyAsync(x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID && x.BIDbFvPID != bI_DB_FvP_Access.BIDbFvPID))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                        }

                        var storedInDb = await db.BI_DB_FvP_Access.FindAsync(bI_DB_FvP_Access.BIDbFvPID);

                        List<TableInfo> tableColumnInfos = new List<TableInfo>();

                        if (storedInDb.ManagerID != bI_DB_FvP_Access.ManagerID)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "ManagerID", NewValue = bI_DB_FvP_Access.ManagerID.ToString(), OldValue = storedInDb.ManagerID.ToString() });
                            storedInDb.ManagerID = bI_DB_FvP_Access.ManagerID;
                        }
                        if (storedInDb.DB_ID != bI_DB_FvP_Access.DB_ID)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "DB_ID", NewValue = bI_DB_FvP_Access.DB_ID.ToString(), OldValue = storedInDb.DB_ID.ToString() });
                            storedInDb.DB_ID = bI_DB_FvP_Access.DB_ID;
                        }
                        if (storedInDb.FvPID != bI_DB_FvP_Access.FvPID)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "FvPID", NewValue = bI_DB_FvP_Access.FvPID.ToString(), OldValue = storedInDb.FvPID.ToString() });
                            storedInDb.FvPID = bI_DB_FvP_Access.FvPID;
                        }
                        if (storedInDb.Active != bI_DB_FvP_Access.Active)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "Active", NewValue = bI_DB_FvP_Access.Active.ToString(), OldValue = storedInDb.Active.ToString() });
                            storedInDb.Active = bI_DB_FvP_Access.Active;
                        }

                        db.BI_DB_FvP_Access.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditAction = "U",
                            tableInfos = tableColumnInfos,
                            AuditDateTime = DateTime.Now,
                            ModelPKey = storedInDb.BIDbFvPID,
                            TableName = "BI_DB_Fvp_Access"
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);
                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] {bI_DB_FvP_Access}.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_BI_DB_FvP([DataSourceRequest] DataSourceRequest request, [Bind(Include = "BIDbFvPID,ManagerID,DB_ID,FvPID,Active")] VMBI_DB_FvP bI_DB_FvP_Access, int ParentID)
        {
            if (ModelState.IsValid && ParentID > 0)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
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
                            FvPID = bI_DB_FvP_Access.FvPID,
                            DB_ID = bI_DB_FvP_Access.DB_ID,
                            Active = bI_DB_FvP_Access.Active,
                            ManagerID = ParentID
                        };
                        db.BI_DB_FvP_Access.Add(newOb);
                        await db.SaveChangesAsync();
                        bI_DB_FvP_Access.BIDbFvPID = newOb.BIDbFvPID;

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditAction = "U",
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "FvPID", NewValue = newOb.FvPID.ToString()},
                                new TableInfo{Field_ColumName = "DB_ID", NewValue = newOb.DB_ID.ToString()},
                                new TableInfo{Field_ColumName = "Active", NewValue = newOb.Active.ToString()},
                                new TableInfo{Field_ColumName = "ManagerID", NewValue = newOb.ManagerID.ToString()},
                            },
                            AuditDateTime = DateTime.Now,
                            ModelPKey = newOb.BIDbFvPID,
                            TableName = "BI_DB_Fvp_Access"
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                    }
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
