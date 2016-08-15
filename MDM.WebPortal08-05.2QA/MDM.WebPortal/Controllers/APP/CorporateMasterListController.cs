﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Models.FromDB;
using System.Threading.Tasks;
﻿using MDM.WebPortal.Areas.AudiTrails.Controllers;
﻿using MDM.WebPortal.Areas.AudiTrails.Models;
﻿using MDM.WebPortal.Data_Annotations;
﻿using MDM.WebPortal.Models.ViewModel;
﻿using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Controllers.APP
{
    [SetPermissions]
    //[Authorize(Roles = "EMPLOYEE")]
    public class CorporateMasterListController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CorporateMasterLists_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(db.CorporateMasterLists.ToDataSourceResult(request, x => new VMCorporateMasterLists
            {
                corpID = x.corpID,
                active = x.active,
                CorporateName = x.CorporateName
            }), JsonRequestBehavior.AllowGet);
        }

        /*Obtain the DBs for an specific Corporate: NESTED GRID IN INDEX.CSHTML */
        public ActionResult HierarchyBinding_DBs([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            var result = db.Corp_DBs.Include(x => x.CorporateMasterList).Include(x => x.DBList);
            if (corpID != null)
            {
                result = result.Where(x => x.corpID == corpID);
            }
            return Json(result.Select(x => x.DBList).ToDataSourceResult(request, x => new VMDBList
            {
                DB_ID = x.DB_ID,
                active = x.active,
                databaseName = x.databaseName,
                DB = x.DB   
            }), JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult HierarchyBinding_Owner([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            var result = db.Corp_Owner.Include(x => x.CorporateMasterList).Include(x => x.OwnerList);

            if (corpID != null)
            {
                result = result.Where(x => x.corpID == corpID);
            }

            return Json(result.Select(x => x.OwnerList).ToDataSourceResult(request, x => new VMOwnerList
            {
                OwnersID = x.OwnersID,
                active = x.active,
                FirstName = x.FirstName,
                LastName = x.LastName   
            }), JsonRequestBehavior.AllowGet);
        }


        /*This method is riched only when one or more columns of the entity is modified.*/
        public async Task<ActionResult> CorporateMasterLists_Update([DataSourceRequest]DataSourceRequest request, 
            [Bind(Include="corpID,CorporateName,active")] VMCorporateMasterLists corpMasterList)
        {
            if (corpMasterList != null && ModelState.IsValid)
            {
                try
                {
                    if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.Equals(corpMasterList.CorporateName, StringComparison.CurrentCultureIgnoreCase) && x.corpID != corpMasterList.corpID))
                    {
                        ModelState.AddModelError("CorporateName", "Duplicate Corporate. Please try again!");
                        return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                    }
                   
                    //var toAttach = new CorporateMasterList { corpID = corpMasterList.corpID, CorporateName = corpMasterList.CorporateName, active = corpMasterList.active };

                    var storeInDB = await db.CorporateMasterLists.FindAsync(corpMasterList.corpID);

                    var tableColumnInfo = new List<TableInfo>();
                    if (storeInDB.CorporateName != corpMasterList.CorporateName)
                    {
                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "CorporateName", OldValue = storeInDB.CorporateName, NewValue = corpMasterList.CorporateName });
                        storeInDB.CorporateName = corpMasterList.CorporateName;
                    }
                    if (storeInDB.active != corpMasterList.active)
                    {
                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "active", OldValue = storeInDB.active.ToString(), NewValue = corpMasterList.active.ToString() });
                        storeInDB.active = corpMasterList.active;
                    }

                    db.CorporateMasterLists.Attach(storeInDB);
                    db.Entry(storeInDB).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "U",
                        AuditDateTime = DateTime.Now,
                        ModelPKey = storeInDB.corpID,
                        TableName = "CorporateMasterList",
                        UserLogons = User.Identity.GetUserName(),
                        tableInfos = tableColumnInfo
                    };
                    var respository = new AuditLogRepository();
                    respository.AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                }
            }           
            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> CorporateMasterLists_Create([DataSourceRequest]DataSourceRequest request,
            [Bind(Include = "corpID,CorporateName,active")] VMCorporateMasterLists corpMasterList)
        {
            if (ModelState.IsValid)
            {
                try
                {                   
                    if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.Equals(corpMasterList.CorporateName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "This Corporate is already in the system.");
                        return Json(new[] {corpMasterList}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new CorporateMasterList { CorporateName = corpMasterList.CorporateName, active= corpMasterList.active };
                    db.CorporateMasterLists.Add(toStore);
                    await db.SaveChangesAsync();
                    corpMasterList.corpID = toStore.corpID;

                    /*------------- AUDIT LOG SCENARIO -----------------*/
                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "I",
                        AuditDateTime = DateTime.Now,
                        ModelPKey = toStore.corpID,
                        TableName = "CorporateMasterList",
                        UserLogons = User.Identity.GetUserName(),
                        tableInfos = new List<TableInfo> { new TableInfo { Field_ColumName = "CorporateName", NewValue = corpMasterList.CorporateName }, new TableInfo { Field_ColumName = "active", NewValue = corpMasterList.active.ToString() } }
                    };
                    var respository = new AuditLogRepository();
                    respository.AddAuditLogs(auditLog);
                    /*------------- AUDIT LOG SCENARIO -----------------*/
                }
                catch (Exception)
                {
                   ModelState.AddModelError("","Something failed. Please try again!");
                   return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                } 
            }           
            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
        }

      

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
