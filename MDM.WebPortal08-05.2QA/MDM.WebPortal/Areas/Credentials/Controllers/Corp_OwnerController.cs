using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class Corp_OwnerController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Create(int? corpID)
        {
            if (corpID == null)
            {
                return RedirectToAction("Index","Error", new{area="Error"});
            }
            var corporation = await db.CorporateMasterLists.FindAsync(corpID);
            if (corporation == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
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

                        if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var contactTypeID = db.ContactTypes.First(x => x.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase)).ContactTypeID;
                            db.ContactType_Contact.Add(new ContactType_Contact{Contact_ContactID = contactToStore.ContactID, ContactType_ContactTypeID = contactTypeID});
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
                            db.ContactType_Contact.Add(new ContactType_Contact { Contact_ContactID = contactToStore.ContactID, ContactType_ContactTypeID = newContactType.ContactTypeID });
                        }
                        db.Corp_Owner.Add(new Corp_Owner
                        {
                            corpID = ParentID,
                            Contact_ContactID = contactToStore.ContactID
                        });
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
                    if (!contactStoredInDb.LName.Equals(contact.LName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        contactStoredInDb.LName = contact.LName;
                    }
                    if (!contactStoredInDb.FName.Equals(contact.FName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        contactStoredInDb.FName = contact.FName;
                    }
                    if (contactStoredInDb.PhoneNumber != contact.PhoneNumber)
                    {
                        contactStoredInDb.PhoneNumber = contact.PhoneNumber;
                    }
                    if (!contactStoredInDb.Email.Equals(contact.Email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        contactStoredInDb.Email = contact.Email;
                    }
                    if (contactStoredInDb.active != contact.active)
                    {
                        contactStoredInDb.active = contact.active;
                    }

                    db.Contacts.Attach(contactStoredInDb);
                    db.Entry(contactStoredInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
