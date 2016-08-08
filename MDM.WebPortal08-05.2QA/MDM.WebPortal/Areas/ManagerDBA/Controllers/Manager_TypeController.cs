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
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Controllers
{
    public class Manager_TypeController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ManagerDBA/Manager_Type
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.Manager_Type.ToListAsync());
        //}

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
            /*Some Logic*/
            if (manager_Type != null && ModelState.IsValid)
            {
                var storedInDb = new Manager_Type(){ManagerTypeID = manager_Type.ManagerTypeID, Name = manager_Type.Name, Active = manager_Type.Active};
                try
                {
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { manager_Type }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_Type([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ManagerTypeID,Name,Active")] Manager_Type manager_Type)
        {
            /*Some Logic*/
            if (manager_Type != null && ModelState.IsValid)
            {
                try
                {
                    db.Manager_Type.Add(manager_Type);
                    await db.SaveChangesAsync();
                    return Json(new[] { manager_Type }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Somthing fail. Please try again!");
                    return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Somthing fail. Please try again!");
            return Json(new[] { manager_Type }.ToDataSourceResult(request, ModelState));
        }

        // GET: ManagerDBA/Manager_Type/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager_Type manager_Type = await db.Manager_Type.FindAsync(id);
            if (manager_Type == null)
            {
                return HttpNotFound();
            }
            return View(manager_Type);
        }

        // GET: ManagerDBA/Manager_Type/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagerDBA/Manager_Type/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ManagerTypeID,Name,Active")] Manager_Type manager_Type)
        {
            if (ModelState.IsValid)
            {
                db.Manager_Type.Add(manager_Type);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(manager_Type);
        }

        // GET: ManagerDBA/Manager_Type/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager_Type manager_Type = await db.Manager_Type.FindAsync(id);
            if (manager_Type == null)
            {
                return HttpNotFound();
            }
            return View(manager_Type);
        }

        // POST: ManagerDBA/Manager_Type/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ManagerTypeID,Name,Active")] Manager_Type manager_Type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manager_Type).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(manager_Type);
        }

        // GET: ManagerDBA/Manager_Type/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager_Type manager_Type = await db.Manager_Type.FindAsync(id);
            if (manager_Type == null)
            {
                return HttpNotFound();
            }
            return View(manager_Type);
        }

        // POST: ManagerDBA/Manager_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Manager_Type manager_Type = await db.Manager_Type.FindAsync(id);
            db.Manager_Type.Remove(manager_Type);
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
