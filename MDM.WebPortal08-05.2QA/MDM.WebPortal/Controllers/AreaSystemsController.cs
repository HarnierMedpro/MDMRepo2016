using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models;
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Controllers
{
    [SetPermissions]
    public class AreaSystemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AreaSystems
        public ActionResult Index()
        {
            return View();
        }
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.Areas.ToListAsync());
        //}

        public ActionResult Read_Area([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Areas.OrderBy(x => x.AreaName).ToDataSourceResult(request, x => new VMAreaSystems
            {
                AreaID = x.AreaID,
                AreaName = x.AreaName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Area([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "AreaID,AreaName")] VMAreaSystems areaSystem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Areas.AnyAsync( x => x.AreaName.Equals(areaSystem.AreaName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                      ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {areaSystem}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new AreaSystem
                    {
                        AreaName = areaSystem.AreaName
                    };
                    db.Areas.Add(toStore);
                    await db.SaveChangesAsync();
                    areaSystem.AreaID = toStore.AreaID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "I",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        TableName = "AreaSystems",
                        ModelPKey = toStore.AreaID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "AreaName", NewValue = toStore.AreaName}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                     ModelState.AddModelError("","Something failed. Please try again!");
                     return Json(new[] { areaSystem }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { areaSystem }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Area([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "AreaID,AreaName")] VMAreaSystems areaSystem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Areas.AnyAsync(x => x.AreaName.Equals(areaSystem.AreaName, StringComparison.InvariantCultureIgnoreCase) && x.AreaID != areaSystem.AreaID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { areaSystem }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.Areas.FindAsync(areaSystem.AreaID);
                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo{Field_ColumName = "AreaName", NewValue = areaSystem.AreaName, OldValue = storedInDb.AreaName }
                    };
                    storedInDb.AreaName = areaSystem.AreaName;
                    db.Areas.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "U",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        TableName = "AreaSystems",
                        ModelPKey = storedInDb.AreaID,
                        tableInfos = tableColumnInfos
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { areaSystem }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { areaSystem }.ToDataSourceResult(request, ModelState));
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
