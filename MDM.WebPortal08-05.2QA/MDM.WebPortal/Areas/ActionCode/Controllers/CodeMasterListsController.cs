using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.ActionCode.Models.ViewModels;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Hubs;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.ActionCode.Controllers
{
    [SetPermissions]
    public class CodeMasterListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ActionCode/CodeMasterLists
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }

        public ActionResult Read_Code([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.CodeMasterLists.OrderBy(x => x.Code).ToList();
            return Json(result.ToDataSourceResult(request, x => new VMCodeMasterList
            {
                CodeID = x.CodeID,
                Code = x.Code
            }), JsonRequestBehavior.AllowGet);
        }
      
        public async Task<ActionResult> Create_Code([DataSourceRequest] DataSourceRequest request, [Bind(Include = "CodeID, Code")] VMCodeMasterList codeMaster)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CodeMasterLists.AnyAsync(x => x.Code.Equals(codeMaster.Code, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] { codeMaster }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new CodeMasterList
                    {
                        Code = codeMaster.Code
                    };
                    db.CodeMasterLists.Add(toStore);
                    await db.SaveChangesAsync();
                    codeMaster.CodeID = toStore.CodeID;

                    CodeMasterListHub.DoIfCodeCreated(codeMaster.CodeID, codeMaster.Code);

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "I",
                        TableName = "CodeMasterList",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.CodeID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo { Field_ColumName = "Code", NewValue = toStore.Code }
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { codeMaster }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { codeMaster }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Code([DataSourceRequest] DataSourceRequest request, [Bind(Include = "CodeID, Code")] VMCodeMasterList codeMaster)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CodeMasterLists.AnyAsync(x => x.Code.Equals(codeMaster.Code, StringComparison.CurrentCultureIgnoreCase) && x.CodeID != codeMaster.CodeID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { codeMaster }.ToDataSourceResult(request, ModelState));
                    }
                    //var toStore = new CodeMasterList
                    //{
                    //    CodeID = codeMaster.CodeID,
                    //    Code = codeMaster.Code
                    //};
                    var toStore = await db.CodeMasterLists.FindAsync(codeMaster.CodeID);
                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo {Field_ColumName = "Code", NewValue = codeMaster.Code, OldValue = toStore.Code}
                    };
                    toStore.Code = codeMaster.Code;

                    db.CodeMasterLists.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.CodeID,
                        TableName = "CodeMasterList",
                        tableInfos = tableColumnInfos,
                        AuditAction = "U"
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { codeMaster }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { codeMaster }.ToDataSourceResult(request, ModelState));
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
