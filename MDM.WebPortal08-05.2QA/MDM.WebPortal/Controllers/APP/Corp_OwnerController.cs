using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Models.FromDB;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Controllers.APP
{
    public class Corp_OwnerController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();       

        public ActionResult Index()
        {
            ViewData["Corporate"] = db.CorporateMasterLists.OrderBy(x => x.CorporateName).Select(x => new VMCorporateMasterLists { corpID = x.corpID, CorporateName = x.CorporateName, active = x.active});
            return View();
        }

        public ActionResult Owners([DataSourceRequest] DataSourceRequest request)
        {
            //var Owners = db.Corp_Owner.Include(x => x.OwnerList).Select(x => x.OwnerList).Distinct();
            return Json(db.OwnerLists.ToDataSourceResult(request, x => new VMOwnerList
            {
                active = x.active,
                FirstName = x.FirstName,
                LastName = x.LastName,
                OwnersID = x.OwnersID
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Corporate([DataSourceRequest] DataSourceRequest request , int OwnersID)
        {
            var myCorporate = db.Corp_Owner.Include(x => x.CorporateMasterList).Include(x => x.OwnerList).Where(x => x.OwnersID == OwnersID);
            return Json(myCorporate.ToDataSourceResult(request, x => new VMCorp_Owner
            {
                corpOwnerID = x.corpOwnerID,
                corpID = x.CorporateMasterList.corpID,
                OwnersID = x.OwnersID,
                active = x.CorporateMasterList.active != null && x.CorporateMasterList.active.Value
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "corpOwnerID,corpID,OwnersID")] VMCorp_Owner corp_Owner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Corp_Owner.AnyAsync(x => x.corpID == corp_Owner.corpID && x.OwnersID == corp_Owner.OwnersID && x.corpOwnerID != corp_Owner.corpOwnerID))
                    {
                        ModelState.AddModelError("", "Duplicate data. Please try again!");
                        return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                    }
                    //var toStore = new Corp_Owner { corpOwnerID = corp_Owner.corpOwnerID, corpID = corp_Owner.corpID, OwnersID = corp_Owner.OwnersID };
                    var toStore = await db.Corp_Owner.FindAsync(corp_Owner.OwnersID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (toStore.corpID != corp_Owner.corpID)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            OldValue = toStore.corpID.ToString(),
                            NewValue = corp_Owner.corpID.ToString(),
                            Field_ColumName = "corpID"
                        });
                        toStore.corpID = corp_Owner.corpID;
                    }

                    if (toStore.OwnersID != corp_Owner.OwnersID)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            OldValue = toStore.OwnersID.ToString(),
                            NewValue = corp_Owner.OwnersID.ToString(),
                            Field_ColumName = "OwnersID"
                        });
                        toStore.corpID = corp_Owner.corpID;
                    }

                    db.Corp_Owner.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync(); 
                   
                    AuditToStore auditLog = new AuditToStore
                    {
                        tableInfos = tableColumnInfos,
                        AuditAction = "U",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.corpOwnerID,
                        TableName = "Corp_Owner"
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                }
            }           
            return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Add_Corp([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "corpOwnerID,corpID")] VMCorp_Owner corp_Owner, int ParentID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Corp_Owner.AnyAsync(x => x.corpID == corp_Owner.corpID && x.OwnersID == ParentID))
                    {
                        ModelState.AddModelError("", "Duplicate data. Please try again!");
                        return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new Corp_Owner{ corpID = corp_Owner.corpID, OwnersID = ParentID};
                    db.Corp_Owner.Add(toStore);
                    await db.SaveChangesAsync();
                    corp_Owner.corpOwnerID = toStore.corpOwnerID;
                    corp_Owner.OwnersID = ParentID;
                    if (db.CorporateMasterLists.Find(corp_Owner.corpID).active != null)
                        corp_Owner.active = db.CorporateMasterLists.Find(corp_Owner.corpID).active.Value;

                    AuditToStore auditLog = new AuditToStore
                    {
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo { NewValue = toStore.OwnersID.ToString(), Field_ColumName = "OwnersID" },
                            new TableInfo { NewValue = toStore.corpID.ToString(), Field_ColumName = "corpID" },
                        },
                        AuditAction = "I",
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = toStore.corpOwnerID,
                        TableName = "Corp_Owner"
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                }
            }           
            return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
        }

      

        // GET: Corp_Owner/Create
        public ActionResult Create()
        {
            ViewBag.OwnersID = db.OwnerLists.OrderBy(x => x.LastName).Select( x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName }).ToList();           
            ViewBag.corpID = new SelectList(db.CorporateMasterLists.OrderBy(x => x.CorporateName), "corpID", "CorporateName");           
            return View();
        }

        // POST: Corp_Owner/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "corpOwnerID,corpID,OwnersID")] Corp_Owner corp_Owner)
        {
            if (ModelState.IsValid)
            {
                /*If CorporateMasterList table is empty or OwnerList is empty shows empty dorpdowns in the view,
                 however if the user hit the Create button, send the corp_Owner object with corpID = 0 and/or 
                 OwnersID = 0 and it can not be*/
                if (corp_Owner.corpID == 0 || corp_Owner.OwnersID == 0)
                {
                    ViewBag.OwnersID = db.OwnerLists.OrderBy(x => x.LastName).Select(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false }).ToList();
                    ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                    ViewBag.Error = "You have to select a valid Corporate and/or a valid Owner.";
                    return View(corp_Owner);
                }
                else
                {
                    try
                    {
                        if (db.Corp_Owner.Any(x => x.corpID == corp_Owner.corpID && x.OwnersID ==  corp_Owner.OwnersID))
                        {
                            ViewBag.OwnersID = db.OwnerLists.OrderBy(x => x.LastName).Select(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false }).ToList();
                            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                            ViewBag.Error = "Duplicate relationship. Please try again!";
                            return View(corp_Owner);
                        }
                        db.Corp_Owner.Add(corp_Owner);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ViewBag.OwnersID = db.OwnerLists.OrderBy(x => x.LastName).Select(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false }).ToList();
                        ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                        ViewBag.Error = "Somthing failed. Please try again!";
                        return View(corp_Owner);
                        
                    }
                }                
            }
            ViewBag.OwnersID = db.OwnerLists.OrderBy(x => x.LastName).Select(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false }).ToList();
            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
            return View(corp_Owner);
        }      

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckRelationship(int corp, int owner)
        {
            var result = db.Corp_Owner.Where(x => x.OwnersID == owner && x.corpID == corp).Select(x => new {x.corpID, x.OwnersID}).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
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
