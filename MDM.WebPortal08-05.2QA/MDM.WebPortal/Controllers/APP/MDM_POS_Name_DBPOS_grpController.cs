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
using MDM.WebPortal.Models.FromZoomDB;
using MDM.WebPortal.Models.ViewModel;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data.SqlClient;
using MDM.WebPortal.DAL;

namespace MDM.WebPortal.Controllers.APP
{
    [AllowAnonymous]
    public class MDM_POS_Name_DBPOS_grpController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();
        private ZoomDBEntities dbZoom = new ZoomDBEntities();

       

        public ActionResult Index()
        {

            ViewData["DB"] = db.DBLists.Select(x => new {DB_ID = x.DB_ID, DB = x.DB}).ToList();
            ViewData["POS"] = db.MDM_POS_ListName.Select(x => new {MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.PosName});          
            
            
            return View();
        }

        //GET: All POS_DB_FAC relationship
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.MDM_POS_Name_DBPOS_grp.ToDataSourceResult(request, x => new VMPOS_Name_DBPOS_grp
            {
                DB_ID = x.DB_ID,
                MDMPOS_ListNameID = x.MDMPOS_ListNameID,
                FacilityID = x.FacilityID,
                Extra = x.Extra,
                Active = x.Active,
                MDMPOS_NameID = x.MDMPOS_NameID
            }), JsonRequestBehavior.AllowGet);
        }

        /*Get all the POS without duplicates values*/
        public async Task<ActionResult> POS([DataSourceRequest] DataSourceRequest request)
        {
            var POS = await db.MDM_POS_Name_DBPOS_grp.Include(x => x.MDM_POS_ListName).Select(x => x.MDM_POS_ListName).Distinct().ToListAsync();
            return Json(POS.ToDataSourceResult(request, x => new VMMDM_POS_ListName
            {
                MDMPOS_ListNameID = x.MDMPOS_ListNameID,
                PosName = x.PosName,
                active = x.active
            }));
        }

        /*Get al DB_Facilities of specific POS*/
        public ActionResult DbPosGrp([DataSourceRequest] DataSourceRequest request, int? MDMPOS_ListNameID)
        {
            IQueryable<MDM_POS_Name_DBPOS_grp> Grp = db.MDM_POS_Name_DBPOS_grp;

            if (MDMPOS_ListNameID != null)
            {
                Grp = Grp.Where(x => x.MDMPOS_ListNameID == MDMPOS_ListNameID);
            }

            return Json(Grp.ToDataSourceResult(request, x => new VMPOS_Name_DBPOS_grp
            {
                //PK for MDM_POS_Name_DBPOS_grp
                MDMPOS_NameID = x.MDMPOS_NameID,
                DB_ID = x.DB_ID,
                FacilityID = x.FacilityID,
                Extra = x.Extra,
                MDMPOS_ListNameID = x.MDMPOS_ListNameID
            }));
        }

        //Update an object

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MDMPOS_NameID,DB_ID,FacilityID,Extra,MDMPOS_ListNameID,Active")] MDM_POS_Name_DBPOS_grp
                mDmPosNameDbposGrp)
        {
            try
            {
                if (mDmPosNameDbposGrp != null && ModelState.IsValid)
                {
                    var stored = db.MDM_POS_Name_DBPOS_grp.Find(mDmPosNameDbposGrp.MDMPOS_NameID);
                    List<MDM_POS_Name_DBPOS_grp> contain = new List<MDM_POS_Name_DBPOS_grp>(){stored};

                    if (db.MDM_POS_Name_DBPOS_grp.ToList().Except(contain).FirstOrDefault(x => x.FacilityID == mDmPosNameDbposGrp.FacilityID && x.DB_ID == mDmPosNameDbposGrp.DB_ID) == null)
                    {
                        string[] fieldsToBind = new string[] { "DB_ID", "FacilityID", "MDMPOS_ListNameID", "Active" };
                        if (TryUpdateModel(stored, fieldsToBind))
                        {
                            db.Entry(stored).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request));
                        }
                       
                    }
                    ModelState.AddModelError("", "This entity is already in database.");
                    return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
                }
                return Json(new[] {mDmPosNameDbposGrp}.ToDataSourceResult(request, ModelState));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something failed. Please try again!");
                return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
            }
        }

        // GET: MDM_POS_Name_DBPOS_grp/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp = await db.MDM_POS_Name_DBPOS_grp.FindAsync(id);
            if (mDM_POS_Name_DBPOS_grp == null)
            {
                return HttpNotFound();
            }
            return View(mDM_POS_Name_DBPOS_grp);
        }


