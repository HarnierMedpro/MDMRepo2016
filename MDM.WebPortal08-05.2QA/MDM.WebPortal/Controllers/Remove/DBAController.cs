using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Kendo.Mvc.UI;

using Kendo.Mvc.Extensions;
using System.Security.Principal;
using System.IO;
using System.Text;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel.Delete;

namespace MDM.WebPortal.Controllers.Remove
{
    [Authorize]
    public class DBAController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: ManagerDBA
        public ActionResult Index()
        {

            db = new MedProDBEntities();
            //ViewData["FvPList"] = db.FvPLists.ToList();
            List<VMFvpList> temp = new List<VMFvpList>();
            temp.Add(new VMFvpList {id = 1, FvpName = "FAC"});
            temp.Add(new VMFvpList {id = 2, FvpName = "PHY"});
            ViewData["FvPList"] = temp;
            


            // // // // // // // // // // // // 
            // // // // // // // // // // // // 
            // // // // // // // // // // // // 
            // Create a list of parts.
            List<ManagerList> managerlist = new List<ManagerList>();

            // Add parts to the list.
            managerlist.Add(new ManagerList() { ManagerName = "Manager01", id = 1 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager02", id = 2 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager03", id = 3 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager04", id = 4 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager05", id = 5 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager06", id = 6 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager07", id = 7 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager08", id = 8 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager09", id = 9 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager10", id = 10 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager11", id = 11 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager12", id = 12 });
            managerlist.Add(new ManagerList() { ManagerName = "Manager13", id = 13 });


            ViewData["ManagerList"] = managerlist.ToList();
            // // // // // // // // // // // // 
            // // // // // // // // // // // // 
            // // // // // // // // // // // // 


            List<DBList> DBlist = new List<DBList>();
            DBlist.Add(new DBList() { DBNum = "001", id = 1 });
            DBlist.Add(new DBList() { DBNum = "002", id = 2 });
            DBlist.Add(new DBList() { DBNum = "003", id = 3 });
            DBlist.Add(new DBList() { DBNum = "004", id = 4 });
            DBlist.Add(new DBList() { DBNum = "005", id = 5 });
            DBlist.Add(new DBList() { DBNum = "006", id = 6 });
            DBlist.Add(new DBList() { DBNum = "007", id = 7 });
            DBlist.Add(new DBList() { DBNum = "008", id = 8 });
            DBlist.Add(new DBList() { DBNum = "009", id = 9 });
            DBlist.Add(new DBList() { DBNum = "010", id = 10 });
            DBlist.Add(new DBList() { DBNum = "011", id = 11 });
            DBlist.Add(new DBList() { DBNum = "012", id = 12 });
            DBlist.Add(new DBList() { DBNum = "013", id = 13 });
            DBlist.Add(new DBList() { DBNum = "014", id = 14 });
            DBlist.Add(new DBList() { DBNum = "015", id = 15 });
            DBlist.Add(new DBList() { DBNum = "016", id = 16 });
            DBlist.Add(new DBList() { DBNum = "017", id = 17 });
            DBlist.Add(new DBList() { DBNum = "018", id = 18 });
            DBlist.Add(new DBList() { DBNum = "019", id = 19 });
            DBlist.Add(new DBList() { DBNum = "020", id = 20 });
            DBlist.Add(new DBList() { DBNum = "021", id = 21 });
            DBlist.Add(new DBList() { DBNum = "022", id = 22 });
            DBlist.Add(new DBList() { DBNum = "023", id = 23 });
            DBlist.Add(new DBList() { DBNum = "024", id = 24 });
            DBlist.Add(new DBList() { DBNum = "025", id = 25 });
            DBlist.Add(new DBList() { DBNum = "026", id = 26 });
            DBlist.Add(new DBList() { DBNum = "027", id = 27 });
            DBlist.Add(new DBList() { DBNum = "028", id = 28 });
            DBlist.Add(new DBList() { DBNum = "029", id = 29 });
            DBlist.Add(new DBList() { DBNum = "030", id = 30 });
            DBlist.Add(new DBList() { DBNum = "031", id = 31 });
            DBlist.Add(new DBList() { DBNum = "032", id = 32 });
            DBlist.Add(new DBList() { DBNum = "033", id = 33 });
            DBlist.Add(new DBList() { DBNum = "034", id = 34 });
            DBlist.Add(new DBList() { DBNum = "035", id = 35 });
            DBlist.Add(new DBList() { DBNum = "036", id = 36 });
            DBlist.Add(new DBList() { DBNum = "037", id = 37 });
            DBlist.Add(new DBList() { DBNum = "038", id = 38 });
            DBlist.Add(new DBList() { DBNum = "039", id = 39 });
            DBlist.Add(new DBList() { DBNum = "040", id = 40 });
            DBlist.Add(new DBList() { DBNum = "041", id = 41 });
            DBlist.Add(new DBList() { DBNum = "042", id = 42 });
            DBlist.Add(new DBList() { DBNum = "043", id = 43 });
            DBlist.Add(new DBList() { DBNum = "044", id = 44 });
            DBlist.Add(new DBList() { DBNum = "045", id = 45 });
            DBlist.Add(new DBList() { DBNum = "046", id = 46 });
            DBlist.Add(new DBList() { DBNum = "047", id = 47 });
            DBlist.Add(new DBList() { DBNum = "048", id = 48 });
            DBlist.Add(new DBList() { DBNum = "049", id = 49 });
            DBlist.Add(new DBList() { DBNum = "050", id = 50 });
            DBlist.Add(new DBList() { DBNum = "051", id = 51 });
            DBlist.Add(new DBList() { DBNum = "052", id = 52 });
            DBlist.Add(new DBList() { DBNum = "053", id = 53 });
            DBlist.Add(new DBList() { DBNum = "054", id = 54 });
            DBlist.Add(new DBList() { DBNum = "055", id = 55 });
            DBlist.Add(new DBList() { DBNum = "056", id = 56 });
            DBlist.Add(new DBList() { DBNum = "057", id = 57 });
            DBlist.Add(new DBList() { DBNum = "058", id = 58 });
            DBlist.Add(new DBList() { DBNum = "059", id = 59 });
            DBlist.Add(new DBList() { DBNum = "060", id = 60 });
            DBlist.Add(new DBList() { DBNum = "061", id = 61 });
            DBlist.Add(new DBList() { DBNum = "062", id = 62 });
            DBlist.Add(new DBList() { DBNum = "063", id = 63 });
            DBlist.Add(new DBList() { DBNum = "064", id = 64 });
            DBlist.Add(new DBList() { DBNum = "065", id = 65 });
            DBlist.Add(new DBList() { DBNum = "066", id = 66 });
            DBlist.Add(new DBList() { DBNum = "067", id = 67 });
            DBlist.Add(new DBList() { DBNum = "068", id = 68 });
            DBlist.Add(new DBList() { DBNum = "069", id = 69 });
            DBlist.Add(new DBList() { DBNum = "070", id = 70 });
            DBlist.Add(new DBList() { DBNum = "071", id = 71 });
            DBlist.Add(new DBList() { DBNum = "072", id = 72 });
            DBlist.Add(new DBList() { DBNum = "073", id = 73 });
            DBlist.Add(new DBList() { DBNum = "074", id = 74 });
            DBlist.Add(new DBList() { DBNum = "075", id = 75 });
            DBlist.Add(new DBList() { DBNum = "076", id = 76 });
            DBlist.Add(new DBList() { DBNum = "077", id = 77 });
            DBlist.Add(new DBList() { DBNum = "078", id = 78 });
            DBlist.Add(new DBList() { DBNum = "079", id = 79 });
            DBlist.Add(new DBList() { DBNum = "080", id = 80 });
            DBlist.Add(new DBList() { DBNum = "081", id = 81 });
            DBlist.Add(new DBList() { DBNum = "082", id = 82 });
            DBlist.Add(new DBList() { DBNum = "083", id = 83 });
            DBlist.Add(new DBList() { DBNum = "084", id = 84 });
            DBlist.Add(new DBList() { DBNum = "085", id = 85 });
            DBlist.Add(new DBList() { DBNum = "086", id = 86 });
            DBlist.Add(new DBList() { DBNum = "087", id = 87 });
            DBlist.Add(new DBList() { DBNum = "088", id = 88 });
            DBlist.Add(new DBList() { DBNum = "089", id = 89 });
            DBlist.Add(new DBList() { DBNum = "090", id = 90 });
            DBlist.Add(new DBList() { DBNum = "091", id = 91 });
            DBlist.Add(new DBList() { DBNum = "092", id = 92 });
            DBlist.Add(new DBList() { DBNum = "093", id = 93 });
            DBlist.Add(new DBList() { DBNum = "094", id = 94 });
            DBlist.Add(new DBList() { DBNum = "095", id = 95 });
            DBlist.Add(new DBList() { DBNum = "096", id = 96 });
            DBlist.Add(new DBList() { DBNum = "097", id = 97 });
            DBlist.Add(new DBList() { DBNum = "098", id = 98 });
            DBlist.Add(new DBList() { DBNum = "099", id = 99 });
            DBlist.Add(new DBList() { DBNum = "201", id = 201 });
            DBlist.Add(new DBList() { DBNum = "202", id = 202 });
            DBlist.Add(new DBList() { DBNum = "203", id = 203 });
            DBlist.Add(new DBList() { DBNum = "204", id = 204 });
            DBlist.Add(new DBList() { DBNum = "205", id = 205 });
            DBlist.Add(new DBList() { DBNum = "206", id = 206 });
            DBlist.Add(new DBList() { DBNum = "207", id = 207 });
            DBlist.Add(new DBList() { DBNum = "208", id = 208 });
            DBlist.Add(new DBList() { DBNum = "209", id = 209 });
            DBlist.Add(new DBList() { DBNum = "210", id = 210 });
            DBlist.Add(new DBList() { DBNum = "211", id = 211 });
            DBlist.Add(new DBList() { DBNum = "212", id = 212 });
            DBlist.Add(new DBList() { DBNum = "213", id = 213 });
            DBlist.Add(new DBList() { DBNum = "214", id = 214 });
            DBlist.Add(new DBList() { DBNum = "215", id = 215 });
            DBlist.Add(new DBList() { DBNum = "216", id = 216 });
            DBlist.Add(new DBList() { DBNum = "217", id = 217 });
            DBlist.Add(new DBList() { DBNum = "218", id = 218 });
            DBlist.Add(new DBList() { DBNum = "219", id = 219 });
            DBlist.Add(new DBList() { DBNum = "220", id = 220 });
            DBlist.Add(new DBList() { DBNum = "221", id = 221 });
            DBlist.Add(new DBList() { DBNum = "222", id = 222 });
            DBlist.Add(new DBList() { DBNum = "223", id = 223 });
            DBlist.Add(new DBList() { DBNum = "224", id = 224 });
            DBlist.Add(new DBList() { DBNum = "225", id = 225 });
            DBlist.Add(new DBList() { DBNum = "226", id = 226 });
            DBlist.Add(new DBList() { DBNum = "227", id = 227 });
            DBlist.Add(new DBList() { DBNum = "228", id = 228 });
            DBlist.Add(new DBList() { DBNum = "229", id = 229 });
            DBlist.Add(new DBList() { DBNum = "230", id = 230 });
            DBlist.Add(new DBList() { DBNum = "231", id = 231 });
            DBlist.Add(new DBList() { DBNum = "232", id = 232 });
            DBlist.Add(new DBList() { DBNum = "233", id = 233 });
            DBlist.Add(new DBList() { DBNum = "234", id = 234 });
            DBlist.Add(new DBList() { DBNum = "235", id = 235 });
            DBlist.Add(new DBList() { DBNum = "236", id = 236 });
            DBlist.Add(new DBList() { DBNum = "237", id = 237 });
            DBlist.Add(new DBList() { DBNum = "238", id = 238 });
            DBlist.Add(new DBList() { DBNum = "239", id = 239 });
            DBlist.Add(new DBList() { DBNum = "240", id = 240 });
            DBlist.Add(new DBList() { DBNum = "241", id = 241 });
            DBlist.Add(new DBList() { DBNum = "242", id = 242 });
            DBlist.Add(new DBList() { DBNum = "243", id = 243 });
            DBlist.Add(new DBList() { DBNum = "244", id = 244 });
            DBlist.Add(new DBList() { DBNum = "245", id = 245 });
            DBlist.Add(new DBList() { DBNum = "246", id = 246 });
            DBlist.Add(new DBList() { DBNum = "247", id = 247 });
            DBlist.Add(new DBList() { DBNum = "248", id = 248 });
            DBlist.Add(new DBList() { DBNum = "249", id = 249 });
            DBlist.Add(new DBList() { DBNum = "250", id = 250 });
            DBlist.Add(new DBList() { DBNum = "251", id = 251 });
            DBlist.Add(new DBList() { DBNum = "252", id = 252 });
            DBlist.Add(new DBList() { DBNum = "253", id = 253 });
            DBlist.Add(new DBList() { DBNum = "254", id = 254 });
            DBlist.Add(new DBList() { DBNum = "255", id = 255 });
            DBlist.Add(new DBList() { DBNum = "256", id = 256 });
            DBlist.Add(new DBList() { DBNum = "257", id = 257 });
            DBlist.Add(new DBList() { DBNum = "258", id = 258 });
            DBlist.Add(new DBList() { DBNum = "259", id = 259 });
            DBlist.Add(new DBList() { DBNum = "260", id = 260 });
            DBlist.Add(new DBList() { DBNum = "261", id = 261 });
            DBlist.Add(new DBList() { DBNum = "262", id = 262 });
            DBlist.Add(new DBList() { DBNum = "263", id = 263 });
            DBlist.Add(new DBList() { DBNum = "264", id = 264 });
            DBlist.Add(new DBList() { DBNum = "265", id = 265 });
            DBlist.Add(new DBList() { DBNum = "266", id = 266 });
            DBlist.Add(new DBList() { DBNum = "267", id = 267 });
            DBlist.Add(new DBList() { DBNum = "268", id = 268 });
            DBlist.Add(new DBList() { DBNum = "269", id = 269 });
            DBlist.Add(new DBList() { DBNum = "270", id = 270 });
            DBlist.Add(new DBList() { DBNum = "271", id = 271 });
            DBlist.Add(new DBList() { DBNum = "272", id = 272 });
            DBlist.Add(new DBList() { DBNum = "273", id = 273 });
            DBlist.Add(new DBList() { DBNum = "274", id = 274 });
            DBlist.Add(new DBList() { DBNum = "275", id = 275 });
            DBlist.Add(new DBList() { DBNum = "276", id = 276 });
            DBlist.Add(new DBList() { DBNum = "277", id = 277 });
            DBlist.Add(new DBList() { DBNum = "278", id = 278 });
            DBlist.Add(new DBList() { DBNum = "279", id = 279 });
            DBlist.Add(new DBList() { DBNum = "280", id = 280 });
            DBlist.Add(new DBList() { DBNum = "281", id = 281 });
            DBlist.Add(new DBList() { DBNum = "282", id = 282 });
            DBlist.Add(new DBList() { DBNum = "283", id = 283 });
            DBlist.Add(new DBList() { DBNum = "284", id = 284 });
            DBlist.Add(new DBList() { DBNum = "285", id = 285 });
            DBlist.Add(new DBList() { DBNum = "286", id = 286 });
            DBlist.Add(new DBList() { DBNum = "287", id = 287 });
            DBlist.Add(new DBList() { DBNum = "288", id = 288 });
            DBlist.Add(new DBList() { DBNum = "289", id = 289 });
            DBlist.Add(new DBList() { DBNum = "290", id = 290 });
            DBlist.Add(new DBList() { DBNum = "291", id = 291 });
            DBlist.Add(new DBList() { DBNum = "292", id = 292 });
            DBlist.Add(new DBList() { DBNum = "293", id = 293 });
            DBlist.Add(new DBList() { DBNum = "294", id = 294 });
            DBlist.Add(new DBList() { DBNum = "295", id = 295 });
            DBlist.Add(new DBList() { DBNum = "296", id = 296 });
            DBlist.Add(new DBList() { DBNum = "297", id = 297 });
            DBlist.Add(new DBList() { DBNum = "298", id = 298 });
            DBlist.Add(new DBList() { DBNum = "299", id = 299 });

            ViewData["DBList"] = DBlist.ToList();

            //Console.WriteLine();
            //foreach (Part aPart in parts)
            //{
            //    Console.WriteLine(aPart);
            //}



            return View();
            //return View(db.ManagerDBAccessBIs.ToList());
        }


        public ActionResult Products_Read([DataSourceRequest]DataSourceRequest request)
        {
            using (var vardb = new MedProDBEntities())
            {
                IQueryable<ManagerDBAccessBI> products = vardb.ManagerDBAccessBIs;
                DataSourceResult result = products.ToDataSourceResult(request);
                return Json(result);
            }
        }



        //// GET: ManagerDBA/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
        //    if (managerDBAccessBI == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(managerDBAccessBI);
        //}

         //GET: ManagerDBA/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagerDBA/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,FvP,DB,Manager,AliasName,NTUser,active")] ManagerDBAccessBI managerDBAccessBI)
        {
            if (ModelState.IsValid)
            {
                db.ManagerDBAccessBIs.Add(managerDBAccessBI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(managerDBAccessBI);
        }

        //// GET: ManagerDBA/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
        //    if (managerDBAccessBI == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(managerDBAccessBI);
        //}

        // POST: ManagerDBA/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,FvP,DB,Manager,AliasName,NTUser,active")] ManagerDBAccessBI managerDBAccessBI)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(managerDBAccessBI).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(managerDBAccessBI);
        //}

        //// GET: ManagerDBA/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
        //    if (managerDBAccessBI == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(managerDBAccessBI);
        //}

        // POST: ManagerDBA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ManagerDBAccessBI managerDBAccessBI = db.ManagerDBAccessBIs.Find(id);
            db.ManagerDBAccessBIs.Remove(managerDBAccessBI);
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

        public ActionResult Products_Update([DataSourceRequest]DataSourceRequest request, ManagerDBAccessBIViewModel product)
        {
            if (ModelState.IsValid)
            {
                using (var varProducts = new MedProDBEntities())
                {
                    // Create a new Product entity and set its properties from the posted ProductViewModel
                    var entity = new ManagerDBAccessBI
                    {
                        id = product.id,
                        FvP = product.FvP,
                        DB = product.DB,
                        Manager = product.Manager,
                        AliasName = product.AliasName,
                        NTUser = User.Identity.Name,
                        active = product.active
                    };
                    // Attach the entity
                    varProducts.ManagerDBAccessBIs.Attach(entity);
                    // Change its state to Modified so Entity Framework can update the existing product instead of creating a new one
                    varProducts.Entry(entity).State = EntityState.Modified;
                    // Or use ObjectStateManager if using a previous version of Entity Framework
                    // northwind.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                    //entity.CollNoteType = entity.CollNoteType.Replace("'", "");
                    //entity.Code = entity.Code.Replace("'", "");
                    //entity.CollNoteCat = entity.CollNoteCat.Replace("'", "");
                    //entity.Priority = entity.Priority.Replace("'", "");




                    //string source = entity.Code;
                    //string[] stringSeparators = new string[] { "*" };
                    //string[] result;

                    //result = source.Split(stringSeparators, StringSplitOptions.None);

                    //if ((result.Count() - 1) >= 2)
                    //{
                    //    entity.ParsingYN = "Y";
                    //}
                    //else
                    //{
                    //    entity.ParsingYN = "N";
                    //}

                    // Update the entity in the database
                    varProducts.SaveChanges();
                }
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

    }


    public class Part
    {
        public int id { get; set; }
        public string FvPName { get; set; }
    }

    public class ManagerList
    {
        public int id { get; set; }
        public string ManagerName { get; set; }
    }

    public class DBList
    {
        public int id { get; set; }
        public string DBNum { get; set; }
    }

}
