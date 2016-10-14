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
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
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

        public ActionResult Read_POSContactType([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.ContactTypes.Where(ct => ct.ContactLevel.Equals("pos", StringComparison.InvariantCultureIgnoreCase)).
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
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals(contactType.ContactType_Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                        }
                        var toStore = new ContactType
                        {
                            ContactType_Name = contactType.ContactType_Name,
                            ContactLevel = contactType.ContactLevel
                        };
                        db.ContactTypes.Add(toStore);
                        await db.SaveChangesAsync();
                        contactType.ContactTypeID = toStore.ContactTypeID;

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            AuditAction = "I",
                            TableName = "ContactTypes",
                            ModelPKey = toStore.ContactTypeID,
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{ Field_ColumName = "ContactType_Name", NewValue = contactType.ContactType_Name },
                                new TableInfo{ Field_ColumName = "ContactLevel", NewValue = contactType.ContactLevel }
                            }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ContactType([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "ContactTypeID,ContactType_Name,ContactLevel")] VMContactType contactType)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.ContactTypes.AnyAsync(x => x.ContactType_Name.Equals(contactType.ContactType_Name, StringComparison.InvariantCultureIgnoreCase) && x.ContactTypeID != contactType.ContactTypeID))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                        }
                        var storedInDb = await db.ContactTypes.FindAsync(contactType.ContactTypeID);
                        List<TableInfo> tableColumInfos = new List<TableInfo>();

                        if (storedInDb.ContactType_Name != contactType.ContactType_Name)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "ContactType_Name", OldValue = storedInDb.ContactType_Name, NewValue = contactType.ContactType_Name });
                            storedInDb.ContactType_Name = contactType.ContactType_Name;
                        }
                        if (storedInDb.ContactLevel != contactType.ContactLevel)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "ContactLevel", OldValue = storedInDb.ContactLevel, NewValue = contactType.ContactLevel });
                            storedInDb.ContactLevel = contactType.ContactLevel;
                        }

                        db.ContactTypes.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            AuditAction = "I",
                            TableName = "ContactTypes",
                            ModelPKey = contactType.ContactTypeID,
                            tableInfos = tableColumInfos
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { contactType }.ToDataSourceResult(request, ModelState));
                    }
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
