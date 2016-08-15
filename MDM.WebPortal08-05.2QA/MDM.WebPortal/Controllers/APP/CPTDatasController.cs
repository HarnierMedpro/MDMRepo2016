using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
//using MedProMDM.Models;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Controllers.APP
{
    [SetPermissions]
    public class CPTDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.CPTDatas.ToDataSourceResult(request, x => new VMCPTData
            {
                id = x.id,
                CPT = x.CPT,
                CPT_Description = x.CPT_Description,
                ShortD = x.ShortD,
                Active = x.Active != null && x.Active.Value
            }), JsonRequestBehavior.AllowGet);
        }

      public async Task<ActionResult> Create_CPT([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "id,CPT,CPT_Description,ShortD, Active")] VMCPTData cPTData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CPTDatas.AnyAsync(x => x.CPT == cPTData.CPT))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new CPTData
                    {
                        CPT = cPTData.CPT,
                        CPT_Description = cPTData.CPT_Description,
                        ShortD = cPTData.ShortD,
                        Active = cPTData.Active
                    };
                    db.CPTDatas.Add(toStore);
                    await db.SaveChangesAsync();
                    cPTData.id = toStore.id;

                    /*------------- AUDIT LOG SCENARIO -----------------*/
                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "I",
                        AuditDateTime = DateTime.Now,
                        ModelPKey = toStore.id,
                        TableName = "CPTData",
                        UserLogons = User.Identity.GetUserName(),
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo { Field_ColumName = "CPT", NewValue = cPTData.CPT }, 
                            new TableInfo { Field_ColumName = "CPT_Description", NewValue = cPTData.CPT_Description },
                            new TableInfo { Field_ColumName = "ShortD", NewValue = cPTData.ShortD},
                            new TableInfo { Field_ColumName = "Active", NewValue = cPTData.Active.ToString() }
                        }
                    };
                    var respository = new AuditLogRepository();
                    respository.AddAuditLogs(auditLog);
                    /*------------- AUDIT LOG SCENARIO -----------------*/
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                }                
            }
            return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "id,CPT,CPT_Description,ShortD, Active")] VMCPTData cPTData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CPTDatas.AnyAsync(x => x.CPT == cPTData.CPT && x.id != cPTData.id))
                    {
                        ModelState.AddModelError("", "Duplicate CPT. Please try again!");
                        return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                    }                    
                    //var storedInDb = new CPTData
                    //{
                    //    id = cPTData.id,
                    //    CPT = cPTData.CPT,
                    //    CPT_Description = cPTData.CPT_Description,
                    //    ShortD = cPTData.ShortD,
                    //    Active = cPTData.Active,
                    //};
                    var storedInDb = await db.CPTDatas.FindAsync(cPTData.id);

                    var tableColumnInfo = new List<TableInfo>();
                    if (storedInDb.CPT != cPTData.CPT)
                    {
                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "CPT", OldValue = storedInDb.CPT, NewValue = cPTData.CPT});
                        storedInDb.CPT = cPTData.CPT;
                    }
                    if (storedInDb.CPT_Description != cPTData.CPT_Description)
                    {
                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "CPT_Description", OldValue = storedInDb.CPT_Description, NewValue = cPTData.CPT_Description });
                        storedInDb.CPT_Description = cPTData.CPT_Description;
                    }
                    if (storedInDb.ShortD != cPTData.ShortD)
                    {
                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "ShortD", OldValue = storedInDb.ShortD, NewValue = cPTData.ShortD });
                        storedInDb.ShortD = cPTData.ShortD;
                    }
                    if (storedInDb.Active != cPTData.Active)
                    {
                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "Active", OldValue = storedInDb.Active.ToString(), NewValue = cPTData.Active.ToString() });
                        storedInDb.Active = cPTData.Active;
                    }

                    db.CPTDatas.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync(); 
                   
                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "U",
                        tableInfos = tableColumnInfo,
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = storedInDb.id,
                        TableName = "CPTData"
                    };

                   new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Somethig failed. Please try again!");
                    return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { cPTData }.ToDataSourceResult(request, ModelState));
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