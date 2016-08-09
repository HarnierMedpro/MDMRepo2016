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
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    //[SetPermissions(Permissions = "Index,Read,HierarchyBinding_DBs")]
    //[SetPermissions()]
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
                    var storedInDb = new Corp_DBs { ID_PK = corp_DBs.ID_PK, DB_ID = corp_DBs.DB_ID, corpID = corp_DBs.corpID};
                    db.Corp_DBs.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();                    
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
                var toRelease = new Corp_DBs { ID_PK = corp_DBs.ID_PK, corpID = corp_DBs.corpID, DB_ID = corp_DBs.DB_ID };
                db.Corp_DBs.Attach(toRelease);
                db.Corp_DBs.Remove(toRelease);
                await db.SaveChangesAsync();
            }
            return Json(new[] { corp_DBs }.ToDataSourceResult(request, ModelState));
        }
      

        // GET: Corp_DBs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Corp_DBs corp_DBs = await db.Corp_DBs.FindAsync(id);
            if (corp_DBs == null)
            {
                return HttpNotFound();
            }
            return View(corp_DBs);
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

        // GET: Corp_DBs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Corp_DBs corp_DBs = await db.Corp_DBs.FindAsync(id);
            if (corp_DBs == null)
            {
                return HttpNotFound();
            }
            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
            var dbsTaken = db.Corp_DBs.Select(x => x.DBList).ToList();
            var dbsNoTaken = db.DBLists.ToList().Except(dbsTaken).ToList();
            var ownDB = corp_DBs.DBList;
            dbsNoTaken.Add(corp_DBs.DBList);
            /*Para este caso */
            ViewBag.DB_ID = new SelectList(dbsNoTaken, "DB_ID", "DB", corp_DBs.DB_ID);
            return View(corp_DBs);
        }

        // POST: Corp_DBs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_PK,corpID,DB_ID")] Corp_DBs corp_DBs)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDB = await db.Corp_DBs.FindAsync(corp_DBs.ID_PK);
                    var list = new List<Corp_DBs>() { storedInDB };
                    var distinct = db.Corp_DBs.ToList().Except(list);

                    if (distinct.Select(x => x.DBList).ToList().Exists(x => x.DB_ID == corp_DBs.DB_ID))
                    {
                       //return RedirectToAction("Index", "Error", new { area = "Error" });
                        var corp = await db.Corp_DBs.Include(x => x.DBList).Include(x => x.CorporateMasterList).FirstOrDefaultAsync(x => x.DB_ID == corp_DBs.DB_ID);
                        ViewBag.Error = "The database" + " " + corp.DBList.DB + " " + "is already asociated to" + " " + corp.CorporateMasterList.CorporateName;

                        var dbsTaken = db.Corp_DBs.Select(x => x.DBList).ToList();
                        var dbsNoTaken = db.DBLists.ToList().Except(dbsTaken);

                        var t = dbsNoTaken.ToList();
                        var c = db.Corp_DBs.Find(corp_DBs.ID_PK).DBList;
                        t.Add(c);                   

                        ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
                        ViewBag.DB_ID = new SelectList(t, "DB_ID", "DB", c.DB_ID);
                        return View();
                    }
                    else
                    {
                        string[] fieldsToBind = new string[] { "corpID", "DB_ID" };
                        if (TryUpdateModel(storedInDB, fieldsToBind))
                        {
                            db.Entry(storedInDB).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Error = "Something failed. Please try again!";
                            var dbsTaken1 = db.Corp_DBs.Select(x => x.DBList).ToList();
                            var dbsNoTaken1 = db.DBLists.ToList().Except(dbsTaken1);

                            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
                            ViewBag.DB_ID = new SelectList(dbsNoTaken1, "DB_ID", "DB", corp_DBs.DB_ID);
                            return View();
                        }
                    }
                    
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    var dbsTaken1 = db.Corp_DBs.Select(x => x.DBList).ToList();
                    var dbsNoTaken1 = db.DBLists.ToList().Except(dbsTaken1);

                    ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
                    ViewBag.DB_ID = new SelectList(dbsNoTaken1, "DB_ID", "DB", corp_DBs.DB_ID);
                    return View();
                }
                
            }
            var dbsTaken2 = db.Corp_DBs.Select(x => x.DBList).ToList();
            var dbsNoTaken2 = db.DBLists.ToList().Except(dbsTaken2);

            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_DBs.corpID);
            ViewBag.DB_ID = new SelectList(dbsNoTaken2, "DB_ID", "DB", corp_DBs.DB_ID);
            return View(corp_DBs);           
        }

        // GET: Corp_DBs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Corp_DBs corp_DBs = await db.Corp_DBs.FindAsync(id);
            if (corp_DBs == null)
            {
                return HttpNotFound();
            }
            return View(corp_DBs);
        }

        // POST: Corp_DBs/Delete/5
        /*Este metodo tiene problemas con la estructura de la BD*/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Corp_DBs corp_DBs = await db.Corp_DBs.FindAsync(id);
            db.Corp_DBs.Remove(corp_DBs);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
