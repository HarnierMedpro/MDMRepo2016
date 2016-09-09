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
    public class ContactsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/Contacts
        public ActionResult Index()
        {
            ViewData["ContactType"] = db.ContactTypes.OrderBy(x => x.ContactType_Name).Select(x => new VMContactType {ContactTypeID = x.ContactTypeID, ContactType_Name = x.ContactType_Name});
            return View();
        }

        public ActionResult Read_Contact([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.Contacts.OrderBy(x => x.LName).Include(x => x.ContactType);
            return Json(result.ToDataSourceResult(request, l => new VMContact
            {
                ContactID = l.ContactID,
                ContactTypeID = l.ContactTypeID,
                FName = l.FName,
                LName = l.LName,
                Email = l.Email,
                PhoneNumber = l.PhoneNumber
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Contact([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactID,ContactTypeID,FName,LName,Email,PhoneNumber")] VMContact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var toStore = new Contact
                    {
                        ContactTypeID = contact.ContactTypeID,
                        FName = contact.FName,
                        LName = contact.LName,
                        PhoneNumber = contact.PhoneNumber,
                        Email = contact.Email
                    };
                    db.Contacts.Add(toStore);
                    await db.SaveChangesAsync();
                    contact.ContactID = toStore.ContactID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        TableName = "Contact",
                        ModelPKey = toStore.ContactID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "ContactTypeID", NewValue = toStore.ContactTypeID.ToString()},
                            new TableInfo{Field_ColumName = "FName", NewValue = toStore.FName},
                            new TableInfo{Field_ColumName = "LName", NewValue = toStore.LName},
                            new TableInfo{Field_ColumName = "PhoneNumber", NewValue = toStore.PhoneNumber},
                            new TableInfo{Field_ColumName = "Email", NewValue = toStore.Email}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                }
                catch (Exception)
                {
                   ModelState.AddModelError("","Something failed. Please try again!");
                   return Json(new[] {contact}.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Contact([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "ContactID,ContactTypeID,FName,LName,Email,PhoneNumber")] VMContact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.Contacts.FindAsync(contact.ContactID);
                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDb.ContactTypeID != contact.ContactTypeID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ContactTypeID", OldValue = storedInDb.ContactTypeID.ToString(), NewValue = contact.ContactTypeID.ToString()});
                        storedInDb.ContactTypeID = contact.ContactTypeID;
                    }
                    if (storedInDb.FName != contact.FName)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "FName", OldValue = storedInDb.FName, NewValue = contact.FName });
                        storedInDb.ContactTypeID = contact.ContactTypeID;
                    }
                    if (storedInDb.LName != contact.LName)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "LName", OldValue = storedInDb.LName, NewValue = contact.LName });
                        storedInDb.LName = contact.LName;
                    }
                    if (storedInDb.PhoneNumber != contact.PhoneNumber)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PhoneNumber", OldValue = storedInDb.PhoneNumber, NewValue = contact.PhoneNumber });
                        storedInDb.PhoneNumber = contact.PhoneNumber;
                    }
                    if (storedInDb.Email != contact.Email)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Email", OldValue = storedInDb.Email, NewValue = contact.Email });
                        storedInDb.Email = contact.Email;
                    }

                    db.Contacts.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        TableName = "Contact",
                        ModelPKey = storedInDb.ContactID,
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
        }

        //public async Task<ActionResult> Index()
        //{
        //    var contacts = db.Contacts.Include(c => c.ContactType);
        //    return View(await contacts.ToListAsync());
        //}

        // GET: PlaceOfServices/Contacts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: PlaceOfServices/Contacts/Create
        public ActionResult Create()
        {
            ViewBag.ContactTypeID = new SelectList(db.ContactTypes, "ContactTypeID", "ContactType_Name");
            return View();
        }

        // POST: PlaceOfServices/Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,ContactTypeID")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ContactTypeID = new SelectList(db.ContactTypes, "ContactTypeID", "ContactType_Name", contact.ContactTypeID);
            return View(contact);
        }

        // GET: PlaceOfServices/Contacts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContactTypeID = new SelectList(db.ContactTypes, "ContactTypeID", "ContactType_Name", contact.ContactTypeID);
            return View(contact);
        }

        // POST: PlaceOfServices/Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,ContactTypeID")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ContactTypeID = new SelectList(db.ContactTypes, "ContactTypeID", "ContactType_Name", contact.ContactTypeID);
            return View(contact);
        }

        // GET: PlaceOfServices/Contacts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: PlaceOfServices/Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Contact contact = await db.Contacts.FindAsync(id);
            db.Contacts.Remove(contact);
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
