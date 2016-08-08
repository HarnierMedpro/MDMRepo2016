using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//using MedProMDM.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Security.Principal;
using System.IO;
using System.Text;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    [Authorize]
    public class CollNoteTypeCatPrioritiesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            using (var varCollNoteTypeCatPriorities = new MedProDBEntities())
            {
                // Export only the current page
                var myLists = varCollNoteTypeCatPriorities.CollNoteTypeCatPriorities.ToDataSourceResult(request).Data;
                // Export all pages (uncomment next line)
                var products = varCollNoteTypeCatPriorities.CollNoteTypeCatPriorities.ToList();

                var output = new MemoryStream();
                var writer = new StreamWriter(output, Encoding.UTF8);

                writer.Write("id,");
                writer.Write("Status,");
                writer.Write("ActionCodes,");
                writer.Write("Category,");
                writer.Write("Priority,");
                writer.Write("Active,");
                writer.Write("ParsingYN");

                writer.WriteLine();

                foreach (CollNoteTypeCatPriority varlist in myLists)
                {

                    //public int id { get; set; }
                    //public string CollNoteType { get; set; }
                    //public string Code { get; set; }
                    //public string CollNoteCat { get; set; }
                    //public string Priority { get; set; }
                    //public string NTUser { get; set; }
                    //public bool Active { get; set; }

                    writer.Write(varlist.id);
                    writer.Write(",");

                    writer.Write("\"");
                    writer.Write(varlist.CollNoteType);
                    writer.Write("\"");
                    writer.Write(",");

                    writer.Write("\"");
                    writer.Write(varlist.Code);
                    writer.Write("\"");
                    writer.Write(",");

                    writer.Write("\"");
                    writer.Write(varlist.CollNoteCat);
                    writer.Write("\"");
                    writer.Write(",");

                    writer.Write("\"");
                    writer.Write(varlist.Priority);
                    writer.Write("\"");
                    writer.Write(",");

                    writer.Write("\"");
                    writer.Write(varlist.Active);
                    writer.Write("\"");
                    writer.Write(",");

                    writer.Write("\"");
                    writer.Write(varlist.ParsingYN);
                    writer.Write("\"");

                    writer.WriteLine();
                }

                writer.Flush();
                output.Position = 0;

                return File(output, "text/comma-separated-values", "MDMExported.csv");
            }
        }

       

        public ActionResult Excel_Export()
        {
            return View();
        }

        public ActionResult Excel_Export_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(db.CollNoteTypeCatPriorities.Select(product => new ActionCodeViewModel
            {

                //public int id { get; set; }
                //public string CollNoteType { get; set; }
                //public string Code { get; set; }
                //public string CollNoteCat { get; set; }
                //public string Priority { get; set; }
                //public string NTUser { get; set; }
                //public bool Active { get; set; }
                //public string PayAmount { get; set; }
                //public string CheckDate { get; set; }
                //public string ParsingYN { get; set; }

                CollNoteType = product.CollNoteType,
                Code = product.Code,
                CollNoteCat = product.CollNoteCat,
                Priority = product.Priority,
                NTUser = product.NTUser,
                Active = product.Active,
                //PayAmount = product.PayAmount,
                //CheckDate = product.CheckDate,
                ParsingYN = product.ParsingYN
            }).ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        // GET: CollNoteTypeCatPriorities
        public ActionResult Index() //string searchString)
        {            
            ViewData["ACType"] = db.ACtypes.ToList();
            ViewData["ACCategory"] = db.ACCategories.ToList();
            ViewData["ACPriority"] = db.ACPriorities.ToList();

            return View();
        }

        public ActionResult Products_Read([DataSourceRequest]DataSourceRequest request)
        {
            using (var vardb = new MedProDBEntities())
            {
                IQueryable<CollNoteTypeCatPriority> products = vardb.CollNoteTypeCatPriorities;
                DataSourceResult result = products.ToDataSourceResult(request);
                return Json(result);
            }
        }

        // GET: CollNoteTypeCatPriorities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollNoteTypeCatPriority collNoteTypeCatPriority = db.CollNoteTypeCatPriorities.Find(id);
            if (collNoteTypeCatPriority == null)
            {
                return HttpNotFound();
            }
            return View(collNoteTypeCatPriority);
        }

        // GET: CollNoteTypeCatPriorities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CollNoteTypeCatPriorities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CollNoteType,Code,CollNoteCat,Priority,NTUser,Active,ParsingYN,ACType")] CollNoteTypeCatPriority collNoteTypeCatPriority)
        {
            if (ModelState.IsValid)
            {
                removeSingleQuotes(collNoteTypeCatPriority); //CB
                checkParsingYN(collNoteTypeCatPriority);  //CB 

                db.CollNoteTypeCatPriorities.Add(collNoteTypeCatPriority);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(collNoteTypeCatPriority);
        }

        private void checkParsingYN(CollNoteTypeCatPriority collNoteTypeCatPriority)
        {


            string source = collNoteTypeCatPriority.Code;
            string[] stringSeparators = new string[] { "*" };
            string[] result;

            result = source.Split(stringSeparators, StringSplitOptions.None);
            //Console.WriteLine(result.Count());

            if ((result.Count() - 1) >= 2)
            {
                collNoteTypeCatPriority.ParsingYN = "Y";
            }
            else
            {
                collNoteTypeCatPriority.ParsingYN = "N";
            }

            //Console.WriteLine();             
        }

        private void removeSingleQuotes(CollNoteTypeCatPriority collNoteTypeCatPriority)
        {
            String s = collNoteTypeCatPriority.CollNoteType;
            s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands 
            collNoteTypeCatPriority.CollNoteType = s;

            s = collNoteTypeCatPriority.Code;
            s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands
            collNoteTypeCatPriority.Code = s;

            s = collNoteTypeCatPriority.CollNoteCat;
            s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands
            collNoteTypeCatPriority.CollNoteCat = s;

            s = collNoteTypeCatPriority.Priority;
            s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands
            collNoteTypeCatPriority.Priority = s;

            //s = collNoteTypeCatPriority.PayAmount;
            //s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands
            //collNoteTypeCatPriority.PayAmount = s;

            //s = collNoteTypeCatPriority.CheckDate;
            //s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands
            //collNoteTypeCatPriority.CheckDate = s;

            //s = collNoteTypeCatPriority.ParsingYN;
            //s = s.Replace("'", ""); // eliminating single quotes for dynamic sql commands
            //collNoteTypeCatPriority.ParsingYN = s;

            collNoteTypeCatPriority.NTUser = User.Identity.Name; // "CB";
        }

        // GET: CollNoteTypeCatPriorities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollNoteTypeCatPriority collNoteTypeCatPriority = db.CollNoteTypeCatPriorities.Find(id);
            if (collNoteTypeCatPriority == null)
            {
                return HttpNotFound();
            }

            removeSingleQuotes(collNoteTypeCatPriority);
            checkParsingYN(collNoteTypeCatPriority);  //CB 

            return View(collNoteTypeCatPriority);
        }

        // POST: CollNoteTypeCatPriorities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CollNoteType,Code,CollNoteCat,Priority,NTUser,Active,ParsingYN,ACType")] CollNoteTypeCatPriority collNoteTypeCatPriority)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collNoteTypeCatPriority).State = EntityState.Modified;

                removeSingleQuotes(collNoteTypeCatPriority);
                checkParsingYN(collNoteTypeCatPriority);  //CB 

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(collNoteTypeCatPriority);
        }

        // GET: CollNoteTypeCatPriorities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollNoteTypeCatPriority collNoteTypeCatPriority = db.CollNoteTypeCatPriorities.Find(id);
            if (collNoteTypeCatPriority == null)
            {
                return HttpNotFound();
            }
            return View(collNoteTypeCatPriority);
        }

        // POST: CollNoteTypeCatPriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CollNoteTypeCatPriority collNoteTypeCatPriority = db.CollNoteTypeCatPriorities.Find(id);
            db.CollNoteTypeCatPriorities.Remove(collNoteTypeCatPriority);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Products_Update([DataSourceRequest]DataSourceRequest request, ActionCodeViewModel product)
        {
            if (ModelState.IsValid)
            {
                using (var varProducts = new MedProDBEntities())
                {
                    // Create a new Product entity and set its properties from the posted ProductViewModel
                    var entity = new CollNoteTypeCatPriority
                    {
                        id = product.id,
                        CollNoteType = product.CollNoteType,
                        Code = product.Code,
                        CollNoteCat = product.CollNoteCat,
                        Priority = product.Priority,
                        NTUser = User.Identity.Name,  //WindowsIdentity.GetCurrent().Name,   //product.NTUser,
                        Active = product.Active,
                        ACType = product.ACType,
                        //PayAmount = product.PayAmount,
                        //CheckDate = product.CheckDate,
                        ParsingYN = product.ParsingYN
                    };
                    // Attach the entity
                    varProducts.CollNoteTypeCatPriorities.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    varProducts.Entry(entity).State = EntityState.Modified;
                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    entity.CollNoteType = entity.CollNoteType.Replace("'", "");
                    entity.Code = entity.Code.Replace("'", "");
                    entity.CollNoteCat = entity.CollNoteCat.Replace("'", "");
                    entity.Priority = entity.Priority.Replace("'", "");


                    //if (entity.PayAmount != null)
                    //{
                    //    entity.PayAmount = entity.PayAmount.Replace("'", "");
                    //}

                    //if (entity.CheckDate != null)
                    //{
                    //    entity.CheckDate = entity.CheckDate.Replace("'", "");
                    //}

                    //entity.ParsingYN = entity.ParsingYN.Replace("'", "");


                    string source = entity.Code;
                    string[] stringSeparators = new string[] { "*" };
                    string[] result;

                    result = source.Split(stringSeparators, StringSplitOptions.None);
                    //Console.WriteLine(result.Count());

                    if ((result.Count() - 1) >= 2)
                    {
                        entity.ParsingYN = "Y";
                    }
                    else
                    {
                        entity.ParsingYN = "N";
                    }

                    // Update the entity in the database
                    varProducts.SaveChanges();
                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Products_Destroy([DataSourceRequest]DataSourceRequest request, ActionCodeViewModel product)
        {
            if (ModelState.IsValid)
            {
                using (var northwind = new MedProDBEntities())
                {
                    // Create a new Product entity and set its properties from the posted ProductViewModel
                    var entity = new CollNoteTypeCatPriority
                    {
                        id = product.id,
                        CollNoteType = product.CollNoteType,
                        Code = product.Code,
                        CollNoteCat = product.CollNoteCat,
                        Priority = product.Priority,
                        NTUser = product.NTUser,
                        Active = product.Active,
                        //PayAmount = product.PayAmount,
                        //CheckDate = product.CheckDate,
                        ParsingYN = product.ParsingYN
                    };
                    // Attach the entity
                    northwind.CollNoteTypeCatPriorities.Attach(entity);
                    // Delete the entity 
                    northwind.CollNoteTypeCatPriorities.Remove(entity);
                    // Or use DeleteObject if using a previous versoin of Entity Framework
                    // northwind.Products.DeleteObject(entity);
                    // Delete the entity in the database
                    northwind.SaveChanges();
                }
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
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