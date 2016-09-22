//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using MDM.WebPortal.Models.FromDB;
//using MDM.WebPortal.Models.FromZoomDB;
//using MDM.WebPortal.Models.ViewModel;
//using Kendo.Mvc.UI;
//using Kendo.Mvc.Extensions;
//using System.Data.SqlClient;
//using MDM.WebPortal.Areas.AudiTrails.Controllers;
//using MDM.WebPortal.Areas.AudiTrails.Models;
//using MDM.WebPortal.Data_Annotations;
//using MDM.WebPortal.DAL;
//using Microsoft.AspNet.Identity;

//namespace MDM.WebPortal.Controllers.APP
//{
//    [SetPermissions]
//    public class MDM_POS_Name_DBPOS_grpController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();
      
//        public ActionResult Index()
//        {
//            ViewData["DB"] = db.DBLists.Select(x => new { DB_ID = x.DB_ID, DB = x.DB }).ToList();
//            ViewData["POS"] = db.MDM_POS_ListName.Select(x => new { MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.PosName });             
//            return View();
//        }

//        //GET: All POS_DB_FAC relationship
//        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
//        {
//            return Json(db.MDM_POS_Name_DBPOS_grp.ToDataSourceResult(request, x => new VMPOS_Name_DBPOS_grp
//            {
//                DB_ID = x.DB_ID,
//                MDMPOS_ListNameID = x.MDMPOS_ListNameID,
//                FacilityID = x.FacilityID,
//                Extra = x.Extra,
//                Active = x.Active,
//                MDMPOS_NameID = x.MDMPOS_NameID
//            }), JsonRequestBehavior.AllowGet);
//        }

//         public async Task<ActionResult> Update_Entity([DataSourceRequest] DataSourceRequest request,
//                    [Bind(Include = "MDMPOS_NameID,DB_ID,FacilityID,MDMPOS_ListNameID,Active")] VMPOS_Name_DBPOS_grp mDmPosNameDbposGrp)
//            {
//                if (ModelState.IsValid)
//                {
//                    try
//                    {
//                        if (await db.MDM_POS_Name_DBPOS_grp.AnyAsync(x => x.FacilityID == mDmPosNameDbposGrp.FacilityID && x.DB_ID == mDmPosNameDbposGrp.DB_ID && x.MDMPOS_NameID != mDmPosNameDbposGrp.MDMPOS_NameID))
//                        {
//                            ModelState.AddModelError("", "This entity is already in database. Please try again!");
//                            return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
//                        }
//                        //var toStore = new MDM_POS_Name_DBPOS_grp
//                        //{
//                        //    MDMPOS_NameID = mDmPosNameDbposGrp.MDMPOS_NameID, //PK 
//                        //    MDMPOS_ListNameID = mDmPosNameDbposGrp.MDMPOS_ListNameID,//FK from dbo.MDM_POS_ListName
//                        //    FacilityID = mDmPosNameDbposGrp.FacilityID, //fk from DBO.Facility_DBs
//                        //    DB_ID = mDmPosNameDbposGrp.DB_ID, //FK from dbo.DBList
//                        //    Active = mDmPosNameDbposGrp.Active
//                        //};
//                        var toStore = await db.MDM_POS_Name_DBPOS_grp.FindAsync(mDmPosNameDbposGrp.MDMPOS_NameID);

//                        List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                        if (toStore.MDMPOS_ListNameID != mDmPosNameDbposGrp.MDMPOS_ListNameID)
//                        {
//                            tableColumnInfos.Add(new TableInfo
//                            {
//                                Field_ColumName = "MDMPOS_ListNameID",
//                                NewValue = mDmPosNameDbposGrp.MDMPOS_ListNameID.ToString(),
//                                OldValue = toStore.MDMPOS_ListNameID.ToString()
//                            });
//                            toStore.MDMPOS_ListNameID = mDmPosNameDbposGrp.MDMPOS_ListNameID;
//                        }
//                        if (toStore.FacilityID != mDmPosNameDbposGrp.FacilityID)
//                        {
//                            tableColumnInfos.Add(new TableInfo
//                            {
//                                Field_ColumName = "FacilityID",
//                                NewValue = mDmPosNameDbposGrp.FacilityID.ToString(),
//                                OldValue = toStore.FacilityID.ToString()
//                            });
//                            toStore.FacilityID = mDmPosNameDbposGrp.FacilityID;
//                        }
//                        if (toStore.DB_ID != mDmPosNameDbposGrp.DB_ID)
//                        {
//                            tableColumnInfos.Add(new TableInfo
//                            {
//                                Field_ColumName = "DB_ID",
//                                NewValue = mDmPosNameDbposGrp.DB_ID.ToString(),
//                                OldValue = toStore.DB_ID.ToString()
//                            });
//                            toStore.DB_ID = mDmPosNameDbposGrp.DB_ID;
//                        }
//                        if (toStore.Active != mDmPosNameDbposGrp.Active)
//                        {
//                            tableColumnInfos.Add(new TableInfo
//                            {
//                                Field_ColumName = "Active",
//                                NewValue = mDmPosNameDbposGrp.Active.ToString(),
//                                OldValue = toStore.Active.ToString()
//                            });
//                            toStore.Active = mDmPosNameDbposGrp.Active;
//                        }

