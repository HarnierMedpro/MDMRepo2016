using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Models.FromDB;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class Corp_DBsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult HierarchyBinding_DBs(int? corpID, [DataSourceRequest] DataSourceRequest request)
        {
            var result = db.Corp_DBs.Include(x => x.CorporateMasterList).Include(x => x.DBList).Select(x => new VMCorp_DB
            {
                ID_PK = x.ID_PK, //PK from Corp_DBs table
                DB_ID = x.DB_ID, //FK from DBList table
                corpID = x.corpID, //FK from CorporateMasterList table
                databaseName = x.DBList.databaseName,
                active = x.DBList.active
            });

            if (corpID != null)
            {
                result = result.Where(x => x.corpID == corpID);
            }

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_CorpDbs([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ID_PK,corpID,DB_ID")] VMCorp_DB corp_DBs, int ParentID)
        {
            if (ModelState.IsValid)
            {
                if (ParentID == 0)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }
                corp_DBs.corpID = ParentID;
                var currentCorp = await db.CorporateMasterLists.FindAsync(ParentID);
                var currentDb = await db.DBLists.FindAsync(corp_DBs.DB_ID);

                if (currentCorp == null || currentDb == null)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }
                corp_DBs.databaseName = currentDb.databaseName;
                corp_DBs.active = currentDb.active;

                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.Corp_DBs.AnyAsync(x => x.DB_ID == corp_DBs.DB_ID))
                        {
                            ModelState.AddModelError("", "Duplicate data. Please try again!");
                            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                        }
                        var toStore = new Corp_DBs
                        {
                            DB_ID = corp_DBs.DB_ID,
                            corpID = ParentID
                        };

                        db.Corp_DBs.Add(toStore);
                        await db.SaveChangesAsync();
                        corp_DBs.ID_PK = toStore.ID_PK;

                        /*--------------- AUDIT LOG SCENARIO ----------------*/
                        AuditToStore auditLog = new AuditToStore
                        {
                            ModelPKey = corp_DBs.ID_PK,
                            TableName = "Corp_DBs",
                            AuditAction = "I",
                            AuditDateTime = DateTime.Now,
                            UserLogons = User.Identity.GetUserName(),
                            tableInfos = new List<TableInfo>
                        {
                            new TableInfo { NewValue = corp_DBs.DB_ID.ToString(), Field_ColumName = "DB_ID" }, 
                            new TableInfo { NewValue = corp_DBs.corpID.ToString(), Field_ColumName = "corpID" }
                        }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                        /*--------------- AUDIT LOG SCENARIO ----------------*/

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_CorpDbs([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ID_PK,corpID,DB_ID")] VMCorp_DB corp_DBs)
        {
            if (ModelState.IsValid)
            {
                var currentCorp = await db.CorporateMasterLists.FindAsync(corp_DBs.corpID);
                var currentDB = await db.DBLists.FindAsync(corp_DBs.DB_ID);
                if (currentCorp == null || currentDB == null)
                {
                    ModelState.AddModelError("", "Something Failed. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }
                corp_DBs.databaseName = currentDB.databaseName;
                corp_DBs.active = currentDB.active;

                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.Corp_DBs.AnyAsync(x => x.DB_ID == corp_DBs.DB_ID && x.ID_PK != corp_DBs.ID_PK))
                        {
                            ModelState.AddModelError("", "Duplicate data. Please try again!");
                            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                        }

                        var storedInDb = await db.Corp_DBs.FindAsync(corp_DBs.ID_PK);

                        List<TableInfo> tableColumnInfos = new List<TableInfo>();
                        if (storedInDb.DB_ID != corp_DBs.DB_ID)
                        {
                            tableColumnInfos.Add(new TableInfo
                            {
                                NewValue = corp_DBs.DB_ID.ToString(),
                                OldValue = storedInDb.DB_ID.ToString(),
                                Field_ColumName = "DB_ID"
                            });
                            storedInDb.DB_ID = corp_DBs.DB_ID;
                        }
                        if (storedInDb.corpID != corp_DBs.corpID)
                        {
                            tableColumnInfos.Add(new TableInfo
                            {
                                NewValue = corp_DBs.corpID.ToString(),
                                OldValue = storedInDb.corpID.ToString(),
                                Field_ColumName = "corpID"
                            });
                            storedInDb.DB_ID = corp_DBs.DB_ID;
                        }

                        db.Corp_DBs.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            ModelPKey = corp_DBs.ID_PK,
                            TableName = "Corp_DBs",
                            AuditAction = "U",
                            AuditDateTime = DateTime.Now,
                            UserLogons = User.Identity.GetUserName(),
                            tableInfos = tableColumnInfos
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));

                    }
                }
                
            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
        }


        public async Task<ActionResult> Corp_DBs_Release([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ID_PK,corpID,DB_ID")] VMCorp_DB corp_DBs)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var toRelease = new Corp_DBs { ID_PK = corp_DBs.ID_PK, corpID = corp_DBs.corpID, DB_ID = corp_DBs.DB_ID };
                        db.Corp_DBs.Attach(toRelease);
                        db.Corp_DBs.Remove(toRelease);
                        await db.SaveChangesAsync();

                        /*------------------ AUDIT LOG SCENARIO ----------------------*/
                        AuditToStore auditLog = new AuditToStore
                        {
                            AuditAction = "D",
                            AuditDateTime = DateTime.Now,
                            ModelPKey = toRelease.ID_PK,
                            TableName = "Corp_DBs",
                            UserLogons = User.Identity.GetUserName(),
                            tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{OldValue = toRelease.DB_ID.ToString(), Field_ColumName = "DB_ID"},
                                    new TableInfo{OldValue = toRelease.corpID.ToString(), Field_ColumName = "corpID"},
                                }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                        /*------------------ AUDIT LOG SCENARIO ----------------------*/

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                    }
                }

            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_DbsWithCorp([DataSourceRequest] DataSourceRequest request)
        {
           var result = db.Corp_DBs.Include(c => c.CorporateMasterList).Include(d => d.DBList).Select(d => d.DBList).Distinct();
            return Json(result.ToDataSourceResult(request, x => new VMDBList
            {
                DB_ID = x.DB_ID,
                DB = x.DB,
                databaseName = x.databaseName,
                active = x.active
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
