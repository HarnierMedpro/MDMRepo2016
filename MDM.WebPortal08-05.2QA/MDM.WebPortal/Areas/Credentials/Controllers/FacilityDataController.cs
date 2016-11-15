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
    public class FacilityDataController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Facility_Info(int? MasterPOSID)
        {
            var toView = new List<VMFacilityData>();
            if (MasterPOSID == null)
            {
                ViewBag.MasterPOS = 0;
                return View();
            }
            var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                ViewBag.MasterPOS = 0;
                return View();
            }
            var facInfo = pos.FACInfo;
            if (facInfo != null)
            {
                toView.Add(new VMFacilityData
                {
                    FACInfoID = facInfo.FACInfoID,
                    NPI_Number = facInfo.NPI_Number,
                    MasterPOSID = MasterPOSID.Value
                });
            }
            
            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }

        public async Task<ActionResult> Read_Facility([DataSourceRequest] DataSourceRequest request, int? MasterPosID)
        {
            var result = new List<VMFacilityData>();
            if (MasterPosID != null)
            {
                var masterPos = await db.MasterPOS.FindAsync(MasterPosID);
                if (masterPos != null && masterPos.FACInfo != null)
                {
                    var facInfo = masterPos.FACInfo;
                    result.Add(new VMFacilityData
                    {
                        FACInfoID = facInfo.FACInfoID,
                        NPI_Number = facInfo.NPI_Number,
                        MasterPOSID = MasterPosID.Value
                    });
                }
            }
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Facility([DataSourceRequest] DataSourceRequest request, [Bind(Include = "FACInfoID,MasterPOSID,NPI_Number")] VMFacilityData facility, int? ParentID)
        {
            if (ModelState.IsValid)
            {
                if (ParentID == null || ParentID == 0)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var toStore = new FACInfo
                        {
                            NPI_Number = facility.NPI_Number
                        };

                        db.FACInfoes.Add(toStore);
                        await db.SaveChangesAsync();
                        facility.FACInfoID = toStore.FACInfoID;
                        facility.MasterPOSID = ParentID.Value;

                        var pos = await db.MasterPOS.FindAsync(ParentID);
                        pos.FACInfo_FACInfoID = toStore.FACInfoID;
                        db.MasterPOS.Attach(pos);
                        db.Entry(pos).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        List<AuditToStore> auditLogs = new List<AuditToStore>
                        {
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "I",
                                ModelPKey = toStore.FACInfoID,
                                TableName = "FACInfo",
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "NPI_Number", NewValue = toStore.NPI_Number}
                                }
                            },
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "U",
                                ModelPKey = pos.MasterPOSID,
                                TableName = "MasterPOS",
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "FACInfo_FACInfoID", NewValue = toStore.FACInfoID.ToString()}
                                }
                            }
                        };

                        new AuditLogRepository().SaveLogs(auditLogs);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("","Something failed. Please try again!");
                        return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] {facility}.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Facility([DataSourceRequest] DataSourceRequest request, [Bind(Include = "FACInfoID,MasterPOSID,NPI_Number")] VMFacilityData facility, int? ParentID)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var storedInDb = await db.FACInfoes.FindAsync(facility.FACInfoID);

                        storedInDb.NPI_Number = facility.NPI_Number;

                        db.FACInfoes.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "FACInfo",
                            ModelPKey = storedInDb.FACInfoID,
                            AuditAction = "U",
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "NPI_Number", OldValue = storedInDb.NPI_Number, NewValue = facility.NPI_Number}
                            }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
                    }
                }
                
            }
            return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
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
