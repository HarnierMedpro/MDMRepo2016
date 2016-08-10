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
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ActionCode.Controllers
{
    [SetPermissions]
    public class ActionCodesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ActionCode/ActionCodes
        //public async Task<ActionResult> Index()
        //{
        //    var actionCodes = db.ActionCodes.Include(a => a.ACCategory).Include(a => a.ACPriority).Include(a => a.ACtype).Include(a => a.CodeMasterList);
        //    return View(await actionCodes.ToListAsync());
        //}
        public ActionResult Index()
        {
            ViewData["Codigos"] = db.CodeMasterLists.Select(x => new { x.CodeID, x.Code });
            ViewData["Category"] = db.ACCategories.OrderBy(x => x.CategoryName).Select(cat => new {cat.CatogoryID, cat.CategoryName});
            ViewData["Priority"] = db.ACPriorities.Select(x => new {x.PriorityID, x.PriorityName });
            ViewData["ACType"] = db.ACtypes.OrderBy(x => x.ACTypeName).Select(x => new {x.ACTypeID, x.ACTypeName});
            ViewData["Parsing"] = new List<SelectListItem>{new SelectListItem{Text = "YES", Value = "Y"}, new SelectListItem{Text = "NO", Value = "N"}};
            return View();
        }

        public ActionResult Read_ActionCode([DataSourceRequest] DataSourceRequest request)
        {
            var actionCodes = db.ActionCodes.Include(a => a.ACCategory).Include(a => a.ACPriority).Include(a => a.ACtype).Include(a => a.CodeMasterList);
            return Json(actionCodes.ToDataSourceResult(request, x => new VMActionCode
            {
                ActionCodeID = x.ActionCodeID,
                CollNoteType = x.CollNoteType,
                CodeID = x.CodeID,
                CategoryID = x.CategoryID,
                PriorityID = x.PriorityID,
                ACTypeID = x.ACTypeID,
                Active = x.Active,
                ParsingYN = x.ParsingYN
            }), JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> Create_ActionCode([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ActionCodeID,CollNoteType,CodeID,CategoryID,PriorityID,ACTypeID,Active,ParsingYN")]  VMActionCode actionCode)
        {
            if (ModelState.IsValid)
            {
                var toStore = new WebPortal.Models.FromDB.ActionCode
                {
                    CollNoteType = actionCode.CollNoteType,
                    CodeID = actionCode.CodeID,
                    CategoryID = actionCode.CategoryID,
                    PriorityID = actionCode.PriorityID,
                    ACTypeID = actionCode.ACTypeID,
                    Active = actionCode.Active,
                    ParsingYN = actionCode.ParsingYN
                };
                try
                {
                    db.ActionCodes.Add(toStore);
                    await db.SaveChangesAsync();
                    actionCode.ActionCodeID = toStore.ActionCodeID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] {actionCode}.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { actionCode }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ActionCode([DataSourceRequest] DataSourceRequest request,
          [Bind(Include = "ActionCodeID,CollNoteType,CodeID,CategoryID,PriorityID,ACTypeID,Active,ParsingYN")]  VMActionCode actionCode)
        {
            if (ModelState.IsValid)
            {
                var toStore = new WebPortal.Models.FromDB.ActionCode
                {
                    ActionCodeID = actionCode.ActionCodeID,
                    CollNoteType = actionCode.CollNoteType,
                    CodeID = actionCode.CodeID,
                    CategoryID = actionCode.CategoryID,
                    PriorityID = actionCode.PriorityID,
                    ACTypeID = actionCode.ACTypeID,
                    Active = actionCode.Active,
                    ParsingYN = actionCode.ParsingYN
                };
                try
                {
                    db.ActionCodes.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { actionCode }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { actionCode }.ToDataSourceResult(request, ModelState));
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