//                        db.MDM_POS_Name_DBPOS_grp.Attach(toStore);
//                        db.Entry(toStore).State = EntityState.Modified;
//                        await db.SaveChangesAsync();

//                        AuditToStore auditLog = new AuditToStore
//                        {
//                            tableInfos = tableColumnInfos,
//                            AuditAction = "U",
//                            AuditDateTime = DateTime.Now,
//                            UserLogons = User.Identity.GetUserName(),
//                            ModelPKey = toStore.MDMPOS_NameID,
//                            TableName = "MDM_POS_DBPOS_grp"
//                        };

//                        new AuditLogRepository().AddAuditLogs(auditLog);
//                    }
//                    catch (Exception)
//                    {
//                        ModelState.AddModelError("", "Something failed. Please try again!");
//                        return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
//                    }
//                }
//                return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
//            }

//         public async Task<ActionResult> Create_Entity([DataSourceRequest] DataSourceRequest request,
//                     [Bind(Include = "MDMPOS_NameID,DB_ID,FacilityID,MDMPOS_ListNameID,Active")] VMPOS_Name_DBPOS_grp mDmPosNameDbposGrp)
//         {
//             if (ModelState.IsValid)
//             {
//                 try
//                 {
//                     if (await db.MDM_POS_Name_DBPOS_grp.AnyAsync(x => x.FacilityID == mDmPosNameDbposGrp.FacilityID && x.DB_ID == mDmPosNameDbposGrp.DB_ID ))
//                     {
//                         ModelState.AddModelError("", "This entity is already in database. Please try again!");
//                         return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
//                     }
//                     var toStore = new MDM_POS_Name_DBPOS_grp
//                     {
//                         MDMPOS_ListNameID = mDmPosNameDbposGrp.MDMPOS_ListNameID,//FK from dbo.MDM_POS_ListName
//                         FacilityID = mDmPosNameDbposGrp.FacilityID, //fk from DBO.Facility_DBs
//                         DB_ID = mDmPosNameDbposGrp.DB_ID, //FK
//                         Active = mDmPosNameDbposGrp.Active
//                     };
//                     db.MDM_POS_Name_DBPOS_grp.Add(toStore);
//                     await db.SaveChangesAsync();
//                     mDmPosNameDbposGrp.MDMPOS_NameID = toStore.MDMPOS_NameID;

//                     AuditToStore auditLog = new AuditToStore
//                     {
//                         tableInfos = new List<TableInfo>
//                         {
//                             new TableInfo{Field_ColumName = "MDMPOS_ListNameID", NewValue = toStore.MDMPOS_ListNameID.ToString()},
//                             new TableInfo{Field_ColumName = "FacilityID", NewValue = toStore.FacilityID.ToString()},
//                             new TableInfo{Field_ColumName = "DB_ID", NewValue = toStore.DB_ID.ToString()},
//                             new TableInfo{Field_ColumName = "Active", NewValue = toStore.Active.ToString()},
//                         },
//                         AuditAction = "I",
//                         AuditDateTime = DateTime.Now,
//                         UserLogons = User.Identity.GetUserName(),
//                         ModelPKey = toStore.MDMPOS_NameID,
//                         TableName = "MDM_POS_DBPOS_grp"
//                     };

//                     new AuditLogRepository().AddAuditLogs(auditLog);
//                 }
//                 catch (Exception)
//                 {
//                     ModelState.AddModelError("", "Something failed. Please try again!");
//                     return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
//                 }
//             }
//             return Json(new[] { mDmPosNameDbposGrp }.ToDataSourceResult(request, ModelState));
//         }

       


