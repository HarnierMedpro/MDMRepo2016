using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class ContactsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            ViewBag.ContactType = db.ContactTypes.Select(x => new VMContactType { ContactTypeID = x.ContactTypeID, ContactType_Name = x.ContactType_Name, ContactLevel = x.ContactLevel }).OrderBy(x => x.ContactType_Name).Any();
            return View();
        }

        public ActionResult Read_Contact([DataSourceRequest] DataSourceRequest request)
        {
            /*En caso de que me de problemas puedo leer los contactos desde ContacType_Contact Table*/
            var result = db.Contacts.Include(x => x.ContactType_Contact).Select(x => new VMContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active,
                ContactTypes = x.ContactType_Contact.Select(y =>
                            new VMContactType
                            {
                                ContactTypeID = y.ContactType_ContactTypeID,
                                ContactType_Name = y.ContactType.ContactType_Name,
                                ContactLevel = y.ContactType.ContactLevel
                            })
            });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_Owners([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            var result = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                    .Where(ty => ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                    .Select(tv => tv.Contact);
            
            if (corpID != null && corpID > 0)
            {
                var allContactsOfThisCorp = db.Corp_Owner.Include(x => x.Contact).Include(y => y.CorporateMasterList).Where(z => z.corpID == corpID).Select(f => f.Contact);
                var currentCorpOwners = result.Intersect(allContactsOfThisCorp);
                result = result.Except(currentCorpOwners);
            }

            return Json(result.ToList().ToDataSourceResult(request, x => new VMContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active
            }));
        }

        public ActionResult Read_CorpContacts([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            /*Obtener todos los contactos a nivel de corporacion excepto los de tipo Owner*/
            var result = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                           .Where(ty => ty.ContactType.ContactLevel.Equals("corporation", StringComparison.InvariantCultureIgnoreCase) && 
                                  ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase) == false)
                           .Select(tv => tv.Contact).Distinct();

            if (corpID != null && corpID > 0)
            {
                var allContactsOfThisCorp = db.Corp_Owner.Include(x => x.Contact).Where(x => x.corpID == corpID).Select(x => x.Contact);
                var allNonOwnerOfThisCorp = result.Intersect(allContactsOfThisCorp);
                result = result.Except(allNonOwnerOfThisCorp);
            }
            
            return Json(result.ToDataSourceResult(request, x => new VMContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active
            }));
        }

        public ActionResult Read_POSContacts([DataSourceRequest] DataSourceRequest request, int? masterPOSID)
        {
            var result = new List<Contact>();
            if (masterPOSID != null && masterPOSID > 0)
            {
                result = db.MasterPOS_Contact.Include(c => c.Contact).Include(p => p.MasterPOS).Where(p => p.MasterPOS_MasterPOSID == masterPOSID).Select(c => c.Contact).ToList();
            }
            return Json(result.Where(x => x.active).ToDataSourceResult(request, x => new VMPosContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active
            }));
        }

        public ActionResult Read_AvailablePOSContacts([DataSourceRequest] DataSourceRequest request, int? masterPOSID)
        {
            var currentCntOfThisPos = db.MasterPOS_Contact.Include(c => c.Contact).Include(p => p.MasterPOS).Where(p => p.MasterPOS_MasterPOSID == masterPOSID).Select(c => c.Contact);
            var allPosCnts = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                                                   .Where(ty => ty.ContactType.ContactLevel.Equals("pos", StringComparison.InvariantCultureIgnoreCase))
                                                   .Select(tv => tv.Contact).Distinct();
            var avaliables = allPosCnts.Except(currentCntOfThisPos).ToList();
            return Json(avaliables.ToDataSourceResult(request, x => new VMContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active
            }));
        }

        public async Task<ActionResult> Create_Contact([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,active, ContactTypes")] VMContact contact)
        {
            if (ModelState.IsValid && contact.ContactTypes.Any())
            {
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
                            Email = contact.Email,
                            PhoneNumber = contact.PhoneNumber,
                            active = contact.active
                        };

                        db.Contacts.Add(contactToStore);
                        await db.SaveChangesAsync();
                        contact.ContactID = contactToStore.ContactID;

                        var contactTypeContact = contact.ContactTypes.Select(x => new ContactType_Contact { Contact_ContactID = contactToStore.ContactID, ContactType_ContactTypeID = x.ContactTypeID }).ToList();

                        db.ContactType_Contact.AddRange(contactTypeContact);

                        await db.SaveChangesAsync();

                        dbTransaction.Commit();

                        /*------------------AUDIT LOG SCENARIO------------------*/
                       
                        List<AuditToStore> toStore = new List<AuditToStore>{
                            new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    AuditAction = "I",
                                    TableName = "Contacts",
                                    ModelPKey = contactToStore.ContactID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "FName", NewValue = contactToStore.FName},
                                        new TableInfo{Field_ColumName = "LName", NewValue = contactToStore.LName},
                                        new TableInfo{Field_ColumName = "Email", NewValue = contactToStore.Email},
                                        new TableInfo{Field_ColumName = "PhoneNumber", NewValue = contactToStore.PhoneNumber},
                                        new TableInfo{Field_ColumName = "active", NewValue = contactToStore.active.ToString()}
                                    }
                                }
                        };

                        var auditToStore = contactTypeContact.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "I",
                                TableName = "ContactType_Contact",
                                ModelPKey = x.ContactTypeContactID,
                                tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo
                                        {
                                            Field_ColumName = "Contact_ContactID",
                                            NewValue = x.Contact_ContactID.ToString()
                                        },
                                        new TableInfo
                                        {
                                            Field_ColumName = "ContactType_ContactTypeID",
                                            NewValue = x.ContactType_ContactTypeID.ToString()
                                        }
                                    }
                            }).ToList();

                            toStore.AddRange(auditToStore);

                            new AuditLogRepository().SaveLogs(toStore);
                        /*------------------AUDIT LOG SCENARIO------------------*/
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

        public async Task<ActionResult> Update_Contact([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,active, ContactTypes")] VMContact contact)
        {
            if (ModelState.IsValid && contact.ContactTypes.Any())
            {
                if (await db.Contacts.AnyAsync(x => x.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase) && x.ContactID != contact.ContactID))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var contactStoredInDb = await db.Contacts.FindAsync(contact.ContactID);

                        List<AuditToStore> auditToStores = new List<AuditToStore>();
                        List<TableInfo> cnTableInfos = new List<TableInfo>();

                        if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.FName = contact.FName;
                            cnTableInfos.Add(new TableInfo { Field_ColumName = "FName", OldValue = contactStoredInDb.FName, NewValue = contact.FName});
                        }
                        if (!contactStoredInDb.LName.Equals(contact.LName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.LName = contact.LName;
                            cnTableInfos.Add(new TableInfo { Field_ColumName = "LName", OldValue = contactStoredInDb.LName, NewValue = contact.LName });
                        }
                        if (!contactStoredInDb.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.Email = contact.Email;
                            cnTableInfos.Add(new TableInfo { Field_ColumName = "Email", OldValue = contactStoredInDb.Email, NewValue = contact.Email });
                        }
                        if (!contactStoredInDb.PhoneNumber.Equals(contact.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.PhoneNumber = contact.PhoneNumber;
                            cnTableInfos.Add(new TableInfo { Field_ColumName = "PhoneNumber", OldValue = contactStoredInDb.PhoneNumber, NewValue = contact.PhoneNumber });
                        }
                        if (contactStoredInDb.active != contact.active)
                        {
                            contactStoredInDb.active = contact.active;
                            cnTableInfos.Add(new TableInfo { Field_ColumName = "active", OldValue = contactStoredInDb.active.ToString(), NewValue = contact.active.ToString() });
                        }

                        if (cnTableInfos.Any())
                        {
                           auditToStores.Add(new AuditToStore
                           {
                               UserLogons = User.Identity.GetUserName(),
                               AuditDateTime = DateTime.Now,
                               AuditAction = "U",
                               TableName = "Contacts",
                               ModelPKey = contactStoredInDb.ContactID,
                               tableInfos = cnTableInfos
                           }); 
                        }

                        var currentTypesOfThisContact = contactStoredInDb.ContactType_Contact.Select(y => y.ContactType_ContactTypeID).ToList();
                        var byParamContactType = contact.ContactTypes.Select(x => x.ContactTypeID).ToList();
                       
                        var newTypesToStore = byParamContactType.Except(currentTypesOfThisContact).ToList();
                        foreach (var type in newTypesToStore)
                        {
                            contactStoredInDb.ContactType_Contact.Add(new ContactType_Contact{ContactType_ContactTypeID = type, Contact_ContactID = contact.ContactID});
                        }

                        var typesToDelete = currentTypesOfThisContact.Except(byParamContactType).ToList();
                        foreach (var cntType in typesToDelete)
                        {
                            if (db.ContactTypes.Find(cntType).ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var removeCorpOwners = db.Corp_Owner.Where(x => x.Contact_ContactID == contact.ContactID).ToList();
                                db.Corp_Owner.RemoveRange(removeCorpOwners);


                                var dcorpOwnerAudit = removeCorpOwners.Select(x => new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "Corp_Owner",
                                    AuditAction = "D",
                                    ModelPKey = x.corpOwnerID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo
                                        {
                                            Field_ColumName = "Contact_ContactID",
                                            OldValue = x.Contact_ContactID.ToString()
                                        },
                                        new TableInfo
                                        {
                                            Field_ColumName = "corpID", 
                                            OldValue = x.corpID.ToString()
                                        }
                                    }
                                });
                                auditToStores.AddRange(dcorpOwnerAudit);


                            }
                            var deleteContactTypeContact = await db.ContactType_Contact.FirstOrDefaultAsync(x => x.ContactType_ContactTypeID == cntType && x.Contact_ContactID == contact.ContactID);
                            if (deleteContactTypeContact != null)
                            {
                                db.ContactType_Contact.Remove(deleteContactTypeContact);

                                var delCntType_Cnt = new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    AuditAction = "D",
                                    TableName = "ContactType_Contact",
                                    ModelPKey = deleteContactTypeContact.ContactTypeContactID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "Contact_ContactID", OldValue = deleteContactTypeContact.ContactType_ContactTypeID.ToString()},
                                        new TableInfo{Field_ColumName = "Contact_ContactID", OldValue = deleteContactTypeContact.Contact_ContactID.ToString()},
                                    }
                                };

                                auditToStores.Add(delCntType_Cnt);
                            }
                        }
                        

                        db.Contacts.Attach(contactStoredInDb);
                        db.Entry(contactStoredInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        var currentContact = await db.ContactType_Contact.Include(x => x.Contact).Include(x => x.ContactType).Where(c => c.Contact_ContactID == contact.ContactID).Select(x => x.ContactType.ContactLevel).ToListAsync();
                        if (currentContact.TrueForAll(x => x.Equals("corporation", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            /*Quiere decir que tengo que ver si este contacto esta relacionado con algun pos y eliminar esa relacion*/
                            var posCntToDelete = db.MasterPOS_Contact.Include(x => x.Contact).Include(d => d.MasterPOS).Where(f => f.ContactID == contact.ContactID).ToList();
                            if (posCntToDelete.Any())
                            {
                                db.MasterPOS_Contact.RemoveRange(posCntToDelete);
                            }

                        }
                        if (currentContact.TrueForAll(x => x.Equals("pos", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            /*Quiere decir que tengo que ver si este contacto esta relacionado con alguna corporacion y eliminar esa relacion*/
                            var corpCntToDelete = db.Corp_Owner.Include(d => d.Contact).Include(f => f.CorporateMasterList).Where(g => g.Contact_ContactID == contact.ContactID).ToList();
                            if (corpCntToDelete.Any())
                            {
                                db.Corp_Owner.RemoveRange(corpCntToDelete);
                            }
                        }
                        if (db.ChangeTracker.HasChanges())
                        {
                            await db.SaveChangesAsync();
                        }

                        dbTransaction.Commit();
                      
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("","Something failed. Please try again!");
                        return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { contact }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_MasterPOSContact([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactID,FName,LName,Email,PhoneNumber,active,ContactTypes")] VMPosContact masterPosContact, int? posName)
        {
            if (ModelState.IsValid)
            {
                if (posName == null || posName <= 0)
                {
                    ModelState.AddModelError("","Invalid POS. Please try again!");
                    return Json(new[] { masterPosContact }.ToDataSourceResult(request, ModelState));
                }
                if (await db.Contacts.AnyAsync(x => x.Email.Equals(masterPosContact.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { masterPosContact }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var contactToStore = new Contact
                        {
                            FName = masterPosContact.FName,
                            LName = masterPosContact.LName,
                            PhoneNumber = masterPosContact.PhoneNumber,
                            Email = masterPosContact.Email,
                            active = masterPosContact.active
                        };
                        db.Contacts.Add(contactToStore);
                        await db.SaveChangesAsync();
                        masterPosContact.ContactID = contactToStore.ContactID;

                        db.ContactType_Contact.AddRange(masterPosContact.ContactTypes.Select(x => new ContactType_Contact
                        {
                            Contact_ContactID = contactToStore.ContactID,
                            ContactType_ContactTypeID = x.ContactTypeID
                        }));

                        db.MasterPOS_Contact.Add(new MasterPOS_Contact
                        {
                            ContactID = contactToStore.ContactID,
                            MasterPOS_MasterPOSID = posName.Value
                        });

                        await db.SaveChangesAsync();
                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                      dbTransaction.Rollback();
                      ModelState.AddModelError("", "Something failed. Please try again!");
                      return Json(new[] { masterPosContact }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { masterPosContact }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Save_PosContacts([DataSourceRequest] DataSourceRequest request,VMContactID contacts, int? MasterPOS)
        {
            if (contacts.Contacts.Any() && MasterPOS > 0)
            {
                
            }
            return Json(new[] {new MasterPOS()}.ToDataSourceResult(request, ModelState));
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