//--------------------------------------------Cascading DropDownList Kendo UI ASP.NET MVC5----------------------------------------------------------------------------------//
        //First I get all POS
        public JsonResult GetCascadePos()
        {
            return Json(db.MDM_POS_ListName.Select(x => new { MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.MDMPOS_ListNameID.ToString() + "_" + x.PosName }), JsonRequestBehavior.AllowGet);
        }

        //Second I get all DBs
        public JsonResult GetCascadeDbs()
        {
            return Json(db.DBLists.Select(x => new {DB_ID = x.DB_ID, DB = x.DB}), JsonRequestBehavior.AllowGet);
        }

        //GET: Facilities of a specific DB
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> GetFacByDbs(int DB_ID)
        {
            var currentDb = await db.DBLists.FindAsync(DB_ID);

            /*I have all the DBs with their Facilities*/
            //USING ADO.NET
            //_DALvmPosTab readView = new _DALvmPosTab();

            //var dbFacs = (from DataRow item in readView.Read().Rows
            //              select new VMDB_Fac
            //              {
            //                  Db = item.ItemArray[0].ToString(),
            //                  FacilityID = Convert.ToInt64(item.ItemArray[1]),
            //                  FacName = item.ItemArray[2].ToString()
            //              }).ToList();

            //USING EF 
            var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
            {
                Db = x.DB,
                FacName = x.Fac_NAME,
                FacilityID = x.Facility_ID.Value
            }).ToList();

            /*I have the facilities asociated with a specific DB (DB_ID)*/
            var facOffirstDb = dbFacs.Where(x => x.Db == currentDb.DB).ToList();

            /*I need to check if any of those facilities above are stored in MDM_POS_Name_DBPOS_grp table, 
              and show to the user just the currentFacility and those that aren't stored yet in MDM_POS_Name_DBPOS_grp table*/
            List<VMDB_Fac> stored = facOffirstDb.Where(item => db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == DB_ID && x.FacilityID == item.FacilityID /*&& x.FacilityID != currentFacility*/) != null).ToList();

            
            var toView = facOffirstDb.Except(stored).ToList();

            return Json(facOffirstDb, JsonRequestBehavior.AllowGet);
        }


