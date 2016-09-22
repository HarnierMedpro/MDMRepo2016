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
//    public class LocPOS_LevOfCareController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();

//        public async Task<ActionResult> Create_POS_LOfCare([DataSourceRequest] DataSourceRequest request,
//            [Bind(Include = "LocPosLocID,Lev_of_Care_LevOfCareID")] VMLocPOS_LevOfCare pOSLOC, int ParentID)
//        {
//            if (ModelState.IsValid && ParentID > 0)
//            {
//                try
//                {
//                    if (await db.LocPOS_LevOfCare.AnyAsync(x => x.Lev_of_Care_LevOfCareID == pOSLOC.Lev_of_Care_LevOfCareID && x.LocationsPOS_Facitity_DBs_IDPK == ParentID))
//                    {
//                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
//                        pOSLOC.LocationsPOS_Facitity_DBs_IDPK = ParentID;

//                        return Json(new[] {pOSLOC}.ToDataSourceResult(request, ModelState));
//                    }
//                    var toStore = new LocPOS_LevOfCare
//                    {
//                        Lev_of_Care_LevOfCareID = pOSLOC.Lev_of_Care_LevOfCareID,
//                        LocationsPOS_Facitity_DBs_IDPK = ParentID
//                    };

//                    db.LocPOS_LevOfCare.Add(toStore);
//                    await db.SaveChangesAsync();
//                    pOSLOC.LocPosLocID = toStore.LocPosLocID;

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "I",
//                        ModelPKey = toStore.LocPosLocID,
//                        TableName = "LocPOS_LevOfCare",
//                        tableInfos = new List<TableInfo>
//                        {
//                            new TableInfo{Field_ColumName = "Lev_of_Care_LevOfCareID", NewValue = pOSLOC.Lev_of_Care_LevOfCareID.ToString()},
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


//        public async Task<ActionResult> Update_POS_LOfCare([DataSourceRequest] DataSourceRequest request,
//           [Bind(Include = "LocPosLocID,Lev_of_Care_LevOfCareID,LocationsPOS_Facitity_DBs_IDPK")] VMLocPOS_LevOfCare
//               pOSLOC)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (await db.LocPOS_LevOfCare.AnyAsync(x => x.Lev_of_Care_LevOfCareID == pOSLOC.Lev_of_Care_LevOfCareID && x.LocationsPOS_Facitity_DBs_IDPK == pOSLOC.LocationsPOS_Facitity_DBs_IDPK
//                        && x.LocPosLocID != pOSLOC.LocPosLocID))
//                    {
//                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
//                        return Json(new[] { pOSLOC }.ToDataSourceResult(request, ModelState));
//                    }

//                    var storedInDb = await db.LocPOS_LevOfCare.FindAsync(pOSLOC.LocPosLocID);

//                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                    if (storedInDb.Lev_of_Care_LevOfCareID != pOSLOC.Lev_of_Care_LevOfCareID)
//                    {
//                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Lev_of_Care_LevOfCareID", NewValue = pOSLOC.Lev_of_Care_LevOfCareID.ToString(), OldValue = storedInDb.Lev_of_Care_LevOfCareID.ToString()});
//                        storedInDb.Lev_of_Care_LevOfCareID = pOSLOC.Lev_of_Care_LevOfCareID;
//                    }


//                    db.LocPOS_LevOfCare.Attach(storedInDb);
//                    db.Entry(storedInDb).State = EntityState.Modified;
//                    await db.SaveChangesAsync();
                   

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        UserLogons = User.Identity.GetUserName(),
//                        AuditDateTime = DateTime.Now,
//                        AuditAction = "U",
//                        ModelPKey = pOSLOC.LocPosLocID,
//                        TableName = "LocPOS_LevOfCare",
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
        
        
//        // GET: PlaceOfServices/LocPOS_LevOfCare
//        public async Task<ActionResult> Index()
//        {
//            var locPOS_LevOfCare = db.LocPOS_LevOfCare.Include(l => l.Lev_of_Care).Include(l => l.LocationsPOS);
//            return View(await locPOS_LevOfCare.ToListAsync());
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
