


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
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class MasterPOS_MPServController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Create_POSService([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPosMPServID,MPServices_MPServID")] VMMasterPOS_MPServ masterPosMpServ, int ParentID)
        {
            if (ModelState.IsValid && ParentID > 0)
            {
                try
                {
                    if (await db.MasterPOS_MPServ.AnyAsync(x => x.MPServices_MPServID == masterPosMpServ.MPServices_MPServID && x.MasterPOS_MasterPOSID == ParentID))
                    {
                        masterPosMpServ.MasterPOS_MasterPOSID = ParentID;
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { masterPosMpServ }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new MasterPOS_MPServ
                    {
                        MPServices_MPServID = masterPosMpServ.MPServices_MPServID,
                        MasterPOS_MasterPOSID  = ParentID
                    };

                    db.MasterPOS_MPServ.Add(toStore);
                    await db.SaveChangesAsync();
                    masterPosMpServ.MasterPosMPServID = toStore.MasterPosMPServID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        ModelPKey = toStore.MasterPosMPServID,
                        TableName = "MasterPOS_MPServ",
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "MPServices_MPServID", NewValue = masterPosMpServ.MPServices_MPServID.ToString()},
                            new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = ParentID.ToString()}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { masterPosMpServ }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { masterPosMpServ }.ToDataSourceResult(request, ModelState));
        }


        public async Task<ActionResult> Update_POSService([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "MasterPosMPServID,MPServices_MPServID,MasterPOS_MasterPOSID")] VMMasterPOS_MPServ masterPosMpServ)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.MasterPOS_MPServ.AnyAsync(x => x.MPServices_MPServID == masterPosMpServ.MPServices_MPServID && x.MasterPOS_MasterPOSID == masterPosMpServ.MasterPOS_MasterPOSID
                        && x.MasterPosMPServID != masterPosMpServ.MasterPosMPServID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { masterPosMpServ }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.MasterPOS_MPServ.FindAsync(masterPosMpServ.MasterPosMPServID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDb.MPServices_MPServID != masterPosMpServ.MPServices_MPServID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "MPServices_MPServID", NewValue = masterPosMpServ.MPServices_MPServID.ToString(), OldValue = storedInDb.MPServices_MPServID.ToString() });
                        storedInDb.MPServices_MPServID = masterPosMpServ.MPServices_MPServID;
                    }


                    db.MasterPOS_MPServ.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        ModelPKey = masterPosMpServ.MasterPosMPServID,
                        TableName = "MasterPOS_MPServ",
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { masterPosMpServ }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { masterPosMpServ }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_ServicesOfThisPOS([DataSourceRequest] DataSourceRequest request, int? masterPOSID)
        {
            var result = db.MasterPOS_MPServ.Where(x => x.MasterPOS_MasterPOSID == masterPOSID);
            return Json(result.ToDataSourceResult(request, x => new VMMasterPOS_MPServ
            {
                MasterPosMPServID = x.MasterPosMPServID,
                MasterPOS_MasterPOSID = x.MasterPOS_MasterPOSID,
                MPServices_MPServID = x.MPServices_MPServID
            }));
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
