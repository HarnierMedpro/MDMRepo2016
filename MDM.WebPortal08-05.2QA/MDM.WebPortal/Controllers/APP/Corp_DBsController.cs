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
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Controllers.APP
{
    //[SetPermissions(Permissions = "Index,Read,HierarchyBinding_DBs")]
    [SetPermissions]
    public class Corp_DBsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            var dbsTaken = db.Corp_DBs.Select(x => x.DBList);
            var dbsNoTaken = db.DBLists.Except(dbsTaken).Select(x => new{x.DB_ID, x.DB}).ToList(); 
            /*If use dbsNoTaken excluyo la BD del elemento por lo que no se muestra*/
            //ViewData["DBs"] = dbsNoTaken;
            ViewData["DBs"] = db.DBLists.Select(x => new { x.DB_ID, x.DB }).ToList();
            return View();
            
        }

       
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var corporate = db.Corp_DBs.Include(x => x.CorporateMasterList).Select(x => x.CorporateMasterList).Distinct();

            return Json(corporate.ToDataSourceResult(request, x => new VMCorporateMasterLists
            {
                corpID = x.corpID, 
                CorporateName = x.CorporateName, 
                active = x.active
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult HierarchyBinding_DBs(int corpID, [DataSourceRequest] DataSourceRequest request)
        {
            var result = db.Corp_DBs.Include(x => x.CorporateMasterList).Include(x => x.DBList).Where(x => x.corpID == corpID);

            return Json(result.ToDataSourceResult(request, x => new VMCorp_DB
            {
                ID_PK = x.ID_PK, //PK from Corp_DBs table
                DB_ID = x.DB_ID, //FK from DBList table
                corpID = x.corpID, //FK from CorporateMasterList table
                databaseName = x.DBList.databaseName,
                active = x.DBList.active != null && x.DBList.active.Value
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ID_PK,corpID,DB_ID")] VMCorp_DB corp_DBs)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Corp_DBs.AnyAsync(x => x.DB_ID == corp_DBs.DB_ID && x.ID_PK != corp_DBs.ID_PK))
                    {
                        ModelState.AddModelError("","Duplicate data. Please try again!");
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
                  ModelState.AddModelError("","Something failed. Please try again!");
                  return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
                }
               
            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
        }
      

      

        // GET: Corp_DBs/Create
        public ActionResult Create()
        {
            var dbsTaken = db.Corp_DBs.Select(x => x.DBList).ToList();
            var dbsNoTaken = db.DBLists.ToList().Except(dbsTaken);

            if (!db.CorporateMasterLists.Any())
            {
                ViewBag.Corporate = "The CorporateMasterList table is empty.";               
            }
            if (!db.DBLists.Any())
            {
                ViewBag.DBs = "The DBList table is empty.";
            }
            if (!dbsNoTaken.Any())
            {
                ViewBag.Taken = "All databases are asociated.";
            }

            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName");
            ViewBag.DB_ID = new SelectList(dbsNoTaken, "DB_ID", "DB");
            return View();
        }

        // POST: Corp_DBs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID_PK,corpID,DB_ID")] Corp_DBs corp_DBs)
        {           
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Corp_DBs.Include(x => x.DBList).Select(x => x.DBList).AnyAsync(x => x.DB_ID == corp_DBs.DB_ID))
                    {
                        var corp = await db.Corp_DBs.Include(x => x.DBList).Include(x => x.CorporateMasterList).FirstOrDefaultAsync(x => x.DB_ID == corp_DBs.DB_ID);
                        ViewBag.Error = "The database" + " " + corp.DBList.DB + " " + "is already asociated to" + corp.CorporateMasterList.CorporateName;

                        var dbsTaken = db.Corp_DBs.Select(x => x.DBList).ToList();
                        var dbsNoTaken = db.DBLists.ToList().Except(dbsTaken);

                        ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
                        ViewBag.DB_ID = new SelectList(dbsNoTaken, "DB_ID", "DB", corp_DBs.DB_ID);
                        return View(corp_DBs);
                    }
                    db.Corp_DBs.Add(corp_DBs);
                    await db.SaveChangesAsync();

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

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    var dbsTaken1 = db.Corp_DBs.Select(x => x.DBList).ToList();
                    var dbsNoTaken1 = db.DBLists.ToList().Except(dbsTaken1);

                    ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
                    ViewBag.DB_ID = new SelectList(dbsNoTaken1, "DB_ID", "DB", corp_DBs.DB_ID);
                    return View(corp_DBs);
                }
                
            }
            var dbsTaken2 = db.Corp_DBs.Select(x => x.DBList).ToList();
            var dbsNoTaken2 = db.DBLists.ToList().Except(dbsTaken2);

            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
            ViewBag.DB_ID = new SelectList(dbsNoTaken2, "DB_ID", "DB", corp_DBs.DB_ID);
            return View(corp_DBs);
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
