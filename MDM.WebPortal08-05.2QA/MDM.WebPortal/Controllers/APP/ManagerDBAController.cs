﻿﻿using System;
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
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    public class ManagerDBAController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Products_Read([DataSourceRequest]DataSourceRequest request)
        {
            //using (var vardb = new MedProDBEntities())
            //{
            //    IQueryable<ManagerDBAccessBI> products = vardb.ManagerDBAccessBIs;
            //    DataSourceResult result = products.ToDataSourceResult(request);
            //    return Json(result);
            //}

            return Json(db.ManagerDBAccessBIs.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: ManagerDBA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
            if (managerDBAccessBI == null)
            {
                return HttpNotFound();
            }
            return View(managerDBAccessBI);
        }

        // GET: ManagerDBA/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagerDBA/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,FvP,DB,Manager,AliasName,NTUser,active")] ManagerDBAccessBI managerDBAccessBI)
        {
            if (ModelState.IsValid)
            {
                db.ManagerDBAccessBIs.Add(managerDBAccessBI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(managerDBAccessBI);
        }


        // GET: ManagerDBA/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
            if (managerDBAccessBI == null)
            {
                return HttpNotFound();
            }
            return View(managerDBAccessBI);
        }

        // POST: ManagerDBA/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,FvP,DB,Manager,AliasName,NTUser,active")] ManagerDBAccessBI managerDBAccessBI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(managerDBAccessBI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(managerDBAccessBI);
        }

        // GET: ManagerDBA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
            if (managerDBAccessBI == null)
            {
                return HttpNotFound();
            }
            return View(managerDBAccessBI);
        }

        // POST: ManagerDBA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
            db.ManagerDBAccessBIs.Remove(managerDBAccessBI);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Products_Update([DataSourceRequest]DataSourceRequest request, ManagerDBAccessBIViewModel product)
        {
            if (ModelState.IsValid)
            {
                using (var varProducts = new MedProDBEntities())
                {
                    // Create a new Product entity and set its properties from the posted ProductViewModel
                    var entity = new ManagerDBAccessBI
                    {
                        id = product.id,
                        FvP = product.FvP,
                        DB = product.DB,
                        Manager = product.Manager,
                        AliasName = product.AliasName,
                        NTUser = User.Identity.Name,
                        active = product.active
                    };
                    // Attach the entity
                    varProducts.ManagerDBAccessBIs.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    varProducts.Entry(entity).State = EntityState.Modified;
                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    //entity.CollNoteType = entity.CollNoteType.Replace("'", "");
                    //entity.Code = entity.Code.Replace("'", "");
                    //entity.CollNoteCat = entity.CollNoteCat.Replace("'", "");
                    //entity.Priority = entity.Priority.Replace("'", "");




                    //string source = entity.Code;
                    //string[] stringSeparators = new string[] { "*" };
                    //string[] result;

                    //result = source.Split(stringSeparators, StringSplitOptions.None);

                    //if ((result.Count() - 1) >= 2)
                    //{
                    //    entity.ParsingYN = "Y";
                    //}
                    //else
                    //{
                    //    entity.ParsingYN = "N";
                    //}

                    // Update the entity in the database
                    varProducts.SaveChanges();
                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

   
}