////--------------------------------------------Cascading DropDownList Kendo UI ASP.NET MVC5----------------------------------------------------------------------------------//
//        //First I get all POS
//        public JsonResult GetCascadePos([DataSourceRequest] DataSourceRequest request)
//        {
//            return Json(db.MDM_POS_ListName.Select(x => new { MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.PosName }).OrderBy(x => x.PosName).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
//        }

//        //Second I get all DBs
//        public JsonResult GetCascadeDbs([DataSourceRequest] DataSourceRequest request)
//        {
//            return Json(db.DBLists.Select(x => new {DB_ID = x.DB_ID, DB = x.DB}).OrderBy(x => x.DB).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
//        }

//        //GET: Facilities of a specific DB
//        [AcceptVerbs(HttpVerbs.Get)]
//        public async Task<ActionResult> GetFacByDbs(int DB_ID)
//        {
//            var currentDb = await db.DBLists.FindAsync(DB_ID);          
           
//            var dbFacs = db.Facitity_DBs.Where(x => x.DB == currentDb.DB).Select(x => new VMDB_Fac
//            {
//                Db = x.DB,
//                FacName = x.Fac_NAME,
//                FacilityID = x.Facility_ID.Value
//            }).ToList();

//            /*I have the facilities asociated with a specific DB (DB_ID)
//            var facOffirstDb = dbFacs.Where(x => x.Db == currentDb.DB).ToList();*/

//            /*I need to check if any of those facilities above are stored in MDM_POS_Name_DBPOS_grp table, 
//              and show to the user just the currentFacility and those that aren't stored yet in MDM_POS_Name_DBPOS_grp table
//            List<VMDB_Fac> stored = facOffirstDb.Where(item => db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == DB_ID && x.FacilityID == item.FacilityID && x.FacilityID != currentFacility) != null).ToList();

            
//            var toView = facOffirstDb.Except(stored).ToList();
//*/
//            return Json(dbFacs, JsonRequestBehavior.AllowGet);
//        }


////--------------------------------------------Cascading DropDownList Kendo UI ASP.NET MVC5----------------------------------------------------------------------------------//



//        //GET: Facilities
//        [AcceptVerbs(HttpVerbs.Get)]
//        public async Task<ActionResult> GetFacilities(string DB_ID)
//        {
//            if (!String.IsNullOrEmpty(DB_ID))
//            {
//                int id = 0;
//                bool isValid = Int32.TryParse(DB_ID, out id);

//                var toInt = Convert.ToInt32(DB_ID);

//                var database = await db.DBLists.FindAsync(toInt);

//                //USING EF 
//                var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
//                {
//                    Db = x.DB,
//                    FacName = x.Fac_NAME,
//                    FacilityID = x.Facility_ID.Value
//                }).ToList();

//                /*I have the facilities related with a specific DB*/
//                var facOffirstDb = dbFacs.Where(x => x.Db == database.DB).ToList();

//                /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
//                and show to the users that facilities that aren't stored yet*/
                
//                List<VMDB_Fac> stored = new List<VMDB_Fac>();
//                foreach (var item in facOffirstDb)
//                {
//                 if (db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == toInt && x.FacilityID == item.FacilityID) != null)
//                    {
//                      stored.Add(item);
//                    }
//                }

//                var toView = facOffirstDb.Except(stored).ToList();

//                var facilities = new SelectList(toView, "FacilityId", "FacName");

//                return Json(facilities, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                return View("Create");    
//            }
//        }

//        // GET: MDM_POS_Name_DBPOS_grp/Create
//        public ActionResult Create()
//        {
//            /*Obtain all the POS*/
//            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName");

//            /*Obtain all DBs*/
//            var dBs = db.DBLists.Select(x => new VMDB_Name {DB_ID  = x.DB_ID, DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
//            ViewBag.DB_ID = new SelectList(dBs, "DB_ID", "DB_Name");

//            /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
//            var firstDb = dBs.First();//puede ser null

//            /*I have all the DBs with their Facilities*/
           
//            //USING EF 
//            var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
//            {
//                Db = x.DB,
//                FacName = x.Fac_NAME,
//                FacilityID = x.Facility_ID.Value
//            }).ToList();

//            /*I have the facilities related with a specific DB*/
//            var facOffirstDb = dbFacs.Where(x => x.Db == firstDb.DB).ToList();
                   

