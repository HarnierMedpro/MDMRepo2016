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
    public class MDM_POS_ListNameController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        //// GET: MDM_POS_ListName
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.MDM_POS_ListName.ToListAsync());
        //}

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = await db.MDM_POS_ListName.Select(x => new VMMDM_POS_ListName { active = x.active, MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.PosName }).ToListAsync();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "MDMPOS_ListNameID,PosName,active")] VMMDM_POS_ListName mDM_POS_ListName)
        {
            if (mDM_POS_ListName != null && ModelState.IsValid)
            {
                var StoredInDB = await db.MDM_POS_ListName.FindAsync(mDM_POS_ListName.MDMPOS_ListNameID);

                if (StoredInDB == null)
                {
                    return RedirectToAction("Index", "Error", new { area = "Error" });
                }

                var list = new List<MDM_POS_ListName>() { StoredInDB };
                var distinct = db.MDM_POS_ListName.ToList().Except(list);
                if (distinct.FirstOrDefault(x => x.PosName == mDM_POS_ListName.PosName) != null)
                {
                    /*Duplicate POS*/
                    ModelState.AddModelError("", "Duplicate POS.");
                    var toView = new VMMDM_POS_ListName(){MDMPOS_ListNameID = StoredInDB.MDMPOS_ListNameID, active = StoredInDB.active, PosName = StoredInDB.PosName};
                    return Json(new[] { toView }.ToDataSourceResult(request,ModelState));
                }
                try
                {
                    StoredInDB.PosName = mDM_POS_ListName.PosName;
                    StoredInDB.active = mDM_POS_ListName.active;
                    db.Entry(StoredInDB).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Somthing fail. Please try again!");
                    return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
                } 
            }
            ModelState.AddModelError("", "Somthing fail. Please try again!");
            return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
           
           
           
           

            //string[] fieldsToBind = new string[] { "PosName", "active"};
            //if (TryUpdateModel(StoredInDB, fieldsToBind))
            //{
            //    try
            //    {
            //        db.Entry(StoredInDB).State = EntityState.Modified;
            //        await db.SaveChangesAsync();
            //        return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request));
            //    }
            //    catch (Exception)
            //    {
            //        return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));                       
            //    } 
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Error", new { area = "Error" });
            //}

            ////return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
        }

        // GET: MDM_POS_ListName/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MDM_POS_ListName mDM_POS_ListName = await db.MDM_POS_ListName.FindAsync(id);
            if (mDM_POS_ListName == null)
            {
                return HttpNotFound();
            }
            return View(mDM_POS_ListName);
        }

        // GET: MDM_POS_ListName/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MDM_POS_ListName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MDMPOS_ListNameID,PosName,active")] MDM_POS_ListName mDM_POS_ListName)
        {
            if (ModelState.IsValid)
            {
                /*If exist in DB a POS with the same name, notify to the user that mDM_POS_ListName can not be inserted in the system*/
                if (db.MDM_POS_ListName.Where(x => x.PosName == mDM_POS_ListName.PosName).Count() > 0)
                {
                    ViewBag.Error = "Duplicate POS";
                    return View(mDM_POS_ListName);
                }
                try
                {
                    db.MDM_POS_ListName.Add(mDM_POS_ListName);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    return View(mDM_POS_ListName);
                }                
            }
            return View(mDM_POS_ListName);
        }

        // GET: MDM_POS_ListName/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MDM_POS_ListName mDM_POS_ListName = await db.MDM_POS_ListName.FindAsync(id);
            if (mDM_POS_ListName == null)
            {
                return HttpNotFound();
            }
            return View(mDM_POS_ListName);
        }

        // POST: MDM_POS_ListName/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MDMPOS_ListNameID,PosName")] MDM_POS_ListName mDM_POS_ListName)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mDM_POS_ListName).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(mDM_POS_ListName);
        }

        // GET: MDM_POS_ListName/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MDM_POS_ListName mDM_POS_ListName = await db.MDM_POS_ListName.FindAsync(id);
            if (mDM_POS_ListName == null)
            {
                return HttpNotFound();
            }
            return View(mDM_POS_ListName);
        }

        // POST: MDM_POS_ListName/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MDM_POS_ListName mDM_POS_ListName = await db.MDM_POS_ListName.FindAsync(id);
            db.MDM_POS_ListName.Remove(mDM_POS_ListName);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckIfExist(string term)
        {
            return Json(db.MDM_POS_ListName.Where(x => x.PosName == term).Select(x => x.PosName).ToList(), JsonRequestBehavior.AllowGet);
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
