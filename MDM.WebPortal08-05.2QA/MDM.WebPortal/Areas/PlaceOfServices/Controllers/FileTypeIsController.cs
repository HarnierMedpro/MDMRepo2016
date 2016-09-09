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
    public class FileTypeIsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/FileTypeIs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_FileType([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.FileTypeIs.OrderBy(x => x.FileType_Name);
            return Json(result.ToDataSourceResult(request, x => new VMFileType
            {
                FileTypeID = x.FileTypeID,
                FileType_Name = x.FileType_Name
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_FileType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "FileTypeID, FileType_Name")] VMFileType fileType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.FileTypeIs.AnyAsync(x => x.FileType_Name.Equals(fileType.FileType_Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {fileType}.ToDataSourceResult(request, ModelState));
                    }

                    var toStore = new FileTypeI
                    {
                        FileType_Name = fileType.FileType_Name
                    };

                    db.FileTypeIs.Add(toStore);
                    await db.SaveChangesAsync();
                    fileType.FileTypeID = toStore.FileTypeID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        ModelPKey = toStore.FileTypeID,
                        TableName = "FileTypeI",
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "FileType_Name", NewValue = fileType.FileType_Name}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { fileType }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { fileType }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_FileType([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "FileTypeID, FileType_Name")] VMFileType fileType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.FileTypeIs.AnyAsync(x => x.FileType_Name.Equals(fileType.FileType_Name, StringComparison.InvariantCultureIgnoreCase) && x.FileTypeID != fileType.FileTypeID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { fileType }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.FileTypeIs.FindAsync(fileType.FileTypeID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo{Field_ColumName = "FileType_Name", OldValue = storedInDb.FileType_Name, NewValue = fileType.FileType_Name}
                    };

                    storedInDb.FileType_Name = fileType.FileType_Name;

                    db.FileTypeIs.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                   

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        ModelPKey = fileType.FileTypeID,
                        TableName = "FileTypeI",
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { fileType }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { fileType }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FileTypeID, FileType_Name")] VMFileType fileType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.FileTypeIs.AnyAsync(x => x.FileType_Name.Equals(fileType.FileType_Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        TempData["Error"] = "Duplicate Data. Please try again!";
                        return RedirectToAction("Index", "LocationsPOS", new {area = "PlaceOfServices"});
                    }

                    FileTypeI toStore = new FileTypeI
                    {
                        FileType_Name = fileType.FileType_Name
                    };

                    db.FileTypeIs.Add(toStore);
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        ModelPKey = toStore.FileTypeID,
                        TableName = "FileTypeI",
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "FileType_Name", NewValue = fileType.FileType_Name}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                    return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
                }
                catch (Exception)
                {
                    TempData["Error"] = "Something failed. Please try again!";
                    return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
                }
            }
            TempData["Error"] = "The FILE TYPE field is required. Please try again!";
            return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
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