//            /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
//             and show to the users that facilities that aren't stored yet*/
//            var dataBases = db.DBLists.ToList();
//            List<VMDB_Fac> stored = new List<VMDB_Fac>();
//            foreach (var item in facOffirstDb)
//            {
//                var dataB = dataBases.FirstOrDefault(x => item != null && x.DB == item.Db);
//                if (dataB != null)
//                {
//                    var dbID = dataB.DB_ID;
//                    if (
//                        db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
//                        null)
//                    {
//                        stored.Add(item);
//                    }
//                }
//            }

//            var toView = facOffirstDb.Except(stored).ToList();

//            ViewBag.FacilityID = new SelectList(toView, "FacilityId", "FacilityId");

//            return View();         

//        }

//        // POST: MDM_POS_Name_DBPOS_grp/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create([Bind(Include = "MDMPOS_NameID,DB_ID,FacilityID,Extra,MDMPOS_ListNameID")] MDM_POS_Name_DBPOS_grp mDM_POS_Name_DBPOS_grp)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (!db.MDM_POS_Name_DBPOS_grp.Any(x => x.FacilityID == mDM_POS_Name_DBPOS_grp.FacilityID && x.DB_ID == mDM_POS_Name_DBPOS_grp.DB_ID))
//                    {
//                        mDM_POS_Name_DBPOS_grp.Active = true;
//                        db.MDM_POS_Name_DBPOS_grp.Add(mDM_POS_Name_DBPOS_grp);
//                        await db.SaveChangesAsync();
//                        return RedirectToAction("Index");
//                    }
//                    ViewBag.Error = "Duplicate data. Please try again!";
//                    /*Obtain all the POS*/
//                    ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);

//                    /*Obtain all DBs*/
//                    var dBs = db.DBLists.ToList().ConvertAll(x => new VMDB_Name { DB_ID = x.DB_ID, DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
//                    ViewBag.DB_ID = new SelectList(dBs, "DB_ID", "DB_Name", mDM_POS_Name_DBPOS_grp.DB_ID);


//                    /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
//                    var firstDb = db.DBLists.Find(mDM_POS_Name_DBPOS_grp.DB_ID);

//                    /*I have all the DBs with their Facilities*/
                    
//                    //USING EF 
//                    var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
//                    {
//                        Db = x.DB,
//                        FacName = x.Fac_NAME,
//                        FacilityID = x.Facility_ID.Value
//                    }).ToList();

//                    /*I have the facilities related with a specific DB*/
//                    var facOffirstDb = dbFacs.Where(x => x.Db == firstDb.DB).ToList();


//                    /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
//                     and show to the users that facilities that aren't stored yet*/
//                    var dataBases = db.DBLists.ToList();
//                    List<VMDB_Fac> stored = new List<VMDB_Fac>();
//                    foreach (var item in facOffirstDb)
//                    {
//                        var dataB = dataBases.FirstOrDefault(x => item != null && x.DB == item.Db);
//                        if (dataB != null)
//                        {
//                            var dbID = dataB.DB_ID;
//                            if (
//                                db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
//                                null)
//                            {
//                                stored.Add(item);
//                            }
//                        }
//                    }

//                    var toView = facOffirstDb.Except(stored).ToList();

//                    ViewBag.FacilityID = new SelectList(toView, "FacilityId", "FacName");
//                    return View();
//                }
//                catch (Exception)
//                {
//                    ViewBag.Error = "Something failed. Please try again!";
//                    /*Obtain all the POS*/
//                    ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);

//                    /*Obtain all DBs*/
//                    var dBs = db.DBLists.ToList().ConvertAll(x => new VMDB_Name { DB_ID = x.DB_ID, DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
//                    ViewBag.DB_ID = new SelectList(dBs, "DB_ID", "DB_Name", mDM_POS_Name_DBPOS_grp.DB_ID);


//                    /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
//                    var firstDb = db.DBLists.Find(mDM_POS_Name_DBPOS_grp.DB_ID);

//                    /*I have all the DBs with their Facilities*/
                   

//                    //USING EF 
//                    var dbFacs = db.Facitity_DBs.Select(x => new VMDB_Fac
//                    {
//                        Db = x.DB,
//                        FacName = x.Fac_NAME,
//                        FacilityID = x.Facility_ID.Value
//                    }).ToList();

//                    /*I have the facilities related with a specific DB*/
//                    var facOffirstDb = dbFacs.Where(x => x.Db == firstDb.DB).ToList();


