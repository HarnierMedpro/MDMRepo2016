//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using MDM.WebPortal.Areas.AudiTrails.Controllers;
//using MDM.WebPortal.Areas.AudiTrails.Models;
//using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
//using MDM.WebPortal.Data_Annotations;
//using MDM.WebPortal.Models.FromDB;
//using Microsoft.AspNet.Identity;

//namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
//{
//    [SetPermissions]
//    public class ContactsController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();

//        // GET: PlaceOfServices/Contacts
//        public ActionResult Index()
//        {
//            ViewData["ContactType"] = db.ContactTypes.OrderBy(x => x.ContactType_Name).Select(x => new VMContactType {ContactTypeID = x.ContactTypeID, ContactType_Name = x.ContactType_Name});
//            return View();
//        }

//        public ActionResult Read_Contact([DataSourceRequest] DataSourceRequest request)
//        {
//            var result = db.Contacts.OrderBy(x => x.LName).Include(x => x.ContactType);
//            return Json(result.ToDataSourceResult(request, l => new VMContact
//            {
//                ContactID = l.ContactID,
//                ContactTypeID = l.ContactTypeID,
//                FName = l.FName,
//                LName = l.LName,
//                Email = l.Email,
//                PhoneNumber = l.PhoneNumber
//            }), JsonRequestBehavior.AllowGet);
//        }

//        public async Task<ActionResult> Create_Contact([DataSourceRequest] DataSourceRequest request,
//            [Bind(Include = "ContactID,ContactTypeID,FName,LName,Email,PhoneNumber")] VMContact contact)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var toStore = new Contact
//                    {
//                        ContactTypeID = contact.ContactTypeID,
//                        FName = contact.FName,
//                        LName = contact.LName,
//                        PhoneNumber = contact.PhoneNumber,
//                        Email = contact.Email
//                    };
//                    db.Contacts.Add(toStore);
//                    await db.SaveChangesAsync();
//                    contact.ContactID = toStore.ContactID;

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "I",
//                        TableName = "Contact",
//                        ModelPKey = toStore.ContactID,
//                        tableInfos = new List<TableInfo>
//                        {
//                            new TableInfo{Field_ColumName = "ContactTypeID", NewValue = toStore.ContactTypeID.ToString()},
//                            new TableInfo{Field_ColumName = "FName", NewValue = toStore.FName},
//                            new TableInfo{Field_ColumName = "LName", NewValue = toStore.LName},
//                            new TableInfo{Field_ColumName = "PhoneNumber", NewValue = toStore.PhoneNumber},
//                            new TableInfo{Field_ColumName = "Email", NewValue = toStore.Email}
//                        }
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);

//                }
//                catch (Exception)
//                {
//                   ModelState.AddModelError("","Something failed. Please try again!");
//                   return Json(new[] {contact}.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//        }

//        public async Task<ActionResult> Update_Contact([DataSourceRequest] DataSourceRequest request,
//           [Bind(Include = "ContactID,ContactTypeID,FName,LName,Email,PhoneNumber")] VMContact contact)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var storedInDb = await db.Contacts.FindAsync(contact.ContactID);
//                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                    if (storedInDb.ContactTypeID != contact.ContactTypeID)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ContactTypeID", OldValue = storedInDb.ContactTypeID.ToString(), NewValue = contact.ContactTypeID.ToString()});
//                        storedInDb.ContactTypeID = contact.ContactTypeID;
//                    }
//                    if (storedInDb.FName != contact.FName)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "FName", OldValue = storedInDb.FName, NewValue = contact.FName });
//                        storedInDb.ContactTypeID = contact.ContactTypeID;
//                    }
//                    if (storedInDb.LName != contact.LName)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "LName", OldValue = storedInDb.LName, NewValue = contact.LName });
//                        storedInDb.LName = contact.LName;
//                    }
//                    if (storedInDb.PhoneNumber != contact.PhoneNumber)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PhoneNumber", OldValue = storedInDb.PhoneNumber, NewValue = contact.PhoneNumber });
//                        storedInDb.PhoneNumber = contact.PhoneNumber;
//                    }
//                    if (storedInDb.Email != contact.Email)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Email", OldValue = storedInDb.Email, NewValue = contact.Email });
//                        storedInDb.Email = contact.Email;
//                    }

//                    db.Contacts.Attach(storedInDb);
//                    db.Entry(storedInDb).State = EntityState.Modified;
//                    await db.SaveChangesAsync();

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "I",
//                        TableName = "Contact",
//                        ModelPKey = storedInDb.ContactID,
//                        tableInfos = tableColumnInfos
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);

//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something failed. Please try again!");
//                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//        }