//--------------------------------------------Cascading DropDownList Kendo UI ASP.NET MVC5----------------------------------------------------------------------------------//



        //GET: Facilities
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> GetFacilities(string DB_ID)
        {
            if (!String.IsNullOrEmpty(DB_ID))
            {
                int id = 0;
                bool isValid = Int32.TryParse(DB_ID, out id);

                var toInt = Convert.ToInt32(DB_ID);

                var database = await db.DBLists.FindAsync(toInt);

                /*I have all the DBs with their Facilities*/
                //_DALvmPosTab readView = new _DALvmPosTab();

                //var dbFacs = (from DataRow item in readView.Read().Rows
                //              select new VMDB_Fac
                //              {
                //                  Db = item.ItemArray[0].ToString(),
                //                  FacilityID = Convert.ToInt64(item.ItemArray[1]),
                //                  FacName = item.ItemArray[2].ToString()
                //              }).ToList();

                //USING EF 
                var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
                {
                    Db = x.DB,
                    FacName = x.Fac_NAME,
                    FacilityID = x.Facility_ID.Value
                }).ToList();

                /*I have the facilities related with a specific DB*/
                var facOffirstDb = dbFacs.Where(x => x.Db == database.DB).ToList();

                /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
                and show to the users that facilities that aren't stored yet*/
                
                List<VMDB_Fac> stored = new List<VMDB_Fac>();
                foreach (var item in facOffirstDb)
                {
                 if (db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == toInt && x.FacilityID == item.FacilityID) != null)
                    {
                      stored.Add(item);
                    }
                }

                var toView = facOffirstDb.Except(stored).ToList();

                var facilities = new SelectList(toView, "FacilityId", "FacName");

                return Json(facilities, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View("Create");    
            }
        }

        // GET: MDM_POS_Name_DBPOS_grp/Create
        public ActionResult Create()
        {
            /*Obtain all the POS*/
            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName");

            /*Obtain all DBs*/
            var dBs = db.DBLists.Select(x => new VMDB_Name {DB_ID  = x.DB_ID, DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
            ViewBag.DB_ID = new SelectList(dBs, "DB_ID", "DB_Name");

            /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
            var firstDb = dBs.First();//puede ser null

            /*I have all the DBs with their Facilities*/
            //_DALvmPosTab readView = new _DALvmPosTab();

            //var dbFacs = (from DataRow item in readView.Read().Rows
            //    select new VMDB_Fac
            //    {
            //        Db = item.ItemArray[0].ToString(), FacilityID = Convert.ToInt64(item.ItemArray[1]), FacName = item.ItemArray[2].ToString()
            //    }).ToList();

            //USING EF 
            var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
            {
                Db = x.DB,
                FacName = x.Fac_NAME,
                FacilityID = x.Facility_ID.Value
            }).ToList();

            /*I have the facilities related with a specific DB*/
            var facOffirstDb = dbFacs.Where(x => x.Db == firstDb.DB).ToList();
                   

            /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
             and show to the users that facilities that aren't stored yet*/
            var dataBases = db.DBLists.ToList();
            List<VMDB_Fac> stored = new List<VMDB_Fac>();
            foreach (var item in facOffirstDb)
            {
                var dataB = dataBases.FirstOrDefault(x => item != null && x.DB == item.Db);
                if (dataB != null)
                {
                    var dbID = dataB.DB_ID;
                    if (
                        db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
                        null)
                    {
                        stored.Add(item);
                    }
                }
            }

            var toView = facOffirstDb.Except(stored).ToList();

            ViewBag.FacilityID = new SelectList(toView, "FacilityId", "FacilityId");

            return View();         

        }

        // POST: MDM_POS_Name_DBPOS_grp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MDMPOS_NameID,DB_ID,FacilityID,Extra,MDMPOS_ListNameID")] MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!db.MDM_POS_Name_DBPOS_grp.Any(x => x.FacilityID == mDM_POS_Name_DBPOS_grp.FacilityID && x.DB_ID == mDM_POS_Name_DBPOS_grp.DB_ID))
                    {
                        mDM_POS_Name_DBPOS_grp.Active = true;
                        db.MDM_POS_Name_DBPOS_grp.Add(mDM_POS_Name_DBPOS_grp);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    ViewBag.Error = "Duplicate data. Please try again!";
                    /*Obtain all the POS*/
                    ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);

                    /*Obtain all DBs*/
                    var dBs = db.DBLists.ToList().ConvertAll(x => new VMDB_Name { DB_ID = x.DB_ID, DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
                    ViewBag.DB_ID = new SelectList(dBs, "DB_ID", "DB_Name", mDM_POS_Name_DBPOS_grp.DB_ID);


                    /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
                    var firstDb = db.DBLists.Find(mDM_POS_Name_DBPOS_grp.DB_ID);

                    /*I have all the DBs with their Facilities*/
                    //_DALvmPosTab readView = new _DALvmPosTab();

                    //var dbFacs = (from DataRow item in readView.Read().Rows
                    //              select new VMDB_Fac
                    //              {
                    //                  Db = item.ItemArray[0].ToString(),
                    //                  FacilityID = Convert.ToInt64(item.ItemArray[1]),
                    //                  FacName = item.ItemArray[2].ToString()
                    //              }).ToList();
                    //USING EF 
                    var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
                    {
                        Db = x.DB,
                        FacName = x.Fac_NAME,
                        FacilityID = x.Facility_ID.Value
                    }).ToList();

                    /*I have the facilities related with a specific DB*/
                    var facOffirstDb = dbFacs.Where(x => x.Db == firstDb.DB).ToList();


                    /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
                     and show to the users that facilities that aren't stored yet*/
                    var dataBases = db.DBLists.ToList();
                    List<VMDB_Fac> stored = new List<VMDB_Fac>();
                    foreach (var item in facOffirstDb)
                    {
                        var dataB = dataBases.FirstOrDefault(x => item != null && x.DB == item.Db);
                        if (dataB != null)
                        {
                            var dbID = dataB.DB_ID;
                            if (
                                db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
                                null)
                            {
                                stored.Add(item);
                            }
                        }
                    }

                    var toView = facOffirstDb.Except(stored).ToList();

                    ViewBag.FacilityID = new SelectList(toView, "FacilityId", "FacName");
                    return View();
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    /*Obtain all the POS*/
                    ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);

                    /*Obtain all DBs*/
                    var dBs = db.DBLists.ToList().ConvertAll(x => new VMDB_Name { DB_ID = x.DB_ID, DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
                    ViewBag.DB_ID = new SelectList(dBs, "DB_ID", "DB_Name", mDM_POS_Name_DBPOS_grp.DB_ID);


                    /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
                    var firstDb = db.DBLists.Find(mDM_POS_Name_DBPOS_grp.DB_ID);

                    /*I have all the DBs with their Facilities*/
                    //_DALvmPosTab readView = new _DALvmPosTab();

                    //var dbFacs = (from DataRow item in readView.Read().Rows
                    //              select new VMDB_Fac
                    //              {
                    //                  Db = item.ItemArray[0].ToString(),
                    //                  FacilityID = Convert.ToInt64(item.ItemArray[1]),
                    //                  FacName = item.ItemArray[2].ToString()
                    //              }).ToList();

                    //USING EF 
                    var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
                    {
                        Db = x.DB,
                        FacName = x.Fac_NAME,
                        FacilityID = x.Facility_ID.Value
                    }).ToList();

                    /*I have the facilities related with a specific DB*/
                    var facOffirstDb = dbFacs.Where(x => x.Db == firstDb.DB).ToList();


                    /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
                     and show to the users that facilities that aren't stored yet*/
                    var dataBases = db.DBLists.ToList();
                    List<VMDB_Fac> stored = new List<VMDB_Fac>();
                    foreach (var item in facOffirstDb)
                    {
                        var dataB = dataBases.FirstOrDefault(x => item != null && x.DB == item.Db);
                        if (dataB != null)
                        {
                            var dbID = dataB.DB_ID;
                            if (
                                db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
                                null)
                            {
                                stored.Add(item);
                            }
                        }
                    }

                    var toView = facOffirstDb.Except(stored).ToList();

                    ViewBag.FacilityID = new SelectList(toView, "FacilityID", "FacName");
                    return View();
                }
               
            }
            /*Obtain all the POS*/
            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);

            /*Obtain all DBs*/
            var dBs2 = db.DBLists.Select(x => new VMDB_Name { DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
            var dbName2 = db.DBLists.Find(mDM_POS_Name_DBPOS_grp.DB_ID).DB;
            ViewBag.DB_ID = new SelectList(dBs2, "DB", "DB_Name", dbName2);

            /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
            var firstDb2 = (SelectList) ViewBag.DB_ID; 

            /*I have all the DBs with their Facilities*/
            //_DALvmPosTab readView2 = new _DALvmPosTab();

            //var dbFacs2 = (from DataRow item in readView2.Read().Rows
            //              select new VMDB_Fac
            //              {
            //                  Db = item.ItemArray[0].ToString(),
            //                  FacilityID = Convert.ToInt64(item.ItemArray[1]),
            //                  FacName = item.ItemArray[2].ToString()
            //              }).ToList();
            //USING EF 
            var dbFacs2 = db.Facitity_DBs.Select(x => new VMDB_Fac
            {
                Db = x.DB,
                FacName = x.Fac_NAME,
                FacilityID = x.Facility_ID.Value
            }).ToList();

            /*I have the facilities related with a specific DB*/
            var facOffirstDb2 = dbFacs2.Where(x => x.Db == firstDb2.First().Value.ToString()).ToList();


            /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
             and show to the users that facilities that aren't stored yet*/
            var dataBases2 = db.DBLists.ToList();
            List<VMDB_Fac> stored2 = new List<VMDB_Fac>();
            foreach (var item in facOffirstDb2)
            {
                var dataB = dataBases2.FirstOrDefault(x => item != null && x.DB == item.Db);
                if (dataB != null)
                {
                    var dbID = dataB.DB_ID;
                    if (
                        db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
                        null)
                    {
                        stored2.Add(item);
                    }
                }
            }

            var toView2 = facOffirstDb2.Except(stored2).ToList();

            ViewBag.FacilityID = new SelectList(toView2, "FacilityID", "FacName");

            return View();
        }

        // GET: MDM_POS_Name_DBPOS_grp/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp = await db.MDM_POS_Name_DBPOS_grp.FindAsync(id);
            if (mDM_POS_Name_DBPOS_grp == null)
            {
                return HttpNotFound();
            }
            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);
            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", mDM_POS_Name_DBPOS_grp.DB_ID);
            ViewBag.FacilityID = new SelectList(dbZoom.Pos_Tab.Where(x => x.Facility_ID != null).Select(x => new VMFacility { FacilityID = x.Facility_ID.Value, FacName = x.Fac_NAME }).OrderBy(x => x.FacName), "FacilityID", "FacName", mDM_POS_Name_DBPOS_grp.FacilityID);
            return View(mDM_POS_Name_DBPOS_grp);
        }

        // POST: MDM_POS_Name_DBPOS_grp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MDMPOS_NameID,DB_ID,FacilityID,Extra,MDMPOS_ListNameID")] MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp)
        {
            if (ModelState.IsValid)
            {
                //
                try
                {
                    var storedInDB = await db.MDM_POS_Name_DBPOS_grp.FindAsync(mDM_POS_Name_DBPOS_grp.MDMPOS_NameID);
                    var list = new List<MDM_POS_Name_DBPOS_grp>(){
                       storedInDB
                    };

                    var distinct = db.MDM_POS_Name_DBPOS_grp.ToList().Except(list);

                    //var xyx = distinct.ToList();

                    if (distinct.Where(x => x.FacilityID == mDM_POS_Name_DBPOS_grp.FacilityID && x.FacilityID == mDM_POS_Name_DBPOS_grp.FacilityID).Any())
                    {                       
                        ViewBag.Error = "The Facility is already in Database.";
                        ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);
                        ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", mDM_POS_Name_DBPOS_grp.DB_ID);

                        ViewBag.FacilityID = new SelectList(dbZoom.Pos_Tab.Where(x => x.Facility_ID != null).Select(x => new VMFacility { FacilityID = x.Facility_ID.Value, FacName = x.Fac_NAME }).OrderBy(x => x.FacName), "FacilityID", "FacName");

                        return View(mDM_POS_Name_DBPOS_grp);                     
                    }
                    else
                    {
                        string[] fieldsToBind = new string[] { "DB_ID","FacilityID","Extra","MDMPOS_ListNameID" };
                        if (TryUpdateModel(storedInDB, fieldsToBind))
                        {
                            db.Entry(storedInDB).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Error = "Something failed. Please try again!";
                            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);
                            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", mDM_POS_Name_DBPOS_grp.DB_ID);

                            ViewBag.FacilityID = new SelectList(dbZoom.Pos_Tab.Where(x => x.Facility_ID != null).Select(x => new VMFacility { FacilityID = x.Facility_ID.Value, FacName = x.Fac_NAME }).OrderBy(x => x.FacName), "FacilityID", "FacName", mDM_POS_Name_DBPOS_grp.FacilityID);

                            return View(mDM_POS_Name_DBPOS_grp); 
                        }
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);
                    ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", mDM_POS_Name_DBPOS_grp.DB_ID);

                    ViewBag.FacilityID = new SelectList(dbZoom.Pos_Tab.Where(x => x.Facility_ID != null).Select(x => new VMFacility { FacilityID = x.Facility_ID.Value, FacName = x.Fac_NAME }).OrderBy(x => x.FacName), "FacilityID", "FacName", mDM_POS_Name_DBPOS_grp.FacilityID);

                    return View(mDM_POS_Name_DBPOS_grp); 
                }
                
            }
           
            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);
            ViewBag.DB_ID = new SelectList(db.DBLists, "DB_ID", "DB", mDM_POS_Name_DBPOS_grp.DB_ID);
            ViewBag.FacilityID = new SelectList(dbZoom.Pos_Tab.Where(x => x.Facility_ID != null).Select(x => new VMFacility { FacilityID = x.Facility_ID.Value, FacName = x.Fac_NAME }).OrderBy(x => x.FacName), "FacilityID", "FacName", mDM_POS_Name_DBPOS_grp.FacilityID);
            return View(mDM_POS_Name_DBPOS_grp); 
        }

        // GET: MDM_POS_Name_DBPOS_grp/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp = await db.MDM_POS_Name_DBPOS_grp.FindAsync(id);
            if (mDM_POS_Name_DBPOS_grp == null)
            {
                return HttpNotFound();
            }
            return View(mDM_POS_Name_DBPOS_grp);
        }

        // POST: MDM_POS_Name_DBPOS_grp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp = await db.MDM_POS_Name_DBPOS_grp.FindAsync(id);
            db.MDM_POS_Name_DBPOS_grp.Remove(mDM_POS_Name_DBPOS_grp);
            await db.SaveChangesAsync();
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
