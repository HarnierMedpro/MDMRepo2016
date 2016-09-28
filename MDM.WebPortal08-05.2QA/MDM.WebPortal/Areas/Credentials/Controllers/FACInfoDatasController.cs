using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using eMedServiceCorp.Tools;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class FACInfoDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       

        // GET: PlaceOfServices/FACInfoDatas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var currentLocationPos = await db.MasterPOS.FindAsync(id);
            //FACInfoData fACInfoData = await db.FACInfoDatas.FindAsync(id);
            if (currentLocationPos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            InfoData fACInfoData = currentLocationPos.InfoData;
            if (fACInfoData == null)
            {
                return View(new VMFACInfoData { MasterPOSID = id.Value, LicEffectiveDate = DateTime.Now, LicExpireDate = DateTime.Now });
            }
            VMFACInfoData toView = new VMFACInfoData
            {
                MasterPOSID = id.Value,
                InfoDataID = fACInfoData.InfoDataID,
                LicExpireDate = fACInfoData.LicExpireDate,
                LicEffectiveDate = fACInfoData.LicEffectiveDate,
                Taxonomy = fACInfoData.Taxonomy,
                LicNumCLIA_waiver = fACInfoData.LicNumCLIA_waiver,
                LicType = fACInfoData.LicType,
                StateLic = fACInfoData.StateLic
            };
            return View(toView);
        }

        // GET: PlaceOfServices/FACInfoDatas/Create
        /*Se va a crear un objeto FACInfoData si y solo si se esta modificando un objeto LocationsPOS; por ende se necesita conocer el ID de dicho LocationsPOS
         y ese es el valor que va a tomar la variable locPOS*/
        public async Task<ActionResult> Create(int? locPOS)
        {
            if (locPOS != null && await db.MasterPOS.FindAsync(locPOS) != null)
            {
                var allStates = new AllUSStates().states;
                ViewBag.StateLic = allStates;
                ViewBag.Facitity_DBs_IDPK = locPOS;
                return View();
            }
            /*Si locPOS es nulo quiere decir que no se definio a que LocationsPOS object se le va a crear un objeto FACInfoData, por eso se redirecciona al index de LocationsPOS
             y se le notifica al usuario que ocurrio un error*/
            TempData["Error"] = "Something failed. Please try again!";
            return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
        }

        // POST: PlaceOfServices/FACInfoDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MasterPOSID,InfoDataID,LicExpireDate,LicEffectiveDate,Taxonomy,LicNumCLIA_waiver,LicType,StateLic")]
            VMFACInfoData fACInfoData)
        {
            if (ModelState.IsValid && fACInfoData.MasterPOSID > 0)
            {
                if (fACInfoData.LicEffectiveDate < DateTime.Now && fACInfoData.LicEffectiveDate < fACInfoData.LicExpireDate)
                {
                    try
                    {
                        var currentLocPOS = await db.MasterPOS.FindAsync(fACInfoData.MasterPOSID);
                        ICollection<MasterPOS> Locations = new List<MasterPOS> { currentLocPOS };

                        if (currentLocPOS != null)
                        {
                            var toStore = new InfoData
                            {
                              
                                LicType = fACInfoData.LicType,
                                StateLic = fACInfoData.StateLic,
                                LicNumCLIA_waiver = fACInfoData.LicNumCLIA_waiver,
                                LicEffectiveDate = fACInfoData.LicEffectiveDate,
                                LicExpireDate = fACInfoData.LicExpireDate,
                                Taxonomy = fACInfoData.Taxonomy,
                                MasterPOS = Locations
                            };
                            db.InfoDatas.Add(toStore);
                            await db.SaveChangesAsync();
                            fACInfoData.InfoDataID = toStore.InfoDataID;

                            AuditToStore auditLog = new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "FACInfoData",
                                AuditAction = "I",
                                ModelPKey = toStore.InfoDataID,
                                tableInfos = new List<TableInfo>
                                {
                                   
                                    new TableInfo{Field_ColumName = "LicType", NewValue = toStore.LicType},
                                    new TableInfo{Field_ColumName = "StateLic", NewValue = toStore.StateLic},
                                    new TableInfo{Field_ColumName = "LicNumCLIA_waiver", NewValue = toStore.LicNumCLIA_waiver},
                                    new TableInfo{Field_ColumName = "LicEffectiveDate", NewValue = toStore.LicEffectiveDate.ToShortDateString()},
                                    new TableInfo{Field_ColumName = "LicExpireDate", NewValue = toStore.LicExpireDate.ToShortDateString()},
                                    new TableInfo{Field_ColumName = "Taxonomy", NewValue = toStore.Taxonomy},
                                  
                                }
                            };

                            var repository = new AuditLogRepository();

                            repository.AddAuditLogs(auditLog);

                            auditLog.AuditAction = "U";
                            auditLog.TableName = "MasterPOS";
                            auditLog.ModelPKey = currentLocPOS.MasterPOSID;
                            auditLog.tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "InfoData_InfoDataID", NewValue = toStore.InfoDataID.ToString(),}
                            };

                            repository.AddAuditLogs(auditLog);

                            return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                        }
                    }
                    catch (Exception)
                    {
                        var allStates = new AllUSStates().states;
                        ViewBag.StateLic = allStates;
                        ViewBag.Facitity_DBs_IDPK = fACInfoData.MasterPOSID;
                        ViewBag.Error = "Something failed. Please try again!";
                        return View(fACInfoData);
                    }
                }
                else
                {
                    var allUSStates = new AllUSStates().states;
                    ViewBag.StateLic = allUSStates;
                    ViewBag.Facitity_DBs_IDPK = fACInfoData.MasterPOSID;
                    ViewBag.Error = "Invalid Date(s). Please try again!";
                    return View(fACInfoData);
                }

            }
            var allStates1 = new AllUSStates().states;
            ViewBag.StateLic = allStates1;
            ViewBag.Facitity_DBs_IDPK = fACInfoData.MasterPOSID;
            return View(fACInfoData);
        }

        // GET: PlaceOfServices/FACInfoDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoData storedInDb = await db.InfoDatas.FindAsync(id);
            if (storedInDb == null)
            {
                return HttpNotFound();
            }
            var toView = new VMFACInfoData
            {
                InfoDataID = storedInDb.InfoDataID,
                LicType = storedInDb.LicType,
                StateLic = storedInDb.StateLic,
                LicNumCLIA_waiver = storedInDb.LicNumCLIA_waiver,
                LicEffectiveDate = storedInDb.LicEffectiveDate,
                LicExpireDate = storedInDb.LicExpireDate,
                Taxonomy = storedInDb.Taxonomy
                
            };

            var allStates = new AllUSStates().states;
            if (toView.StateLic != null && allStates.Find(x => x.Value == toView.StateLic) != null)
            {
                allStates.Find(x => x.Value == toView.StateLic).Selected = true;
            }
            ViewBag.StateLic = allStates;
            return View(toView);
        }

        // POST: PlaceOfServices/FACInfoDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MasterPOSID,InfoDataID,LicExpireDate,LicEffectiveDate,Taxonomy,LicNumCLIA_waiver,LicType,StateLic")] VMFACInfoData toUpdate)
        {
            if (ModelState.IsValid)
            {
                if (toUpdate.LicEffectiveDate < DateTime.Now && toUpdate.LicEffectiveDate < toUpdate.LicExpireDate && toUpdate.LicExpireDate >= DateTime.Now)
                {
                    try
                    {
                        var storedInDb = await db.InfoDatas.FindAsync(toUpdate.InfoDataID);

                        List<TableInfo> tableColumnInfos = new List<TableInfo>();
                     
                        if (storedInDb.LicType != toUpdate.LicType)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicType", OldValue = storedInDb.LicType, NewValue = toUpdate.LicType });
                            storedInDb.LicType = toUpdate.LicType;
                        }
                        if (storedInDb.StateLic != toUpdate.StateLic)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "StateLic", OldValue = storedInDb.StateLic, NewValue = toUpdate.StateLic });
                            storedInDb.StateLic = toUpdate.StateLic;
                        }
                        if (storedInDb.LicNumCLIA_waiver != toUpdate.LicNumCLIA_waiver)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicNumCLIA_waiver", OldValue = storedInDb.LicNumCLIA_waiver, NewValue = toUpdate.LicNumCLIA_waiver });
                            storedInDb.LicNumCLIA_waiver = toUpdate.LicNumCLIA_waiver;
                        }
                        if (storedInDb.LicEffectiveDate != toUpdate.LicEffectiveDate)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicEffectiveDate", OldValue = storedInDb.LicEffectiveDate.ToShortDateString(), NewValue = toUpdate.LicEffectiveDate.ToShortDateString() });
                            storedInDb.LicEffectiveDate = toUpdate.LicEffectiveDate;
                        }
                        if (storedInDb.LicExpireDate != toUpdate.LicExpireDate)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "LicExpireDate", OldValue = storedInDb.LicExpireDate.ToShortDateString(), NewValue = toUpdate.LicExpireDate.ToShortDateString() });
                            storedInDb.LicExpireDate = toUpdate.LicExpireDate;
                        }
                        if (storedInDb.Taxonomy != toUpdate.Taxonomy)
                        {
                            tableColumnInfos.Add(new TableInfo { Field_ColumName = "Taxonomy", OldValue = storedInDb.Taxonomy, NewValue = toUpdate.Taxonomy });
                            storedInDb.Taxonomy = toUpdate.Taxonomy;
                        }
                        
                        db.InfoDatas.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "InfoData",
                            AuditAction = "U",
                            ModelPKey = storedInDb.InfoDataID,
                            tableInfos = tableColumnInfos
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);

                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                    }
                    catch (Exception)
                    {
                        ViewBag.Error = "Something failed. Please try again!";
                        var allUSStates = new AllUSStates().states;
                        if (toUpdate.StateLic != null && allUSStates.Find(x => x.Value == toUpdate.StateLic) != null)
                        {
                            allUSStates.Find(x => x.Value == toUpdate.StateLic).Selected = true;
                        }
                        ViewBag.StateLic = allUSStates;
                        return View(toUpdate);
                    }
                }

            }
            var allStates = new AllUSStates().states;
            if (toUpdate.StateLic != null && allStates.Find(x => x.Value == toUpdate.StateLic) != null)
            {
                allStates.Find(x => x.Value == toUpdate.StateLic).Selected = true;
            }
            ViewBag.StateLic = allStates;
            return View(toUpdate);
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
