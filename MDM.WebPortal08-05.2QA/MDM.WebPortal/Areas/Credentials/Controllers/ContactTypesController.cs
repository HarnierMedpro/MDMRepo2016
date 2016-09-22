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
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class ContactTypesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            ViewData["Level"] = new List<SelectListItem> { new SelectListItem { Text = "CORPORATION", Value = "CORPORATION" }, new SelectListItem{Text = "POS", Value = "POS"} };
            return View();
        }

        public ActionResult ContactTypeList()
        {
            ViewData["Level"] = new List<SelectListItem> { new SelectListItem { Text = "CORPORATION", Value = "CORPORATION" }, new SelectListItem { Text = "POS", Value = "POS" } };
            return View();
        }

        public ActionResult Read_ContactType([DataSourceRequest] DataSourceRequest request)
        {
            var resutl = db.ContactTypes.OrderBy(x => x.ContactType_Name).Select(x => new VMContactType
            {
                ContactTypeID = x.ContactTypeID,
                ContactType_Name = x.ContactType_Name,
                ContactLevel = x.ContactLevel
            }).ToList();
            return Json(resutl.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_CorpContactType([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.ContactTypes.Where(ct => ct.ContactLevel.Equals("corporation", StringComparison.InvariantCultureIgnoreCase) &&
                                               ct.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase) == false).
                                               OrderBy(ct => ct.ContactType_Name).ToList();
            return Json(result.ToDataSourceResult(request, x => new VMContactType
            {
                ContactTypeID = x.ContactTypeID,
                ContactType_Name = x.ContactType_Name,
                ContactLevel = x.ContactLevel
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ContactType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactTypeID,ContactType_Name,ContactLevel")] VMContactType contactType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals(contactType.ContactType_Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] {contactType}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new ContactType
                    {
                        ContactType_Name = contactType.ContactType_Name,
                        ContactLevel = contactType.ContactLevel
                    };
                    db.ContactTypes.Add(toStore);
                    await db.SaveChangesAsync();
                    contactType.ContactTypeID = toStore.ContactTypeID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ContactType([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "ContactTypeID,ContactType_Name,ContactLevel")] VMContactType contactType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals(contactType.ContactType_Name, StringComparison.InvariantCultureIgnoreCase) && x.ContactTypeID != contactType.ContactTypeID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = await db.ContactTypes.FindAsync(contactType.ContactTypeID);
                    if (storedInDb.ContactType_Name != contactType.ContactType_Name)
                    {
                        storedInDb.ContactType_Name = contactType.ContactType_Name;
                    }
                    if (storedInDb.ContactLevel != contactType.ContactLevel)
                    {
                        storedInDb.ContactLevel = contactType.ContactLevel;
                    }

                    db.ContactTypes.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
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
