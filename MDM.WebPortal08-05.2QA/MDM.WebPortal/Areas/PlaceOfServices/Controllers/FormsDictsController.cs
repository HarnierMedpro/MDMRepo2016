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
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class FormsDictsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/FormsDicts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Forms([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.FormsDicts.OrderBy(x => x.FormName).ToDataSourceResult(request, x => new VMFormsDict
            {
                FormsID = x.FormsID,
                FormName = x.FormName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Forms([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "FormsID,FormName")] VMFormsDict formsDict)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.FormsDicts.AnyAsync(x => x.FormName.Equals(formsDict.FormName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {formsDict}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new FormsDict
                    {
                        FormName = formsDict.FormName
                    };

                    db.FormsDicts.Add(toStore);
                    await db.SaveChangesAsync();
                    formsDict.FormsID = toStore.FormsID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "I",
                        ModelPKey = toStore.FormsID,
                        TableName = "FormsDict",
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "FormName", NewValue = formsDict.FormName}
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { formsDict }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { formsDict }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Forms([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "FormsID,FormName")] VMFormsDict formsDict)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.FormsDicts.AnyAsync(x => x.FormName.Equals(formsDict.FormName, StringComparison.InvariantCultureIgnoreCase) && x.FormsID != formsDict.FormsID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { formsDict }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = await db.FormsDicts.FindAsync(formsDict.FormsID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo{Field_ColumName = "FormName", OldValue = storedInDb.FormName, NewValue = formsDict.FormName}
                    };
                    storedInDb.FormName = formsDict.FormName;
                    db.FormsDicts.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "U",
                        ModelPKey = formsDict.FormsID,
                        TableName = "FormsDict",
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { formsDict }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { formsDict }.ToDataSourceResult(request, ModelState));
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
