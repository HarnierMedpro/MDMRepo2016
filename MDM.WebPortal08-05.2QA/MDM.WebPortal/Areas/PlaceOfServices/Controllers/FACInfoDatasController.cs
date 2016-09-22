//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using eMedServiceCorp.Tools;
//using MDM.WebPortal.Areas.AudiTrails.Controllers;
//using MDM.WebPortal.Areas.AudiTrails.Models;
//using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
//using MDM.WebPortal.Models.FromDB;
//using Microsoft.AspNet.Identity;

//namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
//{
//    public class FACInfoDatasController : Controller
//    {
//        private MedProDBEntities db = new MedProDBEntities();

//        // GET: PlaceOfServices/FACInfoDatas
//        public async Task<ActionResult> Index()
//        {
//            var fACInfoDatas = db.FACInfoDatas.Include(f => f.POSLOCExtraData);
//            return View(await fACInfoDatas.ToListAsync());
//        }

//        // GET: PlaceOfServices/FACInfoDatas/Details/5
//        public async Task<ActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return RedirectToAction("Index", "Error", new { area = "Error" });
//            }
//            var currentLocationPos = await db.LocationsPOS.FindAsync(id);
//            //FACInfoData fACInfoData = await db.FACInfoDatas.FindAsync(id);
//            if (currentLocationPos == null)
//            {
//                return RedirectToAction("Index", "Error", new { area = "Error" });
//            }
//            FACInfoData fACInfoData = currentLocationPos.FACInfoData;
//            if (fACInfoData == null)
//            {
//                return View( new VMFACInfoData { Facitity_DBs_IDPK = id.Value, LicEffectiveDate = DateTime.Now, LicExpireDate = DateTime.Now} );
//            }
//            VMFACInfoData toView = new VMFACInfoData
//            {
//                Facitity_DBs_IDPK = id.Value,
//                FACInfoDataID = fACInfoData.FACInfoDataID,
//                LicExpireDate = fACInfoData.LicExpireDate,
//                LicEffectiveDate = fACInfoData.LicEffectiveDate,
//                Taxonomy = fACInfoData.Taxonomy,
//                LicNumCLIA_waiver = fACInfoData.LicNumCLIA_waiver,
//                FAC_NPI_Num = fACInfoData.FAC_NPI_Num,
//                LicType = fACInfoData.LicType,
//                StateLic = fACInfoData.StateLic,
//                DocProviderName = fACInfoData.DocProviderName
//            };
//            return View(toView);
//        }

//        // GET: PlaceOfServices/FACInfoDatas/Create
//        /*Se va a crear un objeto FACInfoData si y solo si se esta modificando un objeto LocationsPOS; por ende se necesita conocer el ID de dicho LocationsPOS
//         y ese es el valor que va a tomar la variable locPOS*/
//        public async Task<ActionResult> Create(int? locPOS)
//        {
//            if (locPOS != null && await db.LocationsPOS.FindAsync(locPOS) != null)
//            {
//                var allStates = new AllUSStates().states;
//                ViewBag.StateLic = allStates;
//                ViewBag.Facitity_DBs_IDPK = locPOS;
//                return View();
//            }
//            /*Si locPOS es nulo quiere decir que no se definio a que LocationsPOS object se le va a crear un objeto FACInfoData, por eso se redirecciona al index de LocationsPOS
//             y se le notifica al usuario que ocurrio un error*/
//            TempData["Error"] = "Something failed. Please try again!";
//            return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
//        }

//        // POST: PlaceOfServices/FACInfoDatas/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create([Bind(Include = "FACInfoDataID,DocProviderName,LicType,StateLic,LicNumCLIA_waiver,LicEffectiveDate,LicExpireDate,Taxonomy,FAC_NPI_Num,Facitity_DBs_IDPK")] VMFACInfoData fACInfoData)
//        {
//            if (ModelState.IsValid && fACInfoData.Facitity_DBs_IDPK > 0 )
//            {
//                if (fACInfoData.LicEffectiveDate < DateTime.Now && fACInfoData.LicEffectiveDate < fACInfoData.LicExpireDate)
//                {
//                    try
//                    {
//                        var currentLocPOS = await db.LocationsPOS.FindAsync(fACInfoData.Facitity_DBs_IDPK);
//                        ICollection<LocationsPOS> Locations = new List<LocationsPOS> { currentLocPOS };

