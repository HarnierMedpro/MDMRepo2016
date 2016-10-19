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
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.Manager_Type.AnyAsync(x => x.Name.Equals(manager_Type.Name, StringComparison.CurrentCultureIgnoreCase) && x.ManagerTypeID != manager_Type.ManagerTypeID))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                        }

                        var storedInDb = await db.Manager_Type.FindAsync(manager_Type.ManagerTypeID);

                        List<TableInfo> tableColumnInfos = new List<TableInfo>();

                        if (storedInDb.Name != manager_Type.Name)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "Name", NewValue = manager_Type.Name, OldValue = storedInDb.Name });
                            storedInDb.Name = manager_Type.Name;
                        }
                        if (storedInDb.Active != manager_Type.Active)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "Active", NewValue = manager_Type.Active.ToString(), OldValue = storedInDb.Active.ToString() });
                            storedInDb.Active = manager_Type.Active;
                        }

                        db.Manager_Type.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            AuditAction = "U",
                            tableInfos = tableColumnInfos,
                            AuditDateTime = DateTime.Now,
                            UserLogons = User.Identity.GetUserName(),
                            ModelPKey = storedInDb.ManagerTypeID,
                            TableName = "Manager_Type"
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);
                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_Type([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ManagerTypeID,Name,Active")] VMManager_Type manager_Type)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
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

                        AuditToStore auditLog = new AuditToStore
                        {
                            AuditAction = "I",
                            tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "Name", NewValue = storedInDb.Name },
                            new TableInfo{Field_ColumName = "Active", NewValue = storedInDb.Active.ToString() }
                        },
                            AuditDateTime = DateTime.Now,
                            UserLogons = User.Identity.GetUserName(),
                            ModelPKey = storedInDb.ManagerTypeID,
                            TableName = "Manager_Type"
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Somthing fail. Please try again!");
                        return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                    }
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
