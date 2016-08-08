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
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    public class BI_DB_FvP_AccessController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ManagerDBA/BI_DB_FvP_Access
        //public async Task<ActionResult> Index()
        //{
        //    var bI_DB_FvP_Access = db.BI_DB_FvP_Access.Include(b => b.DBList).Include(b => b.FvPList).Include(b => b.Manager_Master);
        //    return View(await bI_DB_FvP_Access.ToListAsync());
        //}

        public ActionResult Index()
        {
            ViewData["Manager"] = db.Manager_Master.Select(manager => new {manager.ManagerID, manager.AliasName});
            ViewData["DB"] = db.DBLists.Select(dB => new {dB.DB_ID, dB.DB});
            ViewData["FvP"] = db.FvPLists.Select(fvp => new {fvp.FvPID, fvp.FvPName});
            return View();
        }

        public ActionResult Read_GroupByManager([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.BI_DB_FvP_Access.Include(manager => manager.Manager_Master).Select(manager => manager.Manager_Master).Distinct();
            return Json(result.ToDataSourceResult(request, x => new VMManager_BI
            {
                ManagerID = x.ManagerID, //PK from Manager_Master table
                AliasName = x.AliasName,
                Active = x.Active
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
            /*Some Logic Here*/
            if (bI_DB_FvP_Access != null && ModelState.IsValid)
            {
                try
                {
                    //var storedInDb = await db.BI_DB_FvP_Access.FindAsync(bI_DB_FvP_Access.BIDbFvPID);
                    //var s2 = new List<BI_DB_FvP_Access>(){storedInDb};
                    //var except = db.BI_DB_FvP_Access.ToList().Except(s2);
                    //var aux = except.ToList();
                    //if (except.Any((x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID)))
                    if (await db.BI_DB_FvP_Access.AnyAsync(x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID && x.BIDbFvPID != bI_DB_FvP_Access.BIDbFvPID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = await db.BI_DB_FvP_Access.FindAsync(bI_DB_FvP_Access.BIDbFvPID);
                    storedInDb.ManagerID = bI_DB_FvP_Access.ManagerID;
                    storedInDb.DB_ID = bI_DB_FvP_Access.DB_ID;
                    storedInDb.FvPID = bI_DB_FvP_Access.FvPID;
                    storedInDb.Active = bI_DB_FvP_Access.Active;

                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                }
               
            }
            ModelState.AddModelError("","Something failed. Please try again!");
            return Json(new[] {bI_DB_FvP_Access}.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_BI_DB_FvP([DataSourceRequest] DataSourceRequest request, [Bind(Include = "BIDbFvPID,ManagerID,DB_ID,FvPID,Active")] VMBI_DB_FvP bI_DB_FvP_Access, int ParentID)
        {
            if (bI_DB_FvP_Access != null && ModelState.IsValid)
            {
                try
                {
                    if (await db.BI_DB_FvP_Access.AnyAsync(x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                    }
                    var newOb = new BI_DB_FvP_Access {BIDbFvPID = bI_DB_FvP_Access.BIDbFvPID, FvPID = bI_DB_FvP_Access.FvPID, DB_ID = bI_DB_FvP_Access.DB_ID, Active = bI_DB_FvP_Access.Active, ManagerID = ParentID};
                    db.BI_DB_FvP_Access.Add(newOb);
                    await db.SaveChangesAsync();
                    return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { bI_DB_FvP_Access }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
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

        // GET: ManagerDBA/BI_DB_FvP_Access/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BI_DB_FvP_Access bI_DB_FvP_Access = await db.BI_DB_FvP_Access.FindAsync(id);
            if (bI_DB_FvP_Access == null)
            {
                return HttpNotFound();
            }
            return View(bI_DB_FvP_Access);
        }

        // GET: ManagerDBA/BI_DB_FvP_Access/Create
        public ActionResult Create()
        {
            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB");
            ViewBag.FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName");
            ViewBag.ManagerID = new SelectList(db.Manager_Master, "ManagerID", "AliasName");
            return View();
        }

        // POST: ManagerDBA/BI_DB_FvP_Access/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BIDbFvPID,ManagerID,DB_ID,FvPID,Active")] BI_DB_FvP_Access bI_DB_FvP_Access)
        {
            if (ModelState.IsValid)
            {
                if (db.BI_DB_FvP_Access.FirstOrDefault(x => x.DB_ID == bI_DB_FvP_Access.DB_ID && x.FvPID == bI_DB_FvP_Access.FvPID) != null)
                {
                    ViewBag.Error = "Duplicate Data.";
                    return View(bI_DB_FvP_Access);
                }
                try
                {
                    bI_DB_FvP_Access.Active = true;
                    db.BI_DB_FvP_Access.Add(bI_DB_FvP_Access);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Create");
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    return View(bI_DB_FvP_Access);
                }
               
            }
            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", bI_DB_FvP_Access.DB_ID);
            ViewBag.FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName", bI_DB_FvP_Access.FvPID);
            ViewBag.ManagerID = new SelectList(db.Manager_Master, "ManagerID", "AliasName", bI_DB_FvP_Access.ManagerID);
            return View(bI_DB_FvP_Access);
        }

        // GET: ManagerDBA/BI_DB_FvP_Access/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BI_DB_FvP_Access bI_DB_FvP_Access = await db.BI_DB_FvP_Access.FindAsync(id);
            if (bI_DB_FvP_Access == null)
            {
                return HttpNotFound();
            }
            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", bI_DB_FvP_Access.DB_ID);
            ViewBag.FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName", bI_DB_FvP_Access.FvPID);
            ViewBag.ManagerID = new SelectList(db.Manager_Master, "ManagerID", "AliasName", bI_DB_FvP_Access.ManagerID);
            return View(bI_DB_FvP_Access);
        }

        // POST: ManagerDBA/BI_DB_FvP_Access/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BIDbFvPID,ManagerID,DB_ID,FvPID,Active")] BI_DB_FvP_Access bI_DB_FvP_Access)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bI_DB_FvP_Access).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", bI_DB_FvP_Access.DB_ID);
            ViewBag.FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName", bI_DB_FvP_Access.FvPID);
            ViewBag.ManagerID = new SelectList(db.Manager_Master, "ManagerID", "AliasName", bI_DB_FvP_Access.ManagerID);
            return View(bI_DB_FvP_Access);
        }

        // GET: ManagerDBA/BI_DB_FvP_Access/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BI_DB_FvP_Access bI_DB_FvP_Access = await db.BI_DB_FvP_Access.FindAsync(id);
            if (bI_DB_FvP_Access == null)
            {
                return HttpNotFound();
            }
            return View(bI_DB_FvP_Access);
        }

        // POST: ManagerDBA/BI_DB_FvP_Access/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BI_DB_FvP_Access bI_DB_FvP_Access = await db.BI_DB_FvP_Access.FindAsync(id);
            db.BI_DB_FvP_Access.Remove(bI_DB_FvP_Access);
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
