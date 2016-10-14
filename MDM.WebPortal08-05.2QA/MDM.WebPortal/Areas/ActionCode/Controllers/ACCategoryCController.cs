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
using MDM.WebPortal.Areas.ActionCode.Models.ViewModels;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.ActionCode.Controllers
{
    [SetPermissions]
    public class ACCategoryCController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ActionCode/ACCategoryC
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Category([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.ACCategories.OrderBy(x => x.CategoryName).ToDataSourceResult(request, x => new VMCategoria
            {
                CatogoryID = x.CatogoryID,
                CategoryName = x.CategoryName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Category([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "CatogoryID,CategoryName")] VMCategoria aCCategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ACCategories.AnyAsync(x => x.CategoryName.Equals(aCCategory.CategoryName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {aCCategory}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new ACCategory
                    {
                        CategoryName = aCCategory.CategoryName
                    };
                    db.ACCategories.Add(toStore);
                    await db.SaveChangesAsync();
                    aCCategory.CatogoryID = toStore.CatogoryID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "I",
                        TableName = "ACCategory",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.CatogoryID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo { Field_ColumName = "CategoryName", NewValue = toStore.CategoryName }
                        }
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { aCCategory }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { aCCategory }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Category([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "CatogoryID,CategoryName")] VMCategoria aCCategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ACCategories.AnyAsync(x => x.CategoryName.Equals(aCCategory.CategoryName, StringComparison.CurrentCultureIgnoreCase) && x.CatogoryID != aCCategory.CatogoryID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { aCCategory }.ToDataSourceResult(request, ModelState));
                    }
                    //var toStore = new ACCategory
                    //{
                    //    CatogoryID = aCCategory.CatogoryID,
                    //    CategoryName = aCCategory.CategoryName
                    //};
                    var toStore = await db.ACCategories.FindAsync(aCCategory.CatogoryID);
                    List<TableInfo> tableColumnInfos = new List<TableInfo>
                    {
                        new TableInfo
                        {
                            Field_ColumName = "CategoryName",
                            NewValue = aCCategory.CategoryName,
                            OldValue = toStore.CategoryName
                        }
                    };
                    toStore.CategoryName = aCCategory.CategoryName;

                    db.ACCategories.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "U",
                        TableName = "ACCategory",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.CatogoryID,
                        tableInfos = tableColumnInfos
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { aCCategory }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { aCCategory }.ToDataSourceResult(request, ModelState));
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
