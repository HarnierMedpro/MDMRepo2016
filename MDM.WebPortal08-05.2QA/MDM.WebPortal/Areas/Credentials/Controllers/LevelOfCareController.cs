using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class LevelOfCareController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/LevelOfCare
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Level([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.Lev_of_Care.OrderBy(x => x.LevOfCareName).ToList();
            return Json(result.ToDataSourceResult(request, lv => new VMLevelOfCare
            {
                LevOfCareID = lv.LevOfCareID,
                LevOfCareName = lv.LevOfCareName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Level([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "LevOfCareID,LevOfCareName")] VMLevelOfCare levOfCare)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Lev_of_Care.AnyAsync(x => x.LevOfCareName.Equals(levOfCare.LevOfCareName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {levOfCare}.ToDataSourceResult(request, ModelState));
                    }

                    var toStore = new Lev_of_Care
                    {
                        LevOfCareName = levOfCare.LevOfCareName
                    };

                    db.Lev_of_Care.Add(toStore);
                    await db.SaveChangesAsync();
                    levOfCare.LevOfCareID = toStore.LevOfCareID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "I",
                        TableName = "Lev_of_Care",
                        ModelPKey = toStore.LevOfCareID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo { Field_ColumName = "LevOfCareName", NewValue = toStore.LevOfCareName }
                        }
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { levOfCare }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { levOfCare }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Level([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "LevOfCareID,LevOfCareName")] VMLevelOfCare levOfCare)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Lev_of_Care.AnyAsync(x => x.LevOfCareName.Equals(levOfCare.LevOfCareName, StringComparison.InvariantCultureIgnoreCase) && x.LevOfCareID != levOfCare.LevOfCareID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { levOfCare }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.Lev_of_Care.FindAsync(levOfCare.LevOfCareID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo{Field_ColumName = "LevOfCareName", OldValue = storedInDb.LevOfCareName, NewValue = levOfCare.LevOfCareName}
                    };

                    storedInDb.LevOfCareName = levOfCare.LevOfCareName;
                    db.Lev_of_Care.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "U",
                        TableName = "Lev_of_Care",
                        ModelPKey = levOfCare.LevOfCareID,
                        tableInfos = tableColumnInfos
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { levOfCare }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { levOfCare }.ToDataSourceResult(request, ModelState));
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
