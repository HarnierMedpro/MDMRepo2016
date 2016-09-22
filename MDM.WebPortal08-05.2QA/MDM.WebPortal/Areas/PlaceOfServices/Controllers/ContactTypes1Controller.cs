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
    public class ContactTypes1Controller : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/ContactTypes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContactTypeList()
        {
            return View();
        }

        public ActionResult Read_ContactType([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.ContactTypes.OrderBy(x => x.ContactType_Name);
            return Json(result.ToDataSourceResult(request, x => new VMContactType1
            {
                ContactTypeID = x.ContactTypeID,
                ContactType_Name = x.ContactType_Name
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ContactType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ContactTypeID, ContactType_Name")] VMContactType1 contactType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals(contactType.ContactType_Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {contactType}.ToDataSourceResult(request, ModelState));
                    }

                    var toStore = new ContactType
                    {
                        ContactType_Name = contactType.ContactType_Name
                    };

                    db.ContactTypes.Add(toStore);
                    await db.SaveChangesAsync();
                    contactType.ContactTypeID = toStore.ContactTypeID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        TableName = "ContactType",
                        ModelPKey = toStore.ContactTypeID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "ContactType_Name", NewValue = toStore.ContactType_Name}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

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
           [Bind(Include = "ContactTypeID, ContactType_Name")] VMContactType1 contactType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals(contactType.ContactType_Name, StringComparison.InvariantCultureIgnoreCase) && x.ContactTypeID != contactType.ContactTypeID ))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.ContactTypes.FindAsync(contactType.ContactTypeID);
                    List<TableInfo> tableColumInfos = new List<TableInfo>
                    {
                        new TableInfo{Field_ColumName = "ContactType_Name", OldValue = storedInDb.ContactType_Name, NewValue = contactType.ContactType_Name}
                    };

                    db.ContactTypes.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                   
                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        TableName = "ContactType",
                        ModelPKey = contactType.ContactTypeID,
                        tableInfos = tableColumInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

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