//        public ActionResult GetContacts([DataSourceRequest] DataSourceRequest request, int? LocationPOSID)
//        {
//            var allContacts = db.Contacts.OrderBy(x => x.LName).ToList();
//            var result = db.Contacts.Select(x => new VMContactInfo {ContactID = x.ContactID, FullName = x.LName + "," + " " + x.FName}).OrderBy(x => x.FullName).ToList();
//            //var result = db.Contacts.Select(x => new VMContact
//            //{
//            //    ContactID = x.ContactID,
//            //    ContactTypeID = x.ContactTypeID,
//            //    FName = x.FName,
//            //    LName = x.LName,
//            //    Email = x.Email,
//            //    PhoneNumber = x.PhoneNumber
//            //}).ToList();
//            if (LocationPOSID != null)
//            {
//                var contactsOfCurrentLocationsPos = db.LocPOS_Contact.Include(x => x.Contact).Where(x => x.Facility_DBs_IDPK == LocationPOSID).Select(x => x.Contact);
//                result = allContacts.Except(contactsOfCurrentLocationsPos).Select(x => new VMContactInfo { ContactID = x.ContactID, FullName = x.LName + "," + " " + x.FName }).OrderBy(x => x.FullName).ToList();
//                //result = allContacts.Except(contactsOfCurrentLocationsPos).Select(x => new VMContact
//                //{
//                //    ContactID = x.ContactID,
//                //    ContactTypeID = x.ContactTypeID,
//                //    FName = x.FName,
//                //    LName = x.LName,
//                //    Email = x.Email,
//                //    PhoneNumber = x.PhoneNumber
//                //}).OrderBy(x => x.LName).ToList();
//            }
//            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
//        }

//        public async Task<ActionResult> Create_NewPOSContact([DataSourceRequest] DataSourceRequest request,
//            [Bind(Include = "ContactID,ContactTypeID,FName,LName,Email,PhoneNumber")] VMContact contact, int ParentID)
//        {
//            if (ModelState.IsValid)
//            {
//                if (ParentID <= 0 && await db.LocationsPOS.FindAsync(ParentID) == null)
//                {
//                    ModelState.AddModelError("", "You are trying to assign a Contact to a POS that is no more in our system. Please try again!");
//                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//                }
//                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        var contactToStore = new Contact
//                        {
//                            ContactTypeID = contact.ContactTypeID,
//                            FName = contact.FName,
//                            LName = contact.LName,
//                            Email = contact.Email,
//                            PhoneNumber = contact.PhoneNumber
//                        };

//                        db.Contacts.Add(contactToStore);
//                        await db.SaveChangesAsync();
//                        contact.ContactID = contactToStore.ContactID;

//                        LocPOS_Contact pos_Contact = new LocPOS_Contact { ContactID = contact.ContactID, Facility_DBs_IDPK = ParentID };
//                        db.LocPOS_Contact.Add(pos_Contact);
//                        await db.SaveChangesAsync();

//                        //commit transaction
//                        dbTransaction.Commit();
//                    }
//                    catch (Exception)
//                    {
//                        //Rollback transaction if exception occurs
//                        dbTransaction.Rollback();
//                        ModelState.AddModelError("", "Something failed. Please try again!");
//                        return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//                    }
//                }
//            }
//            return Json(new[] {contact}.ToDataSourceResult(request, ModelState));
//        }

//        public async Task<ActionResult> Update_POSContact([DataSourceRequest] DataSourceRequest request,
//           [Bind(Include = "ContactID,ContactTypeID,FName,LName,Email,PhoneNumber")] VMContact contact, int ParentID)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (ParentID <= 0 && await db.LocationsPOS.FindAsync(ParentID) == null)
//                    {
//                        ModelState.AddModelError("", "You are trying to update a Contact assigned to a POS that is no more in our system. Please try again!");
//                        return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//                    }

//                    var storedInDb = await db.Contacts.FindAsync(contact.ContactID);
//                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                    if (storedInDb.ContactTypeID != contact.ContactTypeID)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ContactTypeID", OldValue = storedInDb.ContactTypeID.ToString(), NewValue = contact.ContactTypeID.ToString() });
//                        storedInDb.ContactTypeID = contact.ContactTypeID;
//                    }
//                    if (storedInDb.FName != contact.FName)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "FName", OldValue = storedInDb.FName, NewValue = contact.FName });
//                        storedInDb.ContactTypeID = contact.ContactTypeID;
//                    }
//                    if (storedInDb.LName != contact.LName)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "LName", OldValue = storedInDb.LName, NewValue = contact.LName });
//                        storedInDb.LName = contact.LName;
//                    }
//                    if (storedInDb.PhoneNumber != contact.PhoneNumber)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PhoneNumber", OldValue = storedInDb.PhoneNumber, NewValue = contact.PhoneNumber });
//                        storedInDb.PhoneNumber = contact.PhoneNumber;
//                    }
//                    if (storedInDb.Email != contact.Email)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Email", OldValue = storedInDb.Email, NewValue = contact.Email });
//                        storedInDb.Email = contact.Email;
//                    }

//                    db.Contacts.Attach(storedInDb);
//                    db.Entry(storedInDb).State = EntityState.Modified;
//                    await db.SaveChangesAsync();

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "I",
//                        TableName = "Contact",
//                        ModelPKey = storedInDb.ContactID,
//                        tableInfos = tableColumnInfos
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);
//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something Failed. Please try again!");
//                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
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
