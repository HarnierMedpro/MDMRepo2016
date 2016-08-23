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

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class LocationsPOSController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/LocationsPOS
        public async Task<ActionResult> Index()
        {
            var locationsPOS = db.LocationsPOS.Include(l => l.FACInfoData).Include(l => l.Facitity_DBs).Include(l => l.FvPList).Include(l => l.PHYGroup);
            return View(await locationsPOS.ToListAsync());
        }

        // GET: PlaceOfServices/LocationsPOS/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationsPOS locationsPOS = await db.LocationsPOS.FindAsync(id);
            if (locationsPOS == null)
            {
                return HttpNotFound();
            }
            return View(locationsPOS);
        }

        // GET: PlaceOfServices/LocationsPOS/Create
        public ActionResult Create()
        {
            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName");
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.Facitity_DBs, "IDPK", "DB");
            ViewBag.FvPList_FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName");
            ViewBag.PHYGroups_PHYGrpID = new SelectList(db.PHYGroups, "PHYGrpID", "PHYGroupName");
            return View();
        }

        // POST: PlaceOfServices/LocationsPOS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Facitity_DBs_IDPK,PosName,FvPList_FvPID,FACInfoData_FACInfoDataID,PHYGroups_PHYGrpID,TaxID,DBA_Name,Payment_Addr1,Payment_Addr2,Payment_City,Payment_state,Payment_Zip,Physical_Addr1,Physical_Addr2,Physical_City,Physical_state,Physical_Zip,POSFAC_Manager,Notes")] LocationsPOS locationsPOS)
        {
            if (ModelState.IsValid)
            {
                db.LocationsPOS.Add(locationsPOS);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName", locationsPOS.FACInfoData_FACInfoDataID);
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.Facitity_DBs, "IDPK", "DB", locationsPOS.Facitity_DBs_IDPK);
            ViewBag.FvPList_FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName", locationsPOS.FvPList_FvPID);
            ViewBag.PHYGroups_PHYGrpID = new SelectList(db.PHYGroups, "PHYGrpID", "PHYGroupName", locationsPOS.PHYGroups_PHYGrpID);
            return View(locationsPOS);
        }

        // GET: PlaceOfServices/LocationsPOS/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationsPOS locationsPOS = await db.LocationsPOS.FindAsync(id);
            if (locationsPOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName", locationsPOS.FACInfoData_FACInfoDataID);
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.Facitity_DBs, "IDPK", "DB", locationsPOS.Facitity_DBs_IDPK);
            ViewBag.FvPList_FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName", locationsPOS.FvPList_FvPID);
            ViewBag.PHYGroups_PHYGrpID = new SelectList(db.PHYGroups, "PHYGrpID", "PHYGroupName", locationsPOS.PHYGroups_PHYGrpID);
            return View(locationsPOS);
        }

        // POST: PlaceOfServices/LocationsPOS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Facitity_DBs_IDPK,PosName,FvPList_FvPID,FACInfoData_FACInfoDataID,PHYGroups_PHYGrpID,TaxID,DBA_Name,Payment_Addr1,Payment_Addr2,Payment_City,Payment_state,Payment_Zip,Physical_Addr1,Physical_Addr2,Physical_City,Physical_state,Physical_Zip,POSFAC_Manager,Notes")] LocationsPOS locationsPOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationsPOS).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName", locationsPOS.FACInfoData_FACInfoDataID);
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.Facitity_DBs, "IDPK", "DB", locationsPOS.Facitity_DBs_IDPK);
            ViewBag.FvPList_FvPID = new SelectList(db.FvPLists, "FvPID", "FvPName", locationsPOS.FvPList_FvPID);
            ViewBag.PHYGroups_PHYGrpID = new SelectList(db.PHYGroups, "PHYGrpID", "PHYGroupName", locationsPOS.PHYGroups_PHYGrpID);
            return View(locationsPOS);
        }

        // GET: PlaceOfServices/LocationsPOS/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationsPOS locationsPOS = await db.LocationsPOS.FindAsync(id);
            if (locationsPOS == null)
            {
                return HttpNotFound();
            }
            return View(locationsPOS);
        }

        // POST: PlaceOfServices/LocationsPOS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LocationsPOS locationsPOS = await db.LocationsPOS.FindAsync(id);
            db.LocationsPOS.Remove(locationsPOS);
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
