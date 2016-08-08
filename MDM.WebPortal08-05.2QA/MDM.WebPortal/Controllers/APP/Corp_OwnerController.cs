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
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    public class Corp_OwnerController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       

        public ActionResult Index()
        {
            ViewData["Corporate"] = db.CorporateMasterLists.ToList();
            return View();
        }

        public ActionResult Owners([DataSourceRequest] DataSourceRequest request)
        {
            var Owners = db.Corp_Owner.Include(x => x.OwnerList).Select(x => x.OwnerList).Distinct();
            return Json(Owners.ToDataSourceResult(request, x => new VMOwnerList
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

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "corpOwnerID,corpID,OwnersID")] Corp_Owner corp_Owner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Corp_Owner.AnyAsync(x => x.corpID == corp_Owner.corpID && x.OwnersID == corp_Owner.OwnersID))
                    {
                        ModelState.AddModelError("", "Duplicate data. Please try again!");
                        return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                    }
                    db.Entry(corp_Owner).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new[] { corp_Owner }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Add_Corp([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "corpOwnerID,corpID")] VMCreate_Corp_Owner corp_Owner, int ParentID)
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
                    var toStore = new Corp_Owner{corpOwnerID = corp_Owner.corpOwnerID, corpID = corp_Owner.corpID, OwnersID = ParentID};
                    db.Corp_Owner.Add(toStore);
                    await db.SaveChangesAsync();
                    return Json(new[] { corp_Owner }.ToDataSourceResult(request));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
                }
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] { corp_Owner }.ToDataSourceResult(request, ModelState));
        }

      

        // GET: Corp_Owner/Create
        public ActionResult Create()
        {
            ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName });           
            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName");           
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
                    ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                    ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                    ViewBag.Error = "You have to select a valid Corporate and/or a valid Owner.";
                    return View();
                }
                else
                {
                    try
                    {
                        if (db.Corp_Owner.Any(x => x.corpID == corp_Owner.corpID && x.OwnersID ==  corp_Owner.OwnersID))
                        {
                            ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                            ViewBag.Error = "Duplicate relationship.";
                            return View(corp_Owner);
                        }
                        db.Corp_Owner.Add(corp_Owner);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                        ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                        ViewBag.Error = "Somthing goes wrong. Please try again!";
                        return View(corp_Owner);
                        
                    }
                }
                
            }

            ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false});           
            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
            return View(corp_Owner);
        }

        // GET: Corp_Owner/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Corp_Owner corp_Owner = await db.Corp_Owner.FindAsync(id);
            if (corp_Owner == null)
            {
                return HttpNotFound();
            }
            //ViewBag.OwnersID = new SelectList(db.OwnerLists, "OwnersID", "LastName", corp_Owner.OwnersID);
            ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
            return View(corp_Owner);
        }

        // POST: Corp_Owner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "corpOwnerID,corpID,OwnersID")] Corp_Owner corp_Owner)
        {
            if (ModelState.IsValid)
            {
                /*If CorporateMasterList table is empty or OwnerList is empty shows empty dorpdowns in the view,
                however if the user hit the Create button, send the corp_Owner object with corpID = 0 and/or 
                OwnersID = 0 and it can not be*/
                if (corp_Owner.corpID == 0 || corp_Owner.OwnersID == 0)
                {
                    ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                    ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                    ViewBag.Error = "You have to select a valid Corporate and/or a valid Owner.";
                    return View();
                }
                else
                {
                    try
                    {
                        var storedInDB = await db.Corp_Owner.FindAsync(corp_Owner.corpOwnerID);
                        var list = new List<Corp_Owner>(){storedInDB};
                        var distinct = db.Corp_Owner.ToList().Except(list);

                        if (distinct.FirstOrDefault(x => x.corpID == corp_Owner.corpID && x.OwnersID == corp_Owner.OwnersID) != null)
                        {
                            ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                            ViewBag.Error = "Duplicate relationship.";
                            return View(corp_Owner);
                        }

                      string[] fieldsToBind = new string[] { "corpID", "OwnersID"};
                      if (TryUpdateModel(storedInDB, fieldsToBind))
                      {
                          db.Entry(storedInDB).State = EntityState.Modified;
                          await db.SaveChangesAsync();
                          return RedirectToAction("Index");
                      }
                      else
                      {
                          ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                          ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                          ViewBag.Error = "Somthing goes wrong. Please try again!";
                          return View(corp_Owner);
                      }
                    }
                    catch (Exception)
                    {
                        ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
                        ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
                        ViewBag.Error = "Somthing goes wrong. Please try again!";
                        return View(corp_Owner);
                    }
                }
               
            }
            ViewBag.OwnersID = db.OwnerLists.ToList().ConvertAll(x => new SelectListItem { Value = x.OwnersID.ToString(), Text = x.LastName + "," + " " + x.FirstName, Selected = (x.OwnersID == corp_Owner.OwnersID) ? true : false });
            ViewBag.corpID = new SelectList(db.CorporateMasterLists, "corpID", "CorporateName", corp_Owner.corpID);
            return View(corp_Owner);
        }

        // GET: Corp_Owner/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Corp_Owner corp_Owner = await db.Corp_Owner.FindAsync(id);
            if (corp_Owner == null)
            {
                return HttpNotFound();
            }
            return View(corp_Owner);
        }

        // POST: Corp_Owner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Corp_Owner corp_Owner = await db.Corp_Owner.FindAsync(id);
            db.Corp_Owner.Remove(corp_Owner);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