//                        if (currentLocPOS != null)
//                        {
//                            var toStore = new FACInfoData
//                            {
//                                DocProviderName = fACInfoData.DocProviderName,
//                                LicType = fACInfoData.LicType,
//                                StateLic = fACInfoData.StateLic,
//                                LicNumCLIA_waiver = fACInfoData.LicNumCLIA_waiver,
//                                LicEffectiveDate = fACInfoData.LicEffectiveDate,
//                                LicExpireDate = fACInfoData.LicExpireDate,
//                                Taxonomy = fACInfoData.Taxonomy,
//                                FAC_NPI_Num = fACInfoData.FAC_NPI_Num,
//                                LocationsPOS = Locations
//                            };
//                            db.FACInfoDatas.Add(toStore);
//                            await db.SaveChangesAsync();

//                            AuditToStore auditLog = new AuditToStore
//                            {
//                                UserLogons = User.Identity.GetUserName(),
//                                AuditDateTime = DateTime.Now,
//                                TableName = "FACInfoData",
//                                AuditAction = "I",
//                                ModelPKey = toStore.FACInfoDataID,
//                                tableInfos = new List<TableInfo>
//                                {
//                                    new TableInfo{Field_ColumName = "DocProviderName", NewValue = toStore.DocProviderName},
//                                    new TableInfo{Field_ColumName = "LicType", NewValue = toStore.LicType},
//                                    new TableInfo{Field_ColumName = "StateLic", NewValue = toStore.StateLic},
//                                    new TableInfo{Field_ColumName = "LicNumCLIA_waiver", NewValue = toStore.LicNumCLIA_waiver},
//                                    new TableInfo{Field_ColumName = "LicEffectiveDate", NewValue = toStore.LicEffectiveDate.ToShortDateString()},
//                                    new TableInfo{Field_ColumName = "LicExpireDate", NewValue = toStore.LicExpireDate.ToShortDateString()},
//                                    new TableInfo{Field_ColumName = "Taxonomy", NewValue = toStore.Taxonomy},
//                                    new TableInfo{Field_ColumName = "FAC_NPI_Num", NewValue = toStore.FAC_NPI_Num},
//                                }
//                            };

//                            var repository = new AuditLogRepository();

//                            repository.AddAuditLogs(auditLog);

//                            auditLog.AuditAction = "U";
//                            auditLog.TableName = "LocationsPOS";
//                            auditLog.ModelPKey = currentLocPOS.Facitity_DBs_IDPK;
//                            auditLog.tableInfos = new List<TableInfo>
//                            {
//                                new TableInfo{Field_ColumName = "FACInfoData_FACInfoDataID", NewValue = toStore.FACInfoDataID.ToString(),}
//                            };

//                            repository.AddAuditLogs(auditLog);

//                            return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
//                        }
//                    }
//                    catch (Exception)
//                    {
//                        var allStates = new AllUSStates().states;
//                        ViewBag.StateLic = allStates;
//                        ViewBag.Facitity_DBs_IDPK = fACInfoData.Facitity_DBs_IDPK;
//                        ViewBag.Error = "Something failed. Please try again!";
//                        return View(fACInfoData);
//                    }
//                }
//                else
//                {
//                    var allUSStates = new AllUSStates().states;
//                    ViewBag.StateLic = allUSStates;
//                    ViewBag.Facitity_DBs_IDPK = fACInfoData.Facitity_DBs_IDPK;
//                    ViewBag.Error = "Invalid Date(s). Please try again!";
//                    return View(fACInfoData);
//                }
                
//            }
//            var allStates1 = new AllUSStates().states;
//            ViewBag.StateLic = allStates1;
//            ViewBag.Facitity_DBs_IDPK = fACInfoData.Facitity_DBs_IDPK;
//            return View(fACInfoData);
//        }

//        // GET: PlaceOfServices/FACInfoDatas/Edit/5
//        public async Task<ActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            FACInfoData storedInDb = await db.FACInfoDatas.FindAsync(id);
//            if (storedInDb == null)
//            {
//                return HttpNotFound();
//            }
//            var toView = new VMFACInfoData
//            {
//                FACInfoDataID = storedInDb.FACInfoDataID,
//                DocProviderName = storedInDb.DocProviderName,
//                LicType = storedInDb.LicType,
//                StateLic = storedInDb.StateLic,
//                LicNumCLIA_waiver = storedInDb.LicNumCLIA_waiver,
//                LicEffectiveDate = storedInDb.LicEffectiveDate,
//                LicExpireDate = storedInDb.LicExpireDate,
//                Taxonomy = storedInDb.Taxonomy,
//                FAC_NPI_Num = storedInDb.FAC_NPI_Num
//            };

//            var allStates = new AllUSStates().states;
//            if (toView.StateLic != null && allStates.Find(x => x.Value == toView.StateLic) != null)
//            {
//              allStates.Find(x => x.Value == toView.StateLic).Selected = true;
//            }
//            ViewBag.StateLic = allStates;
//            return View(toView);
//        }

