using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Threading;
using System.Globalization;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel;
using MDM.WebPortal.Models;

namespace MDM.WebPortal.Controllers.APP
{
    [Authorize]
    public class ActionCodeDictController : Controller
    {
        private MedProDBEntities db;

        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            if (!string.IsNullOrEmpty(requestContext.HttpContext.Request["culture"]))
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(requestContext.HttpContext.Request["culture"]);
            }
            base.Execute(requestContext);
        }

        public ActionResult Index()
        {
            db = new MedProDBEntities();
            ViewData["categories"] = db.Categories.ToList();
            //ViewData["employees"] = EmployeeRepository.GetAll();

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            using (var actionCodeDict = new MedProDBEntities())
            {
                IQueryable<ActionCodeDict> products = actionCodeDict.ActionCodeDicts;
                //Convert the entities to ViewModel instances.
                DataSourceResult result = products.ToDataSourceResult(request, product => new ActionCodeDictViewModel
                {
                    id = product.id,
                    CollNoteType = product.CollNoteType,
                    Code = product.Code,
                    CategoryID = product.CategoryID,
                    Priority = product.Priority,
                    NTUser = product.NTUser,
                    Active = product.Active,
                    ParsingYN = product.ParsingYN

                });
                return Json(result);
            }
        }

        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, ActionCodeDict actionCodeDict)
        {
            //if (actionCodeDict != null && ModelState.IsValid)
            //{
            //    //ActionCodeDictRepository.Remove(actionCodeDict);
            //}

            return Json(ModelState.ToDataSourceResult());
        }


        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ForeignKeyColumn_Update([DataSourceRequest] DataSourceRequest request,
        //    [Bind(Prefix = "models")]IEnumerable<ActionCodeDictViewModel> products)
        //{
        //    if (products != null && ModelState.IsValid)
        //    {
        //        foreach (var product in products)
        //        {
        //            productService.Update(product);
        //        }
        //    }

        //    return Json(products.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ActionCodeDict actionCodeDict)
        {
            if (actionCodeDict != null && ModelState.IsValid)
            {
                ActionCodeDictRepository.Update(actionCodeDict);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        //public ActionResult Create([DataSourceRequest] DataSourceRequest request, Order order)
        //{
        //    OrderRepository.Insert(order);
        //    return Json(new[] { order }.ToDataSourceResult(request, ModelState));
        //}

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CollNoteType,Code,CategoryID,Priority,NTUser,Active,ParsingYN")] ActionCodeDict actionCodeDict)
        {
            if (ModelState.IsValid)
            {
                db = new MedProDBEntities();
                db.ActionCodeDicts.Add(actionCodeDict);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(actionCodeDict);
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

    }
}