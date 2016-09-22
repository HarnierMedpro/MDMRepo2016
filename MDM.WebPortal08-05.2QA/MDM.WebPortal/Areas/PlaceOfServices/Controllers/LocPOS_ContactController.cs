//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using MDM.WebPortal.Areas.AudiTrails.Controllers;
//using MDM.WebPortal.Areas.AudiTrails.Models;
//using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
//using MDM.WebPortal.Models.FromDB;
//using Microsoft.AspNet.Identity;

//namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
//{
//    public class LocPOS_ContactController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();

//        // GET: PlaceOfServices/LocPOS_Contact
//        public async Task<ActionResult> Index()
//        {
//            var locPOS_Contact = db.LocPOS_Contact.Include(l => l.LocationsPOS).Include(l => l.Contact);
//            return View(await locPOS_Contact.ToListAsync());
//        }

//        // GET: PlaceOfServices/LocPOS_Contact/Details/5
//        public async Task<ActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            LocPOS_Contact locPOS_Contact = await db.LocPOS_Contact.FindAsync(id);
//            if (locPOS_Contact == null)
//            {
//                return HttpNotFound();
//            }
//            return View(locPOS_Contact);
//        }

//        // GET: PlaceOfServices/LocPOS_Contact/Create
//        public async Task<ActionResult> Create(int? LocationPOSID)
//        {
//            if (LocationPOSID != null && await db.LocationsPOS.FindAsync(LocationPOSID) != null)
//            {
//                ViewBag.Facility_DBs_IDPK = LocationPOSID;
//                VMLocPOS_Contact toView = new VMLocPOS_Contact{Facility_DBs_IDPK = LocationPOSID.Value};
//                return View(toView);
//            }
//           return RedirectToAction("Index", "Error", new {area = "Error"});
//        }

//        // POST: PlaceOfServices/LocPOS_Contact/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create([Bind(Include = "Facility_DBs_IDPK")] VMLocPOS_Contact locPOSContact, params int[] Contacts)
//        {
//            if (ModelState.IsValid && Contacts != null && await db.LocationsPOS.FindAsync(locPOSContact.Facility_DBs_IDPK) != null)
//            {
//                try
//                {
//                    var currentContacts = db.LocPOS_Contact.Where(x => x.Facility_DBs_IDPK == locPOSContact.Facility_DBs_IDPK).Select(x => x.ContactID).ToArray();
//                    var contactToStore = Contacts.Except(currentContacts).ToArray();

//                    var posContacts = contactToStore.Select(cnt => new LocPOS_Contact { Facility_DBs_IDPK = locPOSContact.Facility_DBs_IDPK, ContactID = cnt }).ToList();
//                    db.LocPOS_Contact.AddRange(posContacts);
//                    await db.SaveChangesAsync();

//                    foreach (var item in posContacts)
//                    {
//                        AuditToStore auditLog = new AuditToStore
//                        {
//                             UserLogons = User.Identity.GetUserName(),
//                             AuditDateTime = DateTime.Now,
//                             ModelPKey = item.LocPOSContactID,
//                             TableName = "ProvidersInGrps",
//                             AuditAction = "I",
//                             tableInfos = new List<TableInfo>
//                               {
//                                new TableInfo{Field_ColumName = "Facility_DBs_IDPK", NewValue = item.Facility_DBs_IDPK.ToString()},
//                                new TableInfo{Field_ColumName = "ContactID", NewValue = item.ContactID.ToString()}
//                               }
//                        };

//                        new AuditLogRepository().AddAuditLogs(auditLog);
//                    }

//                    return RedirectToAction("Index", "LocationsPOS", new {area = "PlaceOfServices"});
//                }
//                catch (Exception)
//                {
//                    TempData["Error"] = "Something Failed. Please try again!";
//                    return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
//                }
               
//            }
//            if (Contacts == null)
//            {
//                TempData["Error"] = "You have to select at least one CONTACT. Please try again!";
//            }
//            if (db.LocationsPOS.Find(locPOSContact.Facility_DBs_IDPK) == null)
//            {
//                ViewBag.Doc = "This POS is not stored in DB. Please try again!";
//            }
//            return RedirectToAction("Index", "LocationsPOS", new {area = "PlaceOfServices"});
//        }

//        // GET: PlaceOfServices/LocPOS_Contact/Edit/5
//        public async Task<ActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            LocPOS_Contact locPOS_Contact = await db.LocPOS_Contact.FindAsync(id);
//            if (locPOS_Contact == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.Facility_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", locPOS_Contact.Facility_DBs_IDPK);
//            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "FName", locPOS_Contact.ContactID);
//            return View(locPOS_Contact);
//        }

//        // POST: PlaceOfServices/LocPOS_Contact/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Edit([Bind(Include = "Facility_DBs_IDPK,ContactID,LocPOSContactID")] LocPOS_Contact locPOS_Contact)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(locPOS_Contact).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//                return RedirectToAction("Index");
//            }
//            ViewBag.Facility_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", locPOS_Contact.Facility_DBs_IDPK);
//            ViewBag.ContactID = new SelectList(db.Contacts, "ContactID", "FName", locPOS_Contact.ContactID);
//            return View(locPOS_Contact);
//        }

//        // GET: PlaceOfServices/LocPOS_Contact/Delete/5
//        public async Task<ActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            LocPOS_Contact locPOS_Contact = await db.LocPOS_Contact.FindAsync(id);
//            if (locPOS_Contact == null)
//            {
//                return HttpNotFound();
//            }
//            return View(locPOS_Contact);
//        }

//        // POST: PlaceOfServices/LocPOS_Contact/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> DeleteConfirmed(int id)
//        {
//            LocPOS_Contact locPOS_Contact = await db.LocPOS_Contact.FindAsync(id);
//            db.LocPOS_Contact.Remove(locPOS_Contact);
//            await db.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
