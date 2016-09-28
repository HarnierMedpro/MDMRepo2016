
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
    public class MasterPOS_LevOfCareController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Create_POS_LOfCare([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPosLocID,MasterPOS_MasterPOSID,Lev_of_Care_LevOfCareID")] VMMasterPOS_LevOfCare mpLevOfCare, int ParentID)
        {
            if (ModelState.IsValid && ParentID > 0)
            {
                try
                {
                    if (await db.MasterPOS_LevOfCare.AnyAsync(x => x.Lev_of_Care_LevOfCareID == mpLevOfCare.Lev_of_Care_LevOfCareID && x.MasterPOS_MasterPOSID == ParentID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        mpLevOfCare.MasterPOS_MasterPOSID = ParentID;
                        return Json(new[] { mpLevOfCare }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new MasterPOS_LevOfCare
                    {
                        Lev_of_Care_LevOfCareID = mpLevOfCare.Lev_of_Care_LevOfCareID,
                        MasterPOS_MasterPOSID = ParentID
                    };

                    db.MasterPOS_LevOfCare.Add(toStore);
                    await db.SaveChangesAsync();
                    mpLevOfCare.MasterPosLocID = toStore.MasterPosLocID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        ModelPKey = toStore.MasterPosLocID,
                        TableName = "MasterPOS_LevOfCare",
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "Lev_of_Care_LevOfCareID", NewValue = mpLevOfCare.Lev_of_Care_LevOfCareID.ToString()},
                            new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = ParentID.ToString()}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { mpLevOfCare }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { mpLevOfCare }.ToDataSourceResult(request, ModelState));
        }


        public async Task<ActionResult> Update_POS_LOfCare([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPosLocID,MasterPOS_MasterPOSID,Lev_of_Care_LevOfCareID")] VMMasterPOS_LevOfCare mpLevOfCare)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.MasterPOS_LevOfCare.AnyAsync(x => x.Lev_of_Care_LevOfCareID == mpLevOfCare.Lev_of_Care_LevOfCareID && x.MasterPOS_MasterPOSID == mpLevOfCare.MasterPOS_MasterPOSID
                        && x.MasterPosLocID != mpLevOfCare.MasterPosLocID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { mpLevOfCare }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.MasterPOS_LevOfCare.FindAsync(mpLevOfCare.MasterPosLocID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDb.Lev_of_Care_LevOfCareID != mpLevOfCare.Lev_of_Care_LevOfCareID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Lev_of_Care_LevOfCareID", NewValue = mpLevOfCare.Lev_of_Care_LevOfCareID.ToString(), OldValue = storedInDb.Lev_of_Care_LevOfCareID.ToString() });
                        storedInDb.Lev_of_Care_LevOfCareID = mpLevOfCare.Lev_of_Care_LevOfCareID;
                    }


                    db.MasterPOS_LevOfCare.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        ModelPKey = mpLevOfCare.MasterPosLocID,
                        TableName = "MasterPOS_LevOfCare",
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { mpLevOfCare }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { mpLevOfCare }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_LevOfCareOfThisPos([DataSourceRequest] DataSourceRequest request, int? masterPOSID)
        {
            var result = db.MasterPOS_LevOfCare.Include(x => x.MasterPOS).Include(x => x.Lev_of_Care);
            if (masterPOSID != null)
            {
                result = result.Where(d => d.MasterPOS_MasterPOSID == masterPOSID);
            }
            return Json(result.ToDataSourceResult(request, x => new VMMasterPOS_LevOfCare
            {
                MasterPosLocID = x.MasterPosLocID,
                MasterPOS_MasterPOSID = x.MasterPOS_MasterPOSID,
                Lev_of_Care_LevOfCareID = x.Lev_of_Care_LevOfCareID
            }), JsonRequestBehavior.AllowGet);
        }

        //// GET: PlaceOfServices/LocPOS_LevOfCare
        //public async Task<ActionResult> Index()
        //{
        //    var locPOS_LevOfCare = db.LocPOS_LevOfCare.Include(l => l.Lev_of_Care).Include(l => l.LocationsPOS);
        //    return View(await locPOS_LevOfCare.ToListAsync());
        //}



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
