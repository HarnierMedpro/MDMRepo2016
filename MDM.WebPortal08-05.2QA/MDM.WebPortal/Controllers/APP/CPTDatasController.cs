using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
//using MedProMDM.Models;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    [Authorize]
    public class CPTDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        //// GET: CPTDatas
        //public ActionResult Index(string searchString)
        //{

        //    var varList = from s in db.CPTDatas
        //                  select s;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        varList = varList.Where(s => s.CPT.Contains(searchString));
        //    }

        //    varList = varList.OrderBy(s => s.CPT);
        //    return View(varList.ToList());
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.CPTDatas.ToDataSourceResult(request, x => new VMCPTData
            {
                id = x.id,
                CPT = x.CPT,
                CPT_Description = x.CPT_Description,
                ShortD = x.ShortD,
                Active = x.Active != null && x.Active.Value
            }), JsonRequestBehavior.AllowGet);
        }

      public async Task<ActionResult> Create_CPT([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "id,CPT,CPT_Description,ShortD, Active")] VMCPTData cPTData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CPTDatas.AnyAsync(x => x.CPT == cPTData.CPT))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new CPTData
                    {
                        CPT = cPTData.CPT,
                        CPT_Description = cPTData.CPT_Description,
                        ShortD = cPTData.ShortD,
                        Active = cPTData.Active
                    };
                    db.CPTDatas.Add(toStore);
                    await db.SaveChangesAsync();
                    cPTData.id = toStore.id;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                }                
            }
            return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "id,CPT,CPT_Description,ShortD, Active")] VMCPTData cPTData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CPTDatas.AnyAsync(x => x.CPT == cPTData.CPT && x.id != cPTData.id))
                    {
                        ModelState.AddModelError("", "Duplicate CPT. Please try again!");
                        return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                    }                    
                    var storedInDb = new CPTData
                    {
                        id = cPTData.id,
                        CPT = cPTData.CPT,
                        CPT_Description = cPTData.CPT_Description,
                        ShortD = cPTData.ShortD,
                        Active = cPTData.Active,
                    };
                    db.CPTDatas.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();                    
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Somethig failed. Please try again!");
                    return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
        }

        // GET: CPTDatas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPTData cPTData = db.CPTDatas.Find(id);
            if (cPTData == null)
            {
                return HttpNotFound();
            }
            return View(cPTData);
        }

        // GET: CPTDatas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CPTDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,CPT,CPT_Description,ShortD,Active")] CPTData cPTData)
        {
            if (ModelState.IsValid)
            {
                if (await db.CPTDatas.AnyAsync(x => x.CPT == cPTData.CPT))
                {
                    TempData["Error"] = "Duplicate CPT. Please try again!";
                    return RedirectToAction("Index");
                }
                try
                {
                    cPTData.Active = true;
                    db.CPTDatas.Add(cPTData);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["Error"] = "Somthing fail. Please try again!";
                    return RedirectToAction("Index");
                }
            }
            TempData["Error"] = "Somthing fail. Please try again!";
            //return RedirectToAction("Index");
            return View("_CreateCpt", cPTData);
        }

        // GET: CPTDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPTData cPTData = db.CPTDatas.Find(id);
            if (cPTData == null)
            {
                return HttpNotFound();
            }
            return View(cPTData);
        }

        // POST: CPTDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CPT,CPT_Description,ShortD,NTUser")] CPTData cPTData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cPTData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cPTData);
        }

        // GET: CPTDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPTData cPTData = db.CPTDatas.Find(id);
            if (cPTData == null)
            {
                return HttpNotFound();
            }
            return View(cPTData);
        }

        // POST: CPTDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CPTData cPTData = db.CPTDatas.Find(id);
            db.CPTDatas.Remove(cPTData);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FindCpt(string term)
        {
           var result = db.CPTDatas.Where(x => x.CPT == term).Select(x => new VMCPTData
            {
                Active = x.Active != null && x.Active.Value,
                CPT = x.CPT,
                ShortD = x.ShortD,
                CPT_Description = x.CPT_Description,
                id = x.id
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
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