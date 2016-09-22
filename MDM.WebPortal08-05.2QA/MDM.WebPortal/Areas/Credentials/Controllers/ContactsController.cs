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
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
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

        public ActionResult Read_Owners([DataSourceRequest] DataSourceRequest request)
        {
            var result =
                db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                    .Where(ty => ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                    .Select(tv => tv.Contact);

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

        public ActionResult Read_CorpContacts([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            /*Obtener todos los contactos a nivel de corporacion excepto los de tipo Owner*/
            var result =
                db.ContactType_Contact.Include(cnt => cnt.Contact)
                    .Include(ty => ty.ContactType)
                    .Where(ty => ty.ContactType.ContactLevel.Equals("corporation", StringComparison.InvariantCultureIgnoreCase) && ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase) == false)
                    .Select(tv => tv.Contact).Distinct();
            if (corpID != null)
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
                        if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.FName = contact.FName;
                        }
                        if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.FName = contact.FName;
                        }
                        if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.FName = contact.FName;
                        }
                        if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.FName = contact.FName;
                        }
                        if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            contactStoredInDb.FName = contact.FName;
                        }
                        var currentTypesOfThisContact = contactStoredInDb.ContactType_Contact.Select(y => y.ContactType_ContactTypeID).ToList();
                        var byParamContactType = contact.ContactTypes.Select(x => x.ContactTypeID).ToList();

                        if (!byParamContactType.Equals(currentTypesOfThisContact))
                        {
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
                                }
                                var deleteContactTypeContact = await db.ContactType_Contact.FirstOrDefaultAsync(x => x.ContactType_ContactTypeID == cntType && x.Contact_ContactID == contact.ContactID);
                                if (deleteContactTypeContact != null)
                                {
                                    db.ContactType_Contact.Remove(deleteContactTypeContact);
                                }
                            }
                        }
                        db.Contacts.Attach(contactStoredInDb);
                        db.Entry(contactStoredInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

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
