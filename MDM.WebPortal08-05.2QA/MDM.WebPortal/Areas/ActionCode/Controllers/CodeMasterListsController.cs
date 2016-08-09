﻿using System;
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
    public class CodeMasterListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ActionCode/CodeMasterLists
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Code([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.CodeMasterLists.ToDataSourceResult(request, x => new VMCodeMasterList
            {
                CodeID = x.CodeID,
                Code = x.Code
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Code([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "CodeID, Code")] VMCodeMasterList code)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CodeMasterLists.AnyAsync(x => x.Code.Equals(code.Code, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {code}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new CodeMasterList
                    {
                        Code = code.Code
                    };
                    db.CodeMasterLists.Add(toStore);
                    await db.SaveChangesAsync();
                    code.CodeID = toStore.CodeID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { code }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { code }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Code([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "CodeID, Code")] VMCodeMasterList code)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.CodeMasterLists.AnyAsync(x => x.Code.Equals(code.Code, StringComparison.CurrentCultureIgnoreCase) && x.CodeID != code.CodeID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { code }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new CodeMasterList
                    {
                        CodeID = code.CodeID,
                        Code = code.Code
                    };
                    db.CodeMasterLists.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { code }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { code }.ToDataSourceResult(request, ModelState));
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