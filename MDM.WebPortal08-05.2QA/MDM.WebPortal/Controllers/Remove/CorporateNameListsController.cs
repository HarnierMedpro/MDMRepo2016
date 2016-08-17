using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Models.FromDB;


namespace MDM.WebPortal.Controllers.Remove
{
    [Authorize]
    public class CorporateNameListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: CorporateNameLists
        public ActionResult Index(string searchString)
        {

            var varList = from s in db.CorporateNameLists
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                varList = varList.Where(s => s .CorporateName.Contains(searchString));
            }

            varList = varList.OrderBy(s => s.CorporateName);
            return View(varList.ToList()); 
        }

        // GET: CorporateNameLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CorporateNameList corporateNameList = db.CorporateNameLists.Find(id);
            if (corporateNameList == null)
            {
                return HttpNotFound();
            }
            return View(corporateNameList);
        }

        // GET: CorporateNameLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CorporateNameLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,DBno,CorporateName,NTUser")] CorporateNameList corporateNameList)
        {
            if (ModelState.IsValid)
            {
                db.CorporateNameLists.Add(corporateNameList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(corporateNameList);
        }

        // GET: CorporateNameLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CorporateNameList corporateNameList = db.CorporateNameLists.Find(id);
            if (corporateNameList == null)
            {
                return HttpNotFound();
            }
            return View(corporateNameList);
        }

        // POST: CorporateNameLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,DBno,CorporateName,NTUser")] CorporateNameList corporateNameList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(corporateNameList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(corporateNameList);
        }

        //// GET: CorporateNameLists/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CorporateNameList corporateNameList = db.CorporateNameLists.Find(id);
        //    if (corporateNameList == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(corporateNameList);
        //}

        //// POST: CorporateNameLists/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    CorporateNameList corporateNameList = db.CorporateNameLists.Find(id);
        //    db.CorporateNameLists.Remove(corporateNameList);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
