using System;
using System.Collections.Generic;
using System.Data;
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

        //public ActionResult Index()
        //{
        //    var dbsTaken = db.Corp_DBs.Select(x => x.DBList);
        //    var dbsNoTaken = db.DBLists.Except(dbsTaken).Select(x => new { x.DB_ID, x.DB }).ToList();
        //    /*If use dbsNoTaken excluyo la BD del elemento por lo que no se muestra*/
        //    //ViewData["DBs"] = dbsNoTaken;
        //    ViewData["DBs"] = db.DBLists.Select(x => new { x.DB_ID, x.DB }).ToList();
        //    return View();
        //}

        //public ActionResult Details()
        //{
        //    ViewData["DBs"] = db.DBLists.Select(x => new { x.DB_ID, x.DB }).ToList();
        //    return View();
        //}


     

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
            if (ModelState.IsValid && ParentID > 0)
            {
                var currentCorp = await db.CorporateMasterLists.FindAsync(ParentID);
                var currentDB = await db.DBLists.FindAsync(corp_DBs.DB_ID);
                corp_DBs.corpID = ParentID;

                if (currentCorp == null || currentDB == null)
                {
                    ModelState.AddModelError("", "Duplicate data. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }
                corp_DBs.databaseName = currentDB.databaseName;
                corp_DBs.active = currentDB.active;

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
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
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
                    ModelState.AddModelError("", "Duplicate data. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }
                corp_DBs.databaseName = currentDB.databaseName;
                corp_DBs.active = currentDB.active;

                try
                {
                    if (await db.Corp_DBs.AnyAsync(x => x.DB_ID == corp_DBs.DB_ID && x.ID_PK != corp_DBs.ID_PK))
                    {
                        ModelState.AddModelError("", "Duplicate data. Please try again!");
                        return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                    }
                    //var storedInDb = new Corp_DBs { ID_PK = corp_DBs.ID_PK, DB_ID = corp_DBs.DB_ID, corpID = corp_DBs.corpID};
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
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));

                }
            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
        }


        public async Task<ActionResult> Corp_DBs_Release([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ID_PK,corpID,DB_ID")] VMCorp_DB corp_DBs)
        {
            if (ModelState.IsValid)
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
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }

            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
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
