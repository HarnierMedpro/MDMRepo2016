using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class Corp_OwnerController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Create(int? corpID)
        {
            if (corpID == null)
            {
                return RedirectToAction("Index","Error", new{area="BadRequest"});
            }
            var corporation = await db.CorporateMasterLists.FindAsync(corpID);
            if (corporation == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            VMCorp_Contact toView = new VMCorp_Contact{corpID = corpID.Value};
            ViewBag.Corporation = corpID;
            return View(toView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "corpID")] VMCorp_Contact corpContact,
            params int[] corpCnt)
        {
            if (ModelState.IsValid && corpCnt.Any() && await db.CorporateMasterLists.FindAsync(corpContact.corpID) != null)
            {
                if (!corpCnt.Any())
                {
                    TempData["Error"] = "You have to select at least one CONTACT. Please try again!";
                    return RedirectToAction("Index", "CorporateMasterLists", new { area = "Credentials" });
                }
                if (await db.CorporateMasterLists.FindAsync(corpContact.corpID) == null)
                {
                    TempData["Error"] = "This Corporation is not stored in DB. Please try again!";
                    return RedirectToAction("Index", "CorporateMasterLists", new { area = "Credentials" });
                }
                try
                {
                    var currentContacts = db.Corp_Owner.Where(x => x.corpID == corpContact.corpID).Select(x => x.Contact_ContactID).ToArray();
                    var toStore = corpCnt.Except(currentContacts).Select(x => new Corp_Owner { corpID = corpContact.corpID, Contact_ContactID = x });

                    db.Corp_Owner.AddRange(toStore);
                    await db.SaveChangesAsync();

                   
                    var auditLog = toStore.Select(x => new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        TableName = "Corp_Owner",
                        ModelPKey = x.corpOwnerID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()},
                            new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = x.Contact_ContactID.ToString()}
                        }
                    }).ToList();

                    new AuditLogRepository().SaveLogs(auditLog);

                    return RedirectToAction("Index", "CorporateMasterLists", new { area = "Credentials" });
                }
                catch (Exception)
                {
                    TempData["Error"] = "Something failed. Please try again!";
                    return RedirectToAction("Index", "CorporateMasterLists", new { area = "Credentials" });
                }
            }
            TempData["Error"] = "Check your Data. Please try again!";
            return RedirectToAction("Index", "CorporateMasterLists", new { area = "Credentials" });
           
        }

        public async Task<ActionResult> Save_MultipleCorpContacts(int corpID, params int[] Contacts)
        {
            try
            {
                if (corpID == 0 || !Contacts.Any())
                {
                    return Json(new Corp_Owner(), JsonRequestBehavior.AllowGet);
                }
                var currentContacts = db.Corp_Owner.Where(x => x.corpID == corpID).Select(x => x.Contact_ContactID).ToArray();
                var toStore = Contacts.Except(currentContacts).Select(x => new Corp_Owner { corpID = corpID, Contact_ContactID = x });
                
                db.Corp_Owner.AddRange(toStore);
                await db.SaveChangesAsync();

               
                var auditLog = toStore.Select(x => new AuditToStore
                {
                    UserLogons = User.Identity.GetUserName(),
                    AuditDateTime = DateTime.Now,
                    AuditAction = "I",
                    TableName = "Corp_Owner",
                    ModelPKey = x.corpOwnerID,
                    tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()},
                            new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = x.Contact_ContactID.ToString()}
                        }
                }).ToList();

                new AuditLogRepository().SaveLogs(auditLog);

            }
            catch (Exception)
            {
                return Json(new Corp_Owner(), JsonRequestBehavior.AllowGet);
            }
            return Json(Contacts.Select(x => new Corp_Owner { corpID = corpID, Contact_ContactID = x }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Save_MultipleCorpOwners(int corpID, params int[] Contacts)
        {
            try
            {
                if (corpID == 0 || !Contacts.Any())
                {
                    return Json(new Corp_Owner(), JsonRequestBehavior.AllowGet);
                }
                var allSystemOwners = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                   .Where(ty => ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                   .Select(tv => tv.Contact);

                var allContactsOfThisCorp = db.Corp_Owner.Include(x => x.Contact).Include(y => y.CorporateMasterList).Where(z => z.corpID == corpID).Select(f => f.Contact);

                var currentCorpOwners = allSystemOwners.Intersect(allContactsOfThisCorp).Select(x => x.ContactID).ToArray();

                //var currentContacts = db.Corp_Owner.Where(x => x.corpID == corpID).Select(x => x.Contact_ContactID).ToArray();

                var toStore = Contacts.Except(currentCorpOwners).Select(x => new Corp_Owner { corpID = corpID, Contact_ContactID = x });

                db.Corp_Owner.AddRange(toStore);
                await db.SaveChangesAsync();

                var url = Request.RawUrl;
                var auditLog = toStore.Select(x => new AuditToStore
                {
                    UserLogons = User.Identity.GetUserName(),
                    AuditDateTime = DateTime.Now,
                    AuditAction = "I",
                    TableName = "Corp_Owner",
                    ModelPKey = x.corpOwnerID,
                    tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()},
                            new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = x.Contact_ContactID.ToString()}
                        }
                }).ToList();

                new AuditLogRepository().SaveLogs(auditLog);

            }
            catch (Exception)
            {
                return Json(new Corp_Owner(), JsonRequestBehavior.AllowGet);
            }
            return Json(Contacts.Select(x => new Corp_Owner { corpID = corpID, Contact_ContactID = x }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_CorpContact([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,active, ContactTypes")] VMCorpContact contact, int ParentID)
        {
            if (ModelState.IsValid && contact.ContactTypes.Any())
            {
                if (await db.CorporateMasterLists.FindAsync(ParentID) == null)
                {
                    ModelState.AddModelError("", "You are trying to associate a new Contact to a Corporation that does not exist anymore. Please try again!");
                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                }
                if (await db.Contacts.AnyAsync(x => x.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                }

                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var contactToStore = new Contact
                        {
                            FName = contact.FName,
                            LName = contact.LName,
                            PhoneNumber = contact.PhoneNumber,
                            Email = contact.Email,
                            active = contact.active
                        };
                        db.Contacts.Add(contactToStore);
                        await db.SaveChangesAsync();
                        contact.ContactID = contactToStore.ContactID;

                        var contactType_Contact = contact.ContactTypes.Select(x => new ContactType_Contact{Contact_ContactID = contactToStore.ContactID, ContactType_ContactTypeID = x.ContactTypeID});
                        db.ContactType_Contact.AddRange(contactType_Contact);

                        db.Corp_Owner.Add(new Corp_Owner{Contact_ContactID = contactToStore.ContactID, corpID = ParentID});
                        await db.SaveChangesAsync();
                        dbTransaction.Commit();

                        var url = Request.RawUrl;
                        List<AuditToStore> auditLog = new List<AuditToStore>
                        {
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "Contacts",
                                ModelPKey = contactToStore.ContactID,
                                AuditAction = "I",
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "FName", NewValue = contactToStore.FName},
                                    new TableInfo{Field_ColumName = "LName", NewValue = contactToStore.LName},
                                    new TableInfo{Field_ColumName = "PhoneNumber", NewValue = contactToStore.PhoneNumber},
                                    new TableInfo{Field_ColumName = "Email", NewValue = contactToStore.Email},
                                    new TableInfo{Field_ColumName = "active", NewValue = contactToStore.active.ToString()}
                                }
                            }
                        };

                        var logToInsert = contactType_Contact.Select(x => new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "ContactType_Contact",
                            AuditAction = "I",
                            ModelPKey = x.ContactTypeContactID,
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = x.Contact_ContactID.ToString()},
                                new TableInfo{Field_ColumName = "ContactType_ContactTypeID", NewValue = x.ContactType_ContactTypeID.ToString()}
                            }
                        });

                        auditLog.AddRange(logToInsert);
                        new AuditLogRepository().SaveLogs(auditLog);


                    }
                    catch (Exception)
                    {
                       dbTransaction.Rollback();
                       ModelState.AddModelError("", "Something failed. Please try again!");
                       return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_CorpOwner([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,active")] VMContact contact, int ParentID)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.CorporateMasterLists.FindAsync(ParentID) == null)
                        {
                            ModelState.AddModelError("", "You are trying to associate a new Owner to a Corporation that does not exist anymore. Please try again!");
                            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                        }
                        if (await db.Contacts.AnyAsync(x => x.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ModelState.AddModelError("","Duplicate Data. Please try again!");
                            return Json(new[] {contact}.ToDataSourceResult(request, ModelState));
                        }
                       
                        var contactToStore = new Contact
                        {
                            FName = contact.FName,
                            LName = contact.LName,
                            PhoneNumber = contact.PhoneNumber,
                            Email = contact.Email,
                            active = contact.active
                        };
                        db.Contacts.Add(contactToStore);
                        await db.SaveChangesAsync();
                        contact.ContactID = contactToStore.ContactID;

                        ContactType_Contact cntTypeCntToStore; 

                        if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var contactTypeID = db.ContactTypes.First(x => x.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase)).ContactTypeID;

                            cntTypeCntToStore = new ContactType_Contact
                            {
                                Contact_ContactID = contactToStore.ContactID,
                                ContactType_ContactTypeID = contactTypeID
                            };
                            db.ContactType_Contact.Add(cntTypeCntToStore);
                        }
                        else
                        {
                            var newContactType = new ContactType
                            {
                                ContactType_Name = "OWNER",
                                ContactLevel = "CORPORATION"
                            };
                            db.ContactTypes.Add(newContactType);
                            await db.SaveChangesAsync();

                            cntTypeCntToStore = new ContactType_Contact
                            {
                                Contact_ContactID = contactToStore.ContactID,
                                ContactType_ContactTypeID = newContactType.ContactTypeID
                            };
                            db.ContactType_Contact.Add(cntTypeCntToStore);
                        }

                        var corpOwnerToStore = new Corp_Owner
                        {
                            corpID = ParentID,
                            Contact_ContactID = contactToStore.ContactID
                        };

                        db.Corp_Owner.Add(corpOwnerToStore);

                        await db.SaveChangesAsync();

                        dbTransaction.Commit();

                        var url = Request.RawUrl;
                        List<AuditToStore> auditLog = new List<AuditToStore>
                        {
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now, 
                                TableName = "Contacts",
                                AuditAction = "I",
                                ModelPKey = contactToStore.ContactID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "FName", NewValue = contactToStore.FName},
                                    new TableInfo{Field_ColumName = "LName", NewValue = contactToStore.LName},
                                    new TableInfo{Field_ColumName = "PhoneNumber", NewValue = contactToStore.PhoneNumber},
                                    new TableInfo{Field_ColumName = "Email", NewValue = contactToStore.Email},
                                    new TableInfo{Field_ColumName = "active", NewValue = contactToStore.active.ToString()},
                                }
                            },
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now, 
                                TableName = "ContactType_Contact",
                                AuditAction = "I",
                                ModelPKey = cntTypeCntToStore.ContactTypeContactID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = cntTypeCntToStore.Contact_ContactID.ToString()},
                                    new TableInfo{Field_ColumName = "ContactType_ContactTypeID", NewValue = cntTypeCntToStore.ContactType_ContactTypeID.ToString()}
                                }
                            },
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now, 
                                TableName = "Corp_Owner",
                                AuditAction = "I",
                                ModelPKey = corpOwnerToStore.corpOwnerID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = corpOwnerToStore.Contact_ContactID.ToString()},
                                    new TableInfo{Field_ColumName = "corpID", NewValue = corpOwnerToStore.corpID.ToString()}
                                }
                            }
                        };

                        new AuditLogRepository().SaveLogs(auditLog);

                    }
                    catch (Exception)
                    {
                       dbTransaction.Rollback();
                       ModelState.AddModelError("", "Something Failed. Please try again!");
                       return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_CorpOwner([DataSourceRequest] DataSourceRequest request,
             [Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,active")] VMContact contact, int ParentID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CorporateMasterLists.FindAsync(ParentID) == null)
                    {
                        ModelState.AddModelError("", "You are trying to edit the Owner Info of a Corporation that does not exist anymore. Please try again!");
                        return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                    }
                    if (await db.Contacts.AnyAsync(x => x.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                    }

                    var contactStoredInDb = await db.Contacts.FindAsync(contact.ContactID);
                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (!contactStoredInDb.LName.Equals(contact.LName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "LName", OldValue = contactStoredInDb.LName, NewValue = contact.LName});
                        contactStoredInDb.LName = contact.LName;
                    }
                    if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "FName", OldValue = contactStoredInDb.FName, NewValue = contact.FName });
                        contactStoredInDb.FName = contact.FName;
                    }
                    if (contactStoredInDb.PhoneNumber != contact.PhoneNumber)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PhoneNumber", OldValue = contactStoredInDb.PhoneNumber, NewValue = contact.PhoneNumber });
                        contactStoredInDb.PhoneNumber = contact.PhoneNumber;
                    }
                    if (!contactStoredInDb.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Email", OldValue = contactStoredInDb.Email, NewValue = contact.Email });
                        contactStoredInDb.Email = contact.Email;
                    }
                    if (contactStoredInDb.active != contact.active)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "active", OldValue = contactStoredInDb.active.ToString(), NewValue = contact.active.ToString() });
                        contactStoredInDb.active = contact.active;
                    }

                    db.Contacts.Attach(contactStoredInDb);
                    db.Entry(contactStoredInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        TableName = "Contacts",
                        ModelPKey = contactStoredInDb.ContactID,
                        AuditAction = "U",
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
