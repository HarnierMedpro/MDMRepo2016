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
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class MPServicesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/MPServices
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Service([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.MPServices.OrderBy(x => x.ServName).ToList();
            return Json(result.ToDataSourceResult(request, x => new VMMPService
            {
                MPServID = x.MPServID,
                ServName = x.ServName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Service([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MPServID,ServName")] VMMPService mPService)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.MPServices.AnyAsync(x => x.ServName.Equals(mPService.ServName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {mPService}.ToDataSourceResult(request, ModelState));
                    }

                    var toStore = new MPService
                    {
                        ServName = mPService.ServName
                    };

                    db.MPServices.Add(toStore);
                    await db.SaveChangesAsync();
                    mPService.MPServID = toStore.MPServID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        ModelPKey = toStore.MPServID,
                        TableName = "MPServices",
                        AuditAction = "I",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "ServName", NewValue = toStore.ServName}
                        }
                    };

                   new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { mPService }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { mPService }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Service([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MPServID,ServName")] VMMPService mPService)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.MPServices.AnyAsync(x => x.ServName.Equals(mPService.ServName, StringComparison.InvariantCultureIgnoreCase) && x.MPServID != mPService.MPServID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { mPService }.ToDataSourceResult(request, ModelState));
                    }
                    /*If this function is called is because the user change the only property that he/she can change: it's service name*/
                    var storedInDb = await db.MPServices.FindAsync(mPService.MPServID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo
                        {
                            Field_ColumName = "ServName",
                            OldValue = storedInDb.ServName,
                            NewValue = mPService.ServName
                        }
                    };

                    storedInDb.ServName = mPService.ServName;
                    db.MPServices.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        TableName = "MPServices",
                        AuditAction = "U",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = mPService.MPServID,
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { mPService }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { mPService }.ToDataSourceResult(request, ModelState));
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
