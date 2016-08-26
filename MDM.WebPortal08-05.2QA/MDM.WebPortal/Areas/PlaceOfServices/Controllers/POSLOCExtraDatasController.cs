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
    public class POSLOCExtraDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/POSLOCExtraDatas
        public async Task<ActionResult> Index()
        {
            var pOSLOCExtraDatas = db.POSLOCExtraDatas.Include(p => p.FACInfoData);
            return View(await pOSLOCExtraDatas.ToListAsync());
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Details/5
        /*Una vez se haya creado el FACInfoData de un LocationsPOS, se dibuja un boton Next que muestra los detalles de POSLOCExtraData del recien creado
         FACInfoData, por eso es que se pasa como parametro al Details "parentFacInfoDataID" */
        public async Task<ActionResult> Details(int? parentFacInfoDataID)
        {
            if (parentFacInfoDataID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var FACInfoData = await db.FACInfoDatas.FindAsync(parentFacInfoDataID);
            if (FACInfoData == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var PosLOCExtraData = FACInfoData.POSLOCExtraData;

            if (PosLOCExtraData == null)
            {
                ViewBag.FACInfoDataID = parentFacInfoDataID;
                PosLOCExtraData = new POSLOCExtraData();
            }
            string formSent = PosLOCExtraData.Forms_sent.Aggregate("", (current, item) => current+"," + " " + db.FormsDicts.Find(item.FormsDict_FormsID).FormName);
            ViewBag.FormSent = formSent;
            return View(PosLOCExtraData);
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Create
        public ActionResult Create(int? facInfoData)
        {
            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName");
            return View();
        }

        // POST: PlaceOfServices/POSLOCExtraDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FACInfoData_FACInfoDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Ancillary_outpatient_services,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder")] POSLOCExtraData pOSLOCExtraData)
        {
            if (ModelState.IsValid)
            {
                db.POSLOCExtraDatas.Add(pOSLOCExtraData);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName", pOSLOCExtraData.FACInfoData_FACInfoDataID);
            return View(pOSLOCExtraData);
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            POSLOCExtraData pOSLOCExtraData = await db.POSLOCExtraDatas.FindAsync(id);
            if (pOSLOCExtraData == null)
            {
                return HttpNotFound();
            }
            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName", pOSLOCExtraData.FACInfoData_FACInfoDataID);
            return View(pOSLOCExtraData);
        }

        // POST: PlaceOfServices/POSLOCExtraDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FACInfoData_FACInfoDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Ancillary_outpatient_services,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder")] POSLOCExtraData pOSLOCExtraData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pOSLOCExtraData).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FACInfoData_FACInfoDataID = new SelectList(db.FACInfoDatas, "FACInfoDataID", "DocProviderName", pOSLOCExtraData.FACInfoData_FACInfoDataID);
            return View(pOSLOCExtraData);
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            POSLOCExtraData pOSLOCExtraData = await db.POSLOCExtraDatas.FindAsync(id);
            if (pOSLOCExtraData == null)
            {
                return HttpNotFound();
            }
            return View(pOSLOCExtraData);
        }

        // POST: PlaceOfServices/POSLOCExtraDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            POSLOCExtraData pOSLOCExtraData = await db.POSLOCExtraDatas.FindAsync(id);
            db.POSLOCExtraDatas.Remove(pOSLOCExtraData);
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
