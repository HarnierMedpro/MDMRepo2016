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
    public class ACtypesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ActionCode/ACtypes
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.ACtypes.ToListAsync());
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_ACType([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.ACtypes.OrderBy(x => x.ACTypeName).ToDataSourceResult(request, x => new VMACType
            {
                ACTypeID = x.ACTypeID,
                ACTypeName = x.ACTypeName
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ACType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ACTypeID,ACTypeName")] VMACType aCtype)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ACtypes.AnyAsync(x => x.ACTypeName.Equals(aCtype.ACTypeName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {aCtype}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new ACtype
                    {
                        ACTypeName = aCtype.ACTypeName
                    };
                    db.ACtypes.Add(toStore);
                    await db.SaveChangesAsync();
                    aCtype.ACTypeID = toStore.ACTypeID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ACType([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ACTypeID,ACTypeName")] VMACType aCtype)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.ACtypes.AnyAsync(x => x.ACTypeName.Equals(aCtype.ACTypeName, StringComparison.CurrentCultureIgnoreCase) && x.ACTypeID != aCtype.ACTypeID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new ACtype
                    {
                        ACTypeID = aCtype.ACTypeID,
                        ACTypeName = aCtype.ACTypeName
                    };
                    db.ACtypes.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { aCtype }.ToDataSourceResult(request, ModelState));
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