//                    /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
//                     and show to the users that facilities that aren't stored yet*/
//                    var dataBases = db.DBLists.ToList();
//                    List<VMDB_Fac> stored = new List<VMDB_Fac>();
//                    foreach (var item in facOffirstDb)
//                    {
//                        var dataB = dataBases.FirstOrDefault(x => item != null && x.DB == item.Db);
//                        if (dataB != null)
//                        {
//                            var dbID = dataB.DB_ID;
//                            if (
//                                db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
//                                null)
//                            {
//                                stored.Add(item);
//                            }
//                        }
//                    }

//                    var toView = facOffirstDb.Except(stored).ToList();

//                    ViewBag.FacilityID = new SelectList(toView, "FacilityID", "FacName");
//                    return View();
//                }
               
//            }
//            /*Obtain all the POS*/
//            ViewBag.MDMPOS_ListNameID = new SelectList(db.MDM_POS_ListName, "MDMPOS_ListNameID", "PosName", mDM_POS_Name_DBPOS_grp.MDMPOS_ListNameID);

//            /*Obtain all DBs*/
//            var dBs2 = db.DBLists.Select(x => new VMDB_Name { DB = x.DB, DB_Name = x.DB + " " + x.databaseName });
//            var dbName2 = db.DBLists.Find(mDM_POS_Name_DBPOS_grp.DB_ID).DB;
//            ViewBag.DB_ID = new SelectList(dBs2, "DB", "DB_Name", dbName2);

//            /*I need to know who is the first Db to show in Facility dropdown just the facilities related to the first DB*/
//            var firstDb2 = (SelectList) ViewBag.DB_ID; 

//            /*I have all the DBs with their Facilities*/
            
//            //USING EF 
//            var dbFacs2 = db.Facitity_DBs.Select(x => new VMDB_Fac
//            {
//                Db = x.DB,
//                FacName = x.Fac_NAME,
//                FacilityID = x.Facility_ID.Value
//            }).ToList();

//            /*I have the facilities related with a specific DB*/
//            var facOffirstDb2 = dbFacs2.Where(x => x.Db == firstDb2.First().Value.ToString()).ToList();


//            /*I need to check if any of those facilities bellow are stored in MDM_POS_Name_DBPOS_grp table, 
//             and show to the users that facilities that aren't stored yet*/
//            var dataBases2 = db.DBLists.ToList();
//            List<VMDB_Fac> stored2 = new List<VMDB_Fac>();
//            foreach (var item in facOffirstDb2)
//            {
//                var dataB = dataBases2.FirstOrDefault(x => item != null && x.DB == item.Db);
//                if (dataB != null)
//                {
//                    var dbID = dataB.DB_ID;
//                    if (
//                        db.MDM_POS_Name_DBPOS_grp.FirstOrDefault(x => x.DB_ID == dbID && x.FacilityID == item.FacilityID) !=
//                        null)
//                    {
//                        stored2.Add(item);
//                    }
//                }
//            }

//            var toView2 = facOffirstDb2.Except(stored2).ToList();

//            ViewBag.FacilityID = new SelectList(toView2, "FacilityID", "FacName");

//            return View();
//        }

       
      


//        /*Get all the POS without duplicates values*/
//        public async Task<ActionResult> POS([DataSourceRequest] DataSourceRequest request)
//        {
//            var POS = await db.MDM_POS_Name_DBPOS_grp.Include(x => x.MDM_POS_ListName).Select(x => x.MDM_POS_ListName).Distinct().ToListAsync();
//            return Json(POS.ToDataSourceResult(request, x => new VMMDM_POS_ListName
//            {
//                MDMPOS_ListNameID = x.MDMPOS_ListNameID,
//                PosName = x.PosName,
//                active = x.active
//            }));
//        }

//        /*Get al DB_Facilities of specific POS*/
//        public ActionResult DbPosGrp([DataSourceRequest] DataSourceRequest request, int? MDMPOS_ListNameID)
//        {
//            IQueryable<MDM_POS_Name_DBPOS_grp> Grp = db.MDM_POS_Name_DBPOS_grp;

//            if (MDMPOS_ListNameID != null)
//            {
//                Grp = Grp.Where(x => x.MDMPOS_ListNameID == MDMPOS_ListNameID);
//            }

//            return Json(Grp.ToDataSourceResult(request, x => new VMPOS_Name_DBPOS_grp
//            {
//                //PK for MDM_POS_Name_DBPOS_grp
//                MDMPOS_NameID = x.MDMPOS_NameID,
//                DB_ID = x.DB_ID,
//                FacilityID = x.FacilityID,
//                Extra = x.Extra,
//                MDMPOS_ListNameID = x.MDMPOS_ListNameID
//            }));
//        }



//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
