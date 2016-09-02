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
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class LocPOS_MPServController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Create_POSService([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "LocPosMPServID,MPServices_MPServID")] VMLocPOS_MPServ pOSLOC, int ParentID)
        {
            if (ModelState.IsValid && ParentID > 0)
            {
                try
                {
                    if (await db.LocPOS_MPServ.AnyAsync(x => x.MPServices_MPServID == pOSLOC.MPServices_MPServID && x.LocationsPOS_Facitity_DBs_IDPK == ParentID))
                    {
                        pOSLOC.LocationsPOS_Facitity_DBs_IDPK = ParentID;
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new LocPOS_MPServ
                    {
                        MPServices_MPServID = pOSLOC.MPServices_MPServID,
                        LocationsPOS_Facitity_DBs_IDPK = ParentID
                    };

                    db.LocPOS_MPServ.Add(toStore);
                    await db.SaveChangesAsync();
                    pOSLOC.LocPosMPServID = toStore.LocPosMPServID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        ModelPKey = toStore.LocPosMPServID,
                        TableName = "LocPOS_MPServ",
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "MPServices_MPServID", NewValue = pOSLOC.MPServices_MPServID.ToString()},
                            new TableInfo{Field_ColumName = "LocationsPOS_Facitity_DBs_IDPK", NewValue = ParentID.ToString()}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
        }


        public async Task<ActionResult> Update_POSService([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "LocPosMPServID,MPServices_MPServID,LocationsPOS_Facitity_DBs_IDPK")] VMLocPOS_MPServ
               pOSLOC)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.LocPOS_MPServ.AnyAsync(x => x.MPServices_MPServID == pOSLOC.MPServices_MPServID && x.LocationsPOS_Facitity_DBs_IDPK == pOSLOC.LocationsPOS_Facitity_DBs_IDPK
                        && x.LocPosMPServID != pOSLOC.LocPosMPServID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.LocPOS_MPServ.FindAsync(pOSLOC.LocPosMPServID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDb.MPServices_MPServID != pOSLOC.MPServices_MPServID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "MPServices_MPServID", NewValue = pOSLOC.MPServices_MPServID.ToString(), OldValue = storedInDb.MPServices_MPServID.ToString() });
                        storedInDb.MPServices_MPServID = pOSLOC.MPServices_MPServID;
                    }


                    db.LocPOS_MPServ.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        ModelPKey = pOSLOC.LocPosMPServID,
                        TableName = "LocPOS_MPServ",
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
        }
        
        
        
        // GET: PlaceOfServices/LocPOS_MPServ
        public async Task<ActionResult> Index()
        {
            var locPOS_MPServ = db.LocPOS_MPServ.Include(l => l.LocationsPOS).Include(l => l.MPService);
            return View(await locPOS_MPServ.ToListAsync());
        }

        // GET: PlaceOfServices/LocPOS_MPServ/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocPOS_MPServ locPOS_MPServ = await db.LocPOS_MPServ.FindAsync(id);
            if (locPOS_MPServ == null)
            {
                return HttpNotFound();
            }
            return View(locPOS_MPServ);
        }

        // GET: PlaceOfServices/LocPOS_MPServ/Create
        public ActionResult Create()
        {
            ViewBag.LocationsPOS_Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName");
            ViewBag.MPServices_MPServID = new SelectList(db.MPServices, "MPServID", "ServName");
            return View();
        }

        // POST: PlaceOfServices/LocPOS_MPServ/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LocPosMPServID,MPServices_MPServID,LocationsPOS_Facitity_DBs_IDPK")] LocPOS_MPServ locPOS_MPServ)
        {
            if (ModelState.IsValid)
            {
                db.LocPOS_MPServ.Add(locPOS_MPServ);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.LocationsPOS_Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", locPOS_MPServ.LocationsPOS_Facitity_DBs_IDPK);
            ViewBag.MPServices_MPServID = new SelectList(db.MPServices, "MPServID", "ServName", locPOS_MPServ.MPServices_MPServID);
            return View(locPOS_MPServ);
        }

        // GET: PlaceOfServices/LocPOS_MPServ/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocPOS_MPServ locPOS_MPServ = await db.LocPOS_MPServ.FindAsync(id);
            if (locPOS_MPServ == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationsPOS_Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", locPOS_MPServ.LocationsPOS_Facitity_DBs_IDPK);
            ViewBag.MPServices_MPServID = new SelectList(db.MPServices, "MPServID", "ServName", locPOS_MPServ.MPServices_MPServID);
            return View(locPOS_MPServ);
        }

        // POST: PlaceOfServices/LocPOS_MPServ/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LocPosMPServID,MPServices_MPServID,LocationsPOS_Facitity_DBs_IDPK")] LocPOS_MPServ locPOS_MPServ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locPOS_MPServ).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LocationsPOS_Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", locPOS_MPServ.LocationsPOS_Facitity_DBs_IDPK);
            ViewBag.MPServices_MPServID = new SelectList(db.MPServices, "MPServID", "ServName", locPOS_MPServ.MPServices_MPServID);
            return View(locPOS_MPServ);
        }

        // GET: PlaceOfServices/LocPOS_MPServ/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocPOS_MPServ locPOS_MPServ = await db.LocPOS_MPServ.FindAsync(id);
            if (locPOS_MPServ == null)
            {
                return HttpNotFound();
            }
            return View(locPOS_MPServ);
        }

        // POST: PlaceOfServices/LocPOS_MPServ/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LocPOS_MPServ locPOS_MPServ = await db.LocPOS_MPServ.FindAsync(id);
            db.LocPOS_MPServ.Remove(locPOS_MPServ);
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
