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
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ActionCode.Controllers
{
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
                    var toStore = new ACCategory
                    {
                        CatogoryID = aCCategory.CatogoryID,
                        CategoryName = aCCategory.CategoryName
                    };
                    db.ACCategories.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
