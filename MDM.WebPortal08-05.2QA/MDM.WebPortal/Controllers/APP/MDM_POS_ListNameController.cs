//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using MDM.WebPortal.Models.FromDB;
//using Kendo.Mvc.UI;
//using Kendo.Mvc.Extensions;
//using MDM.WebPortal.Areas.AudiTrails.Controllers;
//using MDM.WebPortal.Areas.AudiTrails.Models;
//using MDM.WebPortal.Data_Annotations;
//using MDM.WebPortal.Models.ViewModel;
//using Microsoft.AspNet.Identity;

//namespace MDM.WebPortal.Controllers.APP
//{
//    [SetPermissions]
//    public class MDM_POS_ListNameController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();
      
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request)
//        {
//            var result = await db.MDM_POS_ListName.Select(x => new VMMDM_POS_ListName { active = x.active, MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.PosName }).ToListAsync();
//            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
//        }

//        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "MDMPOS_ListNameID,PosName,active")] VMMDM_POS_ListName mDM_POS_ListName)
//        {
//            if (ModelState.IsValid)
//            {               
//                try
//                {
//                    if (await db.MDM_POS_ListName.AnyAsync(x => x.PosName.Equals(mDM_POS_ListName.PosName, StringComparison.CurrentCultureIgnoreCase) && x.MDMPOS_ListNameID != mDM_POS_ListName.MDMPOS_ListNameID))
//                    {                       
//                        ModelState.AddModelError("", "Duplicate POS. Please try again!");
//                        return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
//                    }
//                    //var StoredInDB = new MDM_POS_ListName
//                    //{
//                    //    MDMPOS_ListNameID = mDM_POS_ListName.MDMPOS_ListNameID, 
//                    //    PosName = mDM_POS_ListName.PosName, 
//                    //    active = mDM_POS_ListName.active
//                    //};
//                    var StoredInDB = await db.MDM_POS_ListName.FindAsync(mDM_POS_ListName.MDMPOS_ListNameID);

//                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                    if (StoredInDB.PosName != mDM_POS_ListName.PosName)
//                    {
//                        tableColumnInfos.Add(new TableInfo
//                        {
//                            Field_ColumName = "PosName",
//                            NewValue = mDM_POS_ListName.PosName,
//                            OldValue = StoredInDB.PosName
//                        });
//                        StoredInDB.PosName = mDM_POS_ListName.PosName;
//                    }
//                    if (StoredInDB.active != mDM_POS_ListName.active)
//                    {
//                        tableColumnInfos.Add(new TableInfo
//                        {
//                            Field_ColumName = "active",
//                            NewValue = mDM_POS_ListName.active.ToString(),
//                            OldValue = StoredInDB.active.ToString()
//                        });
//                        StoredInDB.active = mDM_POS_ListName.active;
//                    }

//                    db.MDM_POS_ListName.Attach(StoredInDB);
//                    db.Entry(StoredInDB).State = EntityState.Modified;
//                    await db.SaveChangesAsync();
                  
//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        AuditAction = "U",
//                        tableInfos = tableColumnInfos,
//                        AuditDateTime = DateTime.Now,
//                        UserLogons = User.Identity.GetUserName(),
//                        ModelPKey = StoredInDB.MDMPOS_ListNameID,
//                        TableName = "MDM_POS_ListName"
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);
//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something failed. Please try again!");
//                    return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
//                } 
//            }           
//            return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));         
//        }

//        public async Task<ActionResult> Create_POS([DataSourceRequest] DataSourceRequest request, [Bind(Include = "MDMPOS_ListNameID,PosName,active")] VMMDM_POS_ListName mDM_POS_ListName)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (await db.MDM_POS_ListName.AnyAsync(x => x.PosName.Equals(mDM_POS_ListName.PosName, StringComparison.CurrentCultureIgnoreCase)))
//                    {
//                        ModelState.AddModelError("", "Duplicate POS. Please try again!");
//                        return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
//                    }
//                    var StoredInDB = new MDM_POS_ListName { PosName = mDM_POS_ListName.PosName, active = mDM_POS_ListName.active };
//                    db.MDM_POS_ListName.Add(StoredInDB);
//                    await db.SaveChangesAsync();
//                    mDM_POS_ListName.MDMPOS_ListNameID = StoredInDB.MDMPOS_ListNameID;

//                    AuditToStore auditLog = new AuditToStore
//                    {
//                        AuditAction = "I",
//                        tableInfos = new List<TableInfo>
//                        {
//                            new TableInfo{Field_ColumName = "MDMPOS_ListNameID", NewValue = StoredInDB.MDMPOS_ListNameID.ToString()},
//                            new TableInfo{Field_ColumName = "PosName", NewValue = StoredInDB.PosName},
//                            new TableInfo{Field_ColumName = "active", NewValue = StoredInDB.active.ToString()}
//                        },
//                        AuditDateTime = DateTime.Now,
//                        UserLogons = User.Identity.GetUserName(),
//                        ModelPKey = StoredInDB.MDMPOS_ListNameID,
//                        TableName = "MDM_POS_ListName"
//                    };

//                    new AuditLogRepository().AddAuditLogs(auditLog);
//                }
//                catch (Exception)
//                {
//                    ModelState.AddModelError("", "Something failed. Please try again!");
//                    return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
//                }
//            }
//            return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
//        }

//        public ActionResult Details()
//        {
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Get)]
//        public ActionResult CheckIfExist(string term)
//        {
//            return Json(db.MDM_POS_ListName.Where(x => x.PosName == term).Select(x => x.PosName).ToList(), JsonRequestBehavior.AllowGet);
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
