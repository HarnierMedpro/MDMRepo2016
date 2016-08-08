using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//using MedProMDM.Models;
using PagedList;
using MDM.WebPortal.Models.FromDB;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace MDM.WebPortal.Controllers.APP
{
    [Authorize]
    public class MasterUserListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        //Database.SetInitializer(new NullDatabaseInitializer<Context>());

        // GET: MasterUserLists                
        public ActionResult Index(string searchString)
        {

            //var userList = from s in db.MasterUserLists
            //               select s;

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    userList = userList.Where(s => s.EmployeeName.Contains(searchString));
            //}

            //userList = userList.OrderBy(s => s.EmployeeName);
            //return View(userList.ToList());

            return View();

        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.MasterUserLists.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Include="id,ADP_ID,EmployeeName,ADP_ID_,Edgemed_UserName,ZoomServer,EdgeMed_ID,Job_Title,Staff_Manager,User_Active,FirstName,Middle_Init,LastName,NTUser,Type")] MasterUserList masterUserList)
        {
            if (masterUserList != null && ModelState.IsValid)
            {
                 db.Entry(masterUserList).State = EntityState.Modified;
                 db.SaveChanges();
            }
            return Json(new[] { masterUserList }.ToDataSourceResult(request, ModelState));
        }

        // GET: MasterUserLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterUserList masterUserList = db.MasterUserLists.Find(id);
            if (masterUserList == null)
            {
                return HttpNotFound();
            }
            return View(masterUserList);
        }

        // GET: MasterUserLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MasterUserLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ADP_ID,EmployeeName,ADP_ID_,Edgemed_UserName,ZoomServer,EdgeMed_ID,Job_Title,Staff_Manager,User_Active,FirstName,Middle_Init,LastName,NTUser,Type")] MasterUserList masterUserList)
        {

            if (ModelState.IsValid)
            {

                var result = db.MasterUserLists.Where(x => x.EdgeMed_ID == masterUserList.EdgeMed_ID && x.ZoomServer == masterUserList.ZoomServer).FirstOrDefault();
                if (result == null)
                {
                    db.MasterUserLists.Add(masterUserList);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.msg = "The [EdgeMED_ID] Record you entered is already in our DB !!!";
            }

            return View(masterUserList);
        }

        // GET: MasterUserLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterUserList masterUserList = db.MasterUserLists.Find(id);
            if (masterUserList == null)
            {
                return HttpNotFound();
            }
            return View(masterUserList);
        }

        // POST: MasterUserLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ADP_ID,EmployeeName,ADP_ID_,Edgemed_UserName,ZoomServer,EdgeMed_ID,Job_Title,Staff_Manager,User_Active,FirstName,Middle_Init,LastName,NTUser,Type")] MasterUserList masterUserList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(masterUserList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(masterUserList);
        }

        // GET: MasterUserLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MasterUserList masterUserList = db.MasterUserLists.Find(id);
            if (masterUserList == null)
            {
                return HttpNotFound();
            }
            return View(masterUserList);
        }

        // POST: MasterUserLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MasterUserList masterUserList = db.MasterUserLists.Find(id);
            db.MasterUserLists.Remove(masterUserList);
            db.SaveChanges();
            return RedirectToAction("Index");
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