//        // POST: PlaceOfServices/FACInfoDatas/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Edit([Bind(Include = "FACInfoDataID,DocProviderName,LicType,StateLic,LicNumCLIA_waiver,LicEffectiveDate,LicExpireDate,Taxonomy,FAC_NPI_Num")] VMFACInfoData toUpdate)
//        {
//            if (ModelState.IsValid)
//            {
//                if (toUpdate.LicEffectiveDate < DateTime.Now && toUpdate.LicEffectiveDate < toUpdate.LicExpireDate && toUpdate.LicExpireDate >= DateTime.Now)
//                {
//                    try
//                    {
//                        var storedInDb = await db.FACInfoDatas.FindAsync(toUpdate.FACInfoDataID);

//                        List<TableInfo> tableColumnInfos = new List<TableInfo>();

//                        if (storedInDb.DocProviderName != toUpdate.DocProviderName)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "DocProviderName", OldValue = storedInDb.DocProviderName, NewValue = toUpdate.DocProviderName});
//                            storedInDb.DocProviderName = toUpdate.DocProviderName;
//                        }
//                        if (storedInDb.LicType != toUpdate.LicType)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicType", OldValue = storedInDb.LicType, NewValue = toUpdate.LicType });
//                            storedInDb.LicType = toUpdate.LicType;
//                        }
//                        if (storedInDb.StateLic != toUpdate.StateLic)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "StateLic", OldValue = storedInDb.StateLic, NewValue = toUpdate.StateLic });
//                            storedInDb.StateLic = toUpdate.StateLic;
//                        }
//                        if (storedInDb.LicNumCLIA_waiver != toUpdate.LicNumCLIA_waiver)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicNumCLIA_waiver", OldValue = storedInDb.LicNumCLIA_waiver, NewValue = toUpdate.LicNumCLIA_waiver });
//                            storedInDb.LicNumCLIA_waiver = toUpdate.LicNumCLIA_waiver;
//                        }
//                        if (storedInDb.LicEffectiveDate != toUpdate.LicEffectiveDate)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicEffectiveDate", OldValue = storedInDb.LicEffectiveDate.ToShortDateString(), NewValue = toUpdate.LicEffectiveDate.ToShortDateString() });
//                            storedInDb.LicEffectiveDate = toUpdate.LicEffectiveDate;
//                        }
//                        if (storedInDb.LicExpireDate != toUpdate.LicExpireDate)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicExpireDate", OldValue = storedInDb.LicExpireDate.ToShortDateString(), NewValue = toUpdate.LicExpireDate.ToShortDateString() });
//                            storedInDb.LicExpireDate = toUpdate.LicExpireDate;
//                        }
//                        if (storedInDb.Taxonomy != toUpdate.Taxonomy)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "Taxonomy", OldValue = storedInDb.Taxonomy, NewValue = toUpdate.Taxonomy });
//                            storedInDb.Taxonomy = toUpdate.Taxonomy;
//                        }
//                        if (storedInDb.FAC_NPI_Num != toUpdate.FAC_NPI_Num)
//                        {
//                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "FAC_NPI_Num", OldValue = storedInDb.FAC_NPI_Num, NewValue = toUpdate.FAC_NPI_Num });
//                            storedInDb.FAC_NPI_Num = toUpdate.FAC_NPI_Num;
//                        }
//                        db.FACInfoDatas.Attach(storedInDb);
//                        db.Entry(storedInDb).State = EntityState.Modified;
//                        await db.SaveChangesAsync();

//                        AuditToStore auditLog = new AuditToStore
//                        {
//                            UserLogons = User.Identity.GetUserName(),
//                            AuditDateTime = DateTime.Now,
//                            TableName = "FACInfoData",
//                            AuditAction = "U",
//                            ModelPKey = storedInDb.FACInfoDataID,
//                            tableInfos = tableColumnInfos
//                        };

//                        new AuditLogRepository().AddAuditLogs(auditLog);

//                        return RedirectToAction("Index","LocationsPOS", new {area ="PlaceOfServices"});
//                    }
//                    catch (Exception)
//                    {
//                        ViewBag.Error = "Something failed. Please try again!";
//                        var allUSStates = new AllUSStates().states;
//                        if (toUpdate.StateLic != null && allUSStates.Find(x => x.Value == toUpdate.StateLic) != null)
//                        {
//                            allUSStates.Find(x => x.Value == toUpdate.StateLic).Selected = true;
//                        }
//                        ViewBag.StateLic = allUSStates;
//                        return View(toUpdate);
//                    }
//                }
                
//            }
//            var allStates = new AllUSStates().states;
//            if (toUpdate.StateLic != null && allStates.Find(x => x.Value == toUpdate.StateLic) != null)
//            {
//                allStates.Find(x => x.Value == toUpdate.StateLic).Selected = true;
//            }
//            ViewBag.StateLic = allStates;
//            return View(toUpdate);
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
