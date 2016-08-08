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
using MDM.WebPortal.Areas.ADP.Models;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Areas.ADP.Controllers
{
    public class Edgemed_LogonsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();
        
        public ActionResult Index()
        {
            ViewData["ADP"] =  db.ADPMasters.Select(s => new {ADPID = s.ADPMasterID, INFO = s.ADP_ID + " " + s.LName + "," + " " + s.FName}).ToList();
            return View();
        }

        /*Get all the ADPMaster without duplicates values*/
        public async Task<ActionResult> ADP([DataSourceRequest] DataSourceRequest request)
        {
            var POS = await db.Edgemed_Logons.Include(x => x.ADPMaster).Select(x => x.ADPMaster).Distinct().ToListAsync();
            return Json(POS.ToDataSourceResult(request, x => new VMAdpMaster
            {
                ADPMaster_ID = x.ADPMasterID,
                ADP = x.ADP_ID,
                FName = x.FName,
                LName = x.LName,
                Title = x.Title,
                Manager = x.Manager,
                Active = x.Active
            }),JsonRequestBehavior.AllowGet);
        }

        /*Get al EdgeMedLogons of specific ADP*/
        public ActionResult EdgeMedLogonsforAdp([DataSourceRequest] DataSourceRequest request, int? ADPMasterID)
        {
            IQueryable<Edgemed_Logons> edgemed = db.Edgemed_Logons;

            if (ADPMasterID != null)
            {
                edgemed = edgemed.Where(x => x.ADPMasterID == ADPMasterID);
            }

            return Json(edgemed.ToDataSourceResult(request, x => new VMEdgemed_Logons
            {
                Edgemed_LogID = x.Edgemed_LogID, //PK from Edgemed table
                Edgemed_UserName = x.Edgemed_UserName,
                Zno = x.Zno,
                EdgeMed_ID = x.EdgeMed_ID,
                Active = x.Active,
                ADPMasterID = x.ADPMasterID //FK from dbo.ADP_Master table
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateEdgemed([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "Edgemed_LogID,ADPMasterID,Edgemed_UserName,Zno,EdgeMed_ID,Active")] Edgemed_Logons edgemed_Logons)
        {
            /*Some logic Here*/
            if (edgemed_Logons != null && ModelState.IsValid)
            {
                try
                {
                    db.Entry(edgemed_Logons).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { edgemed_Logons }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { edgemed_Logons }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { edgemed_Logons }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> CreateEdgemed([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "Edgemed_LogID,ADPMasterID,Edgemed_UserName,Zno,EdgeMed_ID,Active")] Edgemed_Logons edgemed_Logons, int ParentID)
        {
            /*Some logic Here*/
            if (edgemed_Logons != null && ModelState.IsValid)
            {
                try
                {
                    edgemed_Logons.ADPMasterID = ParentID;
                    db.Edgemed_Logons.Add(edgemed_Logons);
                    await db.SaveChangesAsync();
                    return Json(new[] { edgemed_Logons }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { edgemed_Logons }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { edgemed_Logons }.ToDataSourceResult(request, ModelState));
        }
        

        /*------------------------------------AUTOCOMPLETE ------------------------------------------------*/
        //First I get all POS
        public JsonResult GetAdps(string text)
        {
            var adps = db.ADPMasters.Select(x => new VMAdpMaster
            {
                ADPMaster_ID = x.ADPMasterID,
                ADP = x.ADP_ID,
                FName = x.FName,
                LName = x.LName,
                Title = x.Title,
                Manager = x.Manager,
                Active = x.Active
            });

            if (!String.IsNullOrEmpty(text))
            {
                adps = adps.Where( x => x.FName.ToLower().Contains(text.ToLower()) || x.LName.ToLower().Contains(text.ToLower()));
            }
            //var result = db.ADPMasters.Where(x => x.FName.ToLower().Contains(text) || x.LName.ToLower().Contains(text))
            //    .Select(s => new { ADPID = s.id, INFO = s.ADP_ID + " " + s.LName + "," + " " + s.FName }).ToList();
            return Json(adps, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Virtualization_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetADP().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<VMAdpMaster> GetADP()
        {
            return db.ADPMasters.Select(adp => new VMAdpMaster
            {
                ADPMaster_ID = adp.ADPMasterID,
                ADP = adp.ADP_ID,
                FName = adp.FName,
                LName = adp.LName,
                Title = adp.Title,
                Manager = adp.Manager,
                Active = adp.Active
            });
        }
        /*------------------------------------AUTOCOMPLETE ------------------------------------------------*/

        // GET: ADP/Edgemed_Logons/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edgemed_Logons edgemed_Logons = await db.Edgemed_Logons.FindAsync(id);
            if (edgemed_Logons == null)
            {
                return HttpNotFound();
            }
            return View(edgemed_Logons);
        }

        // GET: ADP/Edgemed_Logons/Create
        public ActionResult Create()
        {
            //ViewBag.id = new SelectList(db.ADPMasters.Select(x => new { ADPID = x.id, INFO = x.ADP_ID + " " + x.LName + "," + " " + x.FName }), "ADPID", "INFO");
            return View();
        }

        // POST: ADP/Edgemed_Logons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Edgemed_LogID,id,Edgemed_UserName,Zno,EdgeMed_ID,Active")] Edgemed_Logons edgemed_Logons)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Edgemed_Logons.Add(edgemed_Logons);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    return View(edgemed_Logons);
                }
               
            }
            //ViewBag.id = new SelectList(db.ADPMasters, "id", "ADP_ID", edgemed_Logons.id);
            return View(edgemed_Logons);
        }

        // GET: ADP/Edgemed_Logons/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edgemed_Logons edgemed_Logons = await db.Edgemed_Logons.FindAsync(id);
            if (edgemed_Logons == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = new SelectList(db.ADPMasters, "id", "ADP_ID", edgemed_Logons.ADPMasterID);
            return View(edgemed_Logons);
        }

        // POST: ADP/Edgemed_Logons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Edgemed_LogID,id,Edgemed_UserName,Zno,EdgeMed_ID,Active")] Edgemed_Logons edgemed_Logons)
        {
            if (ModelState.IsValid)
            {
                db.Entry(edgemed_Logons).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id = new SelectList(db.ADPMasters, "id", "ADP_ID", edgemed_Logons.ADPMasterID);
            return View(edgemed_Logons);
        }

        // GET: ADP/Edgemed_Logons/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edgemed_Logons edgemed_Logons = await db.Edgemed_Logons.FindAsync(id);
            if (edgemed_Logons == null)
            {
                return HttpNotFound();
            }
            return View(edgemed_Logons);
        }

        // POST: ADP/Edgemed_Logons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Edgemed_Logons edgemed_Logons = await db.Edgemed_Logons.FindAsync(id);
            db.Edgemed_Logons.Remove(edgemed_Logons);
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
