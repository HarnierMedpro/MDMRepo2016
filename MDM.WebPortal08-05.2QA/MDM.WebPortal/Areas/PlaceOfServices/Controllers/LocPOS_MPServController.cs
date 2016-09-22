//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using MDM.WebPortal.Areas.AudiTrails.Controllers;
//using MDM.WebPortal.Areas.AudiTrails.Models;
//using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
//using MDM.WebPortal.Models.FromDB;
//using Microsoft.AspNet.Identity;

//namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
//{
//    public class LocPOS_MPServController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();

//        public async Task<ActionResult> Create_POSService([DataSourceRequest] DataSourceRequest request,
//            [Bind(Include = "LocPosMPServID,MPServices_MPServID")] VMLocPOS_MPServ pOSLOC, int ParentID)
//        {
//            if (ModelState.IsValid && ParentID > 0)
//            {
//                try
//                {
//                    if (await db.LocPOS_MPServ.AnyAsync(x => x.MPServices_MPServID == pOSLOC.MPServices_MPServID && x.LocationsPOS_Facitity_DBs_IDPK == ParentID))
//                    {
//                        pOSLOC.LocationsPOS_Facitity_DBs_IDPK = ParentID;
//                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
//                        return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//                    }
//                    var toStore = new LocPOS_MPServ
//                    {
//                        MPServices_MPServID = pOSLOC.MPServices_MPServID,
//                        LocationsPOS_Facitity_DBs_IDPK = ParentID
//                    };

//                    db.LocPOS_MPServ.Add(toStore);
//                    await db.SaveChangesAsync();
//                    pOSLOC.LocPosMPServID = toStore.LocPosMPServID;

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "I",
//                        ModelPKey = toStore.LocPosMPServID,
//                        TableName = "LocPOS_MPServ",
//                        tableInfos = new List<TableInfo>
//                        {
//                            new TableInfo{Field_ColumName = "MPServices_MPServID", NewValue = pOSLOC.MPServices_MPServID.ToString()},
//                            new TableInfo{Field_ColumName = "LocationsPOS_Facitity_DBs_IDPK", NewValue = ParentID.ToString()}
//                        }
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);
//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something failed. Please try again!");
//                    return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//        }


//        public async Task<ActionResult> Update_POSService([DataSourceRequest] DataSourceRequest request,
//           [Bind(Include = "LocPosMPServID,MPServices_MPServID,LocationsPOS_Facitity_DBs_IDPK")] VMLocPOS_MPServ
//               pOSLOC)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (await db.LocPOS_MPServ.AnyAsync(x => x.MPServices_MPServID == pOSLOC.MPServices_MPServID && x.LocationsPOS_Facitity_DBs_IDPK == pOSLOC.LocationsPOS_Facitity_DBs_IDPK
//                        && x.LocPosMPServID != pOSLOC.LocPosMPServID))
//                    {
//                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
//                        return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//                    }

//                    var storedInDb = await db.LocPOS_MPServ.FindAsync(pOSLOC.LocPosMPServID);

//                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                    if (storedInDb.MPServices_MPServID != pOSLOC.MPServices_MPServID)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "MPServices_MPServID", NewValue = pOSLOC.MPServices_MPServID.ToString(), OldValue = storedInDb.MPServices_MPServID.ToString() });
//                        storedInDb.MPServices_MPServID = pOSLOC.MPServices_MPServID;
//                    }


//                    db.LocPOS_MPServ.Attach(storedInDb);
//                    db.Entry(storedInDb).State = EntityState.Modified;
//                    await db.SaveChangesAsync();


//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "U",
//                        ModelPKey = pOSLOC.LocPosMPServID,
//                        TableName = "LocPOS_MPServ",
//                        tableInfos = tableColumnInfos
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);
//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something failed. Please try again!");
//                    return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//        }
        
        
       

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
