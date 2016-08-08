﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.DAL;
using MDM.WebPortal.Models.ViewModel;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace MDM.WebPortal.Controllers.APP
{
    public class DBListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: DBLists
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.DBLists.ToListAsync());
        //}

        //With Kendo UI for ASP.NET MVC
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(db.DBLists.ToDataSourceResult(request, x => new VMDBList
            {
                DB_ID = x.DB_ID,
                DB = x.DB,
                databaseName = x.databaseName,
                active = x.active
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest]DataSourceRequest request, 
            [Bind(Include = "DB_ID,DB,databaseName, active")] VMDBList dBList)
        {
            if (dBList != null && ModelState.IsValid)
            {
               var storedInDb = new DBList{DB_ID = dBList.DB_ID, DB = dBList.DB, databaseName = dBList.databaseName, active = dBList.active};
               try
               {
                   if (await db.DBLists.AnyAsync(x => x.DB == dBList.DB && x.DB_ID != dBList.DB_ID))
                   {
                       ModelState.AddModelError("", "Duplicate Database. Please try again!");
                       return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
                   }
                   db.Entry(storedInDb).State = EntityState.Modified;
                   await db.SaveChangesAsync();
                   return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
               }
               catch (Exception)
               {
                   ModelState.AddModelError("", "Something Failed. Please try again!");
                   return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
               }
            }

            ModelState.AddModelError("", "Something Failed. Please try again!");
            return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
           
        }

        public async Task<ActionResult> Create_DBs([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "DB_ID,DB,databaseName")] DBList dBList)
        {
            if (dBList != null && ModelState.IsValid)
            {
                try
                {
                    if (await db.DBLists.AnyAsync(x => x.DB == dBList.DB))
                    {
                        ModelState.AddModelError("", "Duplicate Database. Please try again!");
                        return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
                    }
                    db.DBLists.Add(dBList);
                    await db.SaveChangesAsync();
                    return Json(new[] { dBList }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
                }
                
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { dBList }.ToDataSourceResult(request, ModelState));
        }

       

        // GET: DBLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DBLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DB_ID,DB,databaseName")] DBList dBList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.DBLists.AnyAsync(x => x.DB == dBList.DB))
                    {
                        ViewBag.Error = "This DB is already in the system.";
                        return View(dBList);
                    }
                    db.DBLists.Add(dBList);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    return View(dBList);
                }
                
            }

            return View(dBList);
        }

       

      

      

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckDB(string term)
        {
            if (!String.IsNullOrEmpty(term))
            {
                _DALDBList DBs = new _DALDBList();
                var resultSet = DBs.BuscarDB(term.ToLower());
                List<VMDB> result = new List<VMDB>();
                foreach (DataRow row in resultSet.Rows)
                {
                    result.Add(new VMDB
                    {
                        DB = row.ItemArray[0].ToString()
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Error", new { area = "Error" });
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