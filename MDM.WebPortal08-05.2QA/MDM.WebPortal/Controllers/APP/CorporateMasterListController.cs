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
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
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
            }));
            
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

        public async Task<ActionResult> CorporateMasterLists_Update([DataSourceRequest]DataSourceRequest request, 
            [Bind(Include="corpID,CorporateName,active")] VMCorporateMasterLists corpMasterList)
        {
            if (corpMasterList != null && ModelState.IsValid)
            {
                try
                {
                    if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.Equals(corpMasterList.CorporateName, StringComparison.CurrentCultureIgnoreCase) 
                                                         && x.corpID != corpMasterList.corpID))
                    {
                        ModelState.AddModelError("CorporateName", "Duplicate Corporate. Please try again!");
                        return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = await db.CorporateMasterLists.FindAsync(corpMasterList.corpID);
                    storedInDb.CorporateName = corpMasterList.CorporateName;
                    storedInDb.active = corpMasterList.active;

                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { corpMasterList }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> CorporateMasterLists_Create([DataSourceRequest]DataSourceRequest request,
            [Bind(Include="corpID,CorporateName,active")] CorporateMasterList corpMasterList)
        {
            if (corpMasterList != null && ModelState.IsValid)
            {
                try
                {
                    //if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.ToLower() == corpMasterList.CorporateName.ToLower()))
                    if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.Equals(corpMasterList.CorporateName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "This Corporate is already in the system.");
                        return Json(new[] {corpMasterList}.ToDataSourceResult(request, ModelState));
                    }
                    db.CorporateMasterLists.Add(corpMasterList);
                    await db.SaveChangesAsync();
                    return Json(new[] { corpMasterList }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                   ModelState.AddModelError("","Something failed. Please try again!");
                   return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                } 
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "corpID,CorporateName,active")] CorporateMasterList corpMasterList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var exist = await db.CorporateMasterLists.AnyAsync(x => x.CorporateName == corpMasterList.CorporateName);
                    if (exist)
                    {
                        ViewBag.Error = "This Corporate is already in the system.";
                        return View(corpMasterList);
                    }
                    else
                    {
                        db.CorporateMasterLists.Add(corpMasterList);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something field. Please try again!";
                    return View(corpMasterList);                     
                }
            }
            else
            {
                return View(corpMasterList);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
