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
using MDM.WebPortal.Areas.ActionCode.Models.ViewModels;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.ActionCode.Controllers
{
    [SetPermissions]
    public class ACtypesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_ACType([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.ACtypes.OrderBy(x => x.ACTypeName).ToDataSourceResult(request, x => new VMACType
            {
                ACTypeID = x.ACTypeID,
                ACTypeName = x.ACTypeName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ACType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ACTypeID,ACTypeName")] VMACType aCtype)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.ACtypes.AnyAsync(x => x.ACTypeName.Equals(aCtype.ACTypeName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                        }
                        var toStore = new ACtype
                        {
                            ACTypeName = aCtype.ACTypeName
                        };
                        db.ACtypes.Add(toStore);
                        await db.SaveChangesAsync();
                        aCtype.ACTypeID = toStore.ACTypeID;

                        AuditToStore auditLog = new AuditToStore
                        {
                            AuditAction = "I",
                            TableName = "ACType",
                            AuditDateTime = DateTime.Now,
                            UserLogons = User.Identity.GetUserName(),
                            ModelPKey = toStore.ACTypeID,
                            tableInfos = new List<TableInfo>
                        {
                            new TableInfo { Field_ColumName = "ACTypeName", NewValue = toStore.ACTypeName }
                        }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ACType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ACTypeID,ACTypeName")] VMACType aCtype)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.ACtypes.AnyAsync(x => x.ACTypeName.Equals(aCtype.ACTypeName, StringComparison.CurrentCultureIgnoreCase) && x.ACTypeID != aCtype.ACTypeID))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                        }
                        var toStore = await db.ACtypes.FindAsync(aCtype.ACTypeID);
                        List<TableInfo> tableColumnInfos = new List<TableInfo>
                        {
                            new TableInfo
                            {
                                Field_ColumName = "ACTypeName",
                                NewValue = aCtype.ACTypeName,
                                OldValue = toStore.ACTypeName
                            }
                        };
                        toStore.ACTypeName = aCtype.ACTypeName;

                        db.ACtypes.Attach(toStore);
                        db.Entry(toStore).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            AuditAction = "U",
                            TableName = "ACType",
                            AuditDateTime = DateTime.Now,
                            UserLogons = User.Identity.GetUserName(),
                            ModelPKey = toStore.ACTypeID,
                            tableInfos = tableColumnInfos
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                    }
                }
                
            }
            return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
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
