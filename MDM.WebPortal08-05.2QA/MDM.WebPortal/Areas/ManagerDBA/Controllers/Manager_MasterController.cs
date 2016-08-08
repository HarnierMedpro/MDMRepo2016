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
using MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    //[SetPermissions]
    public class Manager_MasterController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ManagerDBA/Manager_Master
        //public async Task<ActionResult> Index()
        //{
        //    var manager_Master = db.Manager_Master.Include(m => m.Manager_Type);
        //    return View(await manager_Master.ToListAsync());
        //}

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
            ViewData["Type"] = db.Manager_Type.Select(x => new { x.ManagerTypeID, x.Name });
            return View();
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
            [Bind(Include = "ManagerID,ManagerTypeID,AliasName,Active")] Manager_Master manager_Master)
        {
            if (manager_Master != null && ModelState.IsValid)
            {
                try
                {
                    if (await db.Manager_Master.AnyAsync(x => x.ManagerTypeID == manager_Master.ManagerTypeID || x.AliasName == manager_Master.AliasName))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
                    }
                    db.Manager_Master.Add(manager_Master);
                    await db.SaveChangesAsync();
                    return Json(new[] {manager_Master}.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Manager([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "ManagerID,ManagerTypeID,AliasName,Active")] Manager_Master manager_Master)
        {
            if (manager_Master != null && ModelState.IsValid)
            {
                //var storedInDB = new Manager_Master(){ManagerID = manager_Master.ManagerID, AliasName = manager_Master.AliasName, Active = manager_Master.Active, ManagerTypeID = manager_Master.ManagerTypeID};
                try
                {
                    if (await db.Manager_Master.AnyAsync(x => x.ManagerTypeID != manager_Master.ManagerTypeID && x.AliasName == manager_Master.AliasName))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[]{new Manager_Master()}.ToDataSourceResult(request, ModelState));
                    }
                    db.Entry(manager_Master).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { manager_Master }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { manager_Master }.ToDataSourceResult(request, ModelState));
        }

        // GET: ManagerDBA/Manager_Master/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager_Master manager_Master = await db.Manager_Master.FindAsync(id);
            if (manager_Master == null)
            {
                return HttpNotFound();
            }
            return View(manager_Master);
        }

        // GET: ManagerDBA/Manager_Master/Create
        public ActionResult Create()
        {
            ViewBag.ManagerTypeID = new SelectList(db.Manager_Type, "ManagerTypeID", "Name");
            return View();
        }

        // POST: ManagerDBA/Manager_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ManagerID,ManagerTypeID,AliasName,Active")] Manager_Master manager_Master)
        {
            if (ModelState.IsValid)
            {
                db.Manager_Master.Add(manager_Master);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ManagerTypeID = new SelectList(db.Manager_Type, "ManagerTypeID", "Name", manager_Master.ManagerTypeID);
            return View(manager_Master);
        }

        // GET: ManagerDBA/Manager_Master/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager_Master manager_Master = await db.Manager_Master.FindAsync(id);
            if (manager_Master == null)
            {
                return HttpNotFound();
            }
            ViewBag.ManagerTypeID = new SelectList(db.Manager_Type, "ManagerTypeID", "Name", manager_Master.ManagerTypeID);
            return View(manager_Master);
        }

        // POST: ManagerDBA/Manager_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ManagerID,ManagerTypeID,AliasName,Active")] Manager_Master manager_Master)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manager_Master).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ManagerTypeID = new SelectList(db.Manager_Type, "ManagerTypeID", "Name", manager_Master.ManagerTypeID);
            return View(manager_Master);
        }

        // GET: ManagerDBA/Manager_Master/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager_Master manager_Master = await db.Manager_Master.FindAsync(id);
            if (manager_Master == null)
            {
                return HttpNotFound();
            }
            return View(manager_Master);
        }

        // POST: ManagerDBA/Manager_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Manager_Master manager_Master = await db.Manager_Master.FindAsync(id);
            db.Manager_Master.Remove(manager_Master);
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
