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
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    public class OwnerListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.OwnerLists.ToDataSourceResult(request, x => new VMOwnerList
            {
                OwnersID = x.OwnersID, 
                active = x.active, 
                FirstName = x.FirstName, 
                LastName = x.LastName
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult HierarchyBinding_Corp([DataSourceRequest] DataSourceRequest request, int? OwnersID)
        {
            if (OwnersID != null)
            {
                var myCorporates =
                    db.Corp_Owner.Include(x => x.CorporateMasterList)
                        .Where(x => x.OwnersID == OwnersID)
                        .Select(x => x.CorporateMasterList);
                return Json(myCorporates.ToDataSourceResult(request, x => new VMCorporateMasterLists
                {
                    corpID = x.corpID, 
                    CorporateName = x.CorporateName,
                    active = x.active
                }), JsonRequestBehavior.AllowGet);
            }
           ModelState.AddModelError("","Something failed. Please try again!");
           return Json(new[]{new VMCorporateMasterLists()}.ToDataSourceResult(request,ModelState));
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "OwnersID,LastName,FirstName,active")] VMOwnerList ownerList)
        {
            if (ownerList != null && ModelState.IsValid)
            {
                try
                {
                    db.Entry(ownerList).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { ownerList }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { ownerList }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { ownerList }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_Owner([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "OwnersID,LastName,FirstName,active")] OwnerList ownerList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.OwnerLists.Add(ownerList);
                    await db.SaveChangesAsync();
                    return Json(new[] { ownerList }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { ownerList }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { ownerList }.ToDataSourceResult(request, ModelState));
        }

        // GET: OwnerLists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerList ownerList = await db.OwnerLists.FindAsync(id);
            if (ownerList == null)
            {
                return HttpNotFound();
            }
            return View(ownerList);
        }

        // GET: OwnerLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OwnerLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OwnersID,LastName,FirstName,active")] OwnerList ownerList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.OwnerLists.Add(ownerList);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    return View(ownerList);                    
                }                
            }
            return View(ownerList);
        }

        // GET: OwnerLists/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerList ownerList = await db.OwnerLists.FindAsync(id);
            if (ownerList == null)
            {
                return HttpNotFound();
            }
            return View(ownerList);
        }

        // POST: OwnerLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OwnersID,LastName,FirstName")] OwnerList ownerList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ownerList).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ownerList);
        }

        // GET: OwnerLists/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerList ownerList = await db.OwnerLists.FindAsync(id);
            if (ownerList == null)
            {
                return HttpNotFound();
            }
            return View(ownerList);
        }

        // POST: OwnerLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OwnerList ownerList = await db.OwnerLists.FindAsync(id);
            db.OwnerLists.Remove(ownerList);
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
