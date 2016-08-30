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
            ViewData["Codigos"] = db.CodeMasterLists.OrderBy(x => x.Code).Select(x => new { x.CodeID, x.Code });
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

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.ActionCodeID,
                        TableName = "ActionCode",
                        AuditAction = "I",
                        AuditDateTime = DateTime.Now,
                        tableInfos = new List<TableInfo>
                        {
                           new TableInfo{Field_ColumName = "CollNoteType", NewValue = toStore.CollNoteType}, 
                           new TableInfo{Field_ColumName = "CodeID", NewValue = toStore.CodeID.ToString()}, 
                           new TableInfo{Field_ColumName = "CategoryID", NewValue = toStore.CategoryID.ToString()}, 
                           new TableInfo{Field_ColumName = "PriorityID", NewValue = toStore.PriorityID.ToString()}, 
                           new TableInfo{Field_ColumName = "ACTypeID", NewValue = toStore.ACTypeID.ToString()}, 
                           new TableInfo{Field_ColumName = "Active", NewValue = toStore.Active.ToString()}, 
                           new TableInfo{Field_ColumName = "ParsingYN", NewValue = toStore.ParsingYN}, 
                        }
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
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
                //var toStore = new WebPortal.Models.FromDB.ActionCode
                //{
                //    ActionCodeID = actionCode.ActionCodeID,
                //    CollNoteType = actionCode.CollNoteType,
                //    CodeID = actionCode.CodeID,
                //    CategoryID = actionCode.CategoryID,
                //    PriorityID = actionCode.PriorityID,
                //    ACTypeID = actionCode.ACTypeID,
                //    Active = actionCode.Active,
                //    ParsingYN = actionCode.ParsingYN
                //};
                try
                {
                    var toStore = await db.ActionCodes.FindAsync(actionCode.ActionCodeID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (toStore.CollNoteType != actionCode.CollNoteType)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "CollNoteType", 
                            NewValue = actionCode.CollNoteType, 
                            OldValue = toStore.CollNoteType
                        });
                        toStore.CollNoteType = actionCode.CollNoteType;
                    }
                    if (toStore.CodeID != actionCode.CodeID)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "CodeID", 
                            NewValue = actionCode.CodeID.ToString(), 
                            OldValue = toStore.CodeID.ToString()
                        });
                        toStore.CodeID = actionCode.CodeID;
                    }
                    if (toStore.CategoryID != actionCode.CategoryID)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "CategoryID", 
                            NewValue = actionCode.CategoryID.ToString(), 
                            OldValue = toStore.CategoryID.ToString()
                        });
                        toStore.CategoryID = actionCode.CategoryID;
                    }
                    if (toStore.PriorityID != actionCode.PriorityID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PriorityID", NewValue = actionCode.PriorityID.ToString(), OldValue = toStore.PriorityID.ToString() });
                        toStore.PriorityID = actionCode.PriorityID;
                    }
                    if (toStore.ACTypeID != actionCode.ACTypeID)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ACTypeID", NewValue = actionCode.ACTypeID.ToString(), OldValue = toStore.ACTypeID.ToString() });
                        toStore.ACTypeID = actionCode.ACTypeID;
                    }
                    if (toStore.Active != actionCode.Active)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Active", NewValue = actionCode.Active.ToString(), OldValue = toStore.Active.ToString() });
                        toStore.Active = actionCode.Active;
                    }
                    if (toStore.ParsingYN != actionCode.ParsingYN)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ParsingYN", NewValue = actionCode.ParsingYN, OldValue = toStore.ParsingYN });
                        toStore.ParsingYN = actionCode.ParsingYN;
                    }

                    db.ActionCodes.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.ActionCodeID,
                        TableName = "ActionCode",
                        AuditAction = "U",
                        AuditDateTime = DateTime.Now,
                        tableInfos = tableColumnInfos
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
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
