using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels;
using MDM.WebPortal.Areas.PlaceOfServices.Tools;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Hubs;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel.Delete;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class MasterPOSController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();


        public ActionResult Index_MasterPOS()
        {
            ViewData["FvPLists"] = db.FvPLists.OrderBy(x => x.FvPName).Select(x => new VMFvpList { FvPID = x.FvPID, FvpName = x.FvPName });
            ViewData["Manager"] = db.Manager_Master.OrderBy(x => x.AliasName).Select(z => new VMManager_Master{ManagerID = z.ManagerID, AliasName = z.AliasName});
            ViewData["MedDirector"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider{ProvID = x.ProvID, ProviderName = x.ProviderName});
            ViewData["Database"] = db.DBLists.Select(x => new VMDBList {DB_ID = x.DB_ID, DB = x.DB});

            ViewData["LevOfCare"] = db.Lev_of_Care.OrderBy(x => x.LevOfCareName).Select(x => new VMLevelOfCare { LevOfCareID = x.LevOfCareID, LevOfCareName = x.LevOfCareName });
            ViewData["Service"] = db.MPServices.OrderBy(x => x.ServName).Select(x => new VMMPService { MPServID = x.MPServID, ServName = x.ServName });
            ViewData["Forms"] = db.FormsDicts.OrderBy(x => x.FormName).Select(x => new VMFormsDict{FormsID = x.FormsID, FormName = x.FormName});

           
            return View();
        }

        public ActionResult Read_MasterPOS([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.MasterPOS.Include(mm => mm.MasterPOS_MPServ)
                           .Include(ml => ml.MasterPOS_LevOfCare)
                           .Include(d => d.ZoomDB_POSID_grp)
                           .Include(s => s.DBList)
                           .Include(f => f.Forms_sent).OrderBy(d => d.PosMasterName)
                           .Select(x => new VMMasterPOS
                            {
                               MasterPOSID = x.MasterPOSID,
                               PosMasterName = x.PosMasterName,
                               active = x.active,
                               FvPList_FvPID = x.FvPList_FvPID,
                               ManagerID = x.Manager_Master_ManagerID,
                               ProvID = x.Providers_ProvID,
                               DB_ID = x.DBList_DB_ID,
                               Corporation = db.Corp_DBs.FirstOrDefault(d => d.DB_ID == x.DBList_DB_ID) != null ? db.Corp_DBs.FirstOrDefault(d => d.DB_ID == x.DBList_DB_ID).CorporateMasterList.CorporateName : "",
                               LevelOfCares = x.MasterPOS_LevOfCare.Select(d => new VMLevelOfCare { LevOfCareID = d.Lev_of_Care_LevOfCareID, LevOfCareName = d.Lev_of_Care.LevOfCareName }),
                               Services = x.MasterPOS_MPServ.Select(f => new VMMPService { MPServID = f.MPServices_MPServID, ServName = f.MPService.ServName }),
                               PosIDs = from a in x.ZoomDB_POSID_grp join b in db.Facitity_DBs on a.ZoomPos_ID equals b.Facility_ID where b.DB_ID == x.DBList_DB_ID
                                        select new VMZoomDB_POSID { ZoomPos_ID = b.Facility_ID, ZoomPos_Name = b.Fac_NAME},
                               Forms_Sents = x.Forms_sent.Select(d => new VMFormsDict{FormsID = d.FormsDict_FormsID, FormName = d.FormsDict.FormName})
                            }).OrderBy(f => f.PosMasterName).ToList();
           

            return Json(result.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Read_OwnersOfThisPos([DataSourceRequest] DataSourceRequest request, int masterPOSID)
        {
            var masterPOS = await db.MasterPOS.FindAsync(masterPOSID);
            var result = new List<VMContact>();
            if (masterPOS != null)
            {
                var corpDBs = db.Corp_DBs.Include(c => c.CorporateMasterList).Include(d => d.DBList).FirstOrDefault(x => x.DB_ID == masterPOS.DBList_DB_ID);
                if (corpDBs != null)
                {
                    var corpPOS = corpDBs.CorporateMasterList;
                    var allCorpContacts = db.Corp_Owner.Include(c => c.CorporateMasterList).Include(o => o.Contact).Where(c => c.corpID == corpPOS.corpID).Select(o => o.Contact);
                    var temp = from cnt in allCorpContacts
                               join ctype in db.ContactType_Contact.Include(t => t.ContactType).Include(c => c.Contact) on cnt
                                   equals ctype.Contact
                               where
                                   ctype.ContactType.ContactLevel.Equals("corporation",
                                       StringComparison.InvariantCultureIgnoreCase) &&
                                   ctype.ContactType.ContactType_Name.Equals("owner",
                                       StringComparison.InvariantCultureIgnoreCase) && cnt.active
                               select
                                   new VMContact
                                   {
                                       ContactID = cnt.ContactID,
                                       FName = cnt.FName,
                                       LName = cnt.LName,
                                       Email = cnt.Email,
                                       PhoneNumber = cnt.PhoneNumber,
                                       active = cnt.active
                                   };
                    result = temp.ToList();
                }
            }
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_POSFromCorp([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,PosMasterName,active")] VMMasterPOSPartial masterPos)
        {
            if (ModelState.IsValid)
            {
                if (await db.MasterPOS.AnyAsync(x => x.PosMasterName.Equals(masterPos.PosMasterName, StringComparison.InvariantCultureIgnoreCase) && x.MasterPOSID != masterPos.MasterPOSID))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var posNameStoredInDb = await db.MasterPOS.FindAsync(masterPos.MasterPOSID);
                        var tableColumnInfo = new List<TableInfo>();

                        if (!posNameStoredInDb.PosMasterName.Equals(masterPos.PosMasterName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "PosMasterName", OldValue = posNameStoredInDb.PosMasterName, NewValue = masterPos.PosMasterName });
                            posNameStoredInDb.PosMasterName = masterPos.PosMasterName;
                        }

                        if (posNameStoredInDb.active != masterPos.active)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "active", OldValue = posNameStoredInDb.active.ToString(), NewValue = masterPos.active.ToString() });
                            posNameStoredInDb.active = masterPos.active;
                        }

                        db.MasterPOS.Attach(posNameStoredInDb);
                        db.Entry(posNameStoredInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "MasterPOS",
                            AuditAction = "U",
                            ModelPKey = posNameStoredInDb.MasterPOSID,
                            tableInfos = tableColumnInfo
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();

                        //UpdatePosInCorHub.DoIfUpdatePosFromCorp(posNameStoredInDb.MasterPOSID, posNameStoredInDb.PosMasterName, posNameStoredInDb.active);

                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_MasterPOS([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,PosMasterName,active,FvPList_FvPID,ManagerID,ProvID,DB_ID,LevelOfCares,Services,PosIDs,Forms_Sents")] VMMasterPOS masterPos)
        {
            if (ModelState.IsValid)
            {
                if (await db.MasterPOS.AnyAsync(x => x.PosMasterName.Equals(masterPos.PosMasterName, StringComparison.InvariantCultureIgnoreCase) && x.MasterPOSID != masterPos.MasterPOSID))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        bool signalRUpdates = false;

                        var posNameStoredInDb = await db.MasterPOS.FindAsync(masterPos.MasterPOSID);
                        var corporation = db.Corp_DBs.FirstOrDefault(d => d.DB_ID == masterPos.DB_ID);
                        masterPos.Corporation = corporation != null ? corporation.CorporateMasterList.CorporateName : "";

                        var audiLogs = new List<AuditToStore>();
                        var tableColumnInfo = new List<TableInfo>();

                        var currentLevOfCare = posNameStoredInDb.MasterPOS_LevOfCare.Select(x => x.Lev_of_Care_LevOfCareID).ToList();
                        var levOfCareByParam = masterPos.LevelOfCares.Select(x => x.LevOfCareID).ToList();

                        var newLevIDs = levOfCareByParam.Except(currentLevOfCare).ToList();
                        var newLevToInsert = newLevIDs.Select(x => new MasterPOS_LevOfCare { Lev_of_Care_LevOfCareID = x, MasterPOS_MasterPOSID = masterPos.MasterPOSID }).ToList();
                        db.MasterPOS_LevOfCare.AddRange(newLevToInsert);
                        
                        var oldLevToDelete = currentLevOfCare.Except(levOfCareByParam).ToList();
                        foreach (var lc in oldLevToDelete)
                        {
                            var toDelete = await db.MasterPOS_LevOfCare.FirstOrDefaultAsync(x => x.Lev_of_Care_LevOfCareID == lc && x.MasterPOS_MasterPOSID == masterPos.MasterPOSID);
                            if (toDelete != null)
                            {
                                db.MasterPOS_LevOfCare.Remove(toDelete);
                                audiLogs.Add(new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    AuditAction = "D",
                                    TableName = "MasterPOS_LevOfCare",
                                    ModelPKey = toDelete.MasterPosLocID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "Lev_of_Care_LevOfCareID", OldValue = toDelete.Lev_of_Care_LevOfCareID.ToString()},
                                        new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", OldValue = toDelete.MasterPOS_MasterPOSID.ToString()},
                                    }
                                });
                            }
                        }

                        var currentServices = posNameStoredInDb.MasterPOS_MPServ.Select(x => x.MPServices_MPServID).ToList();
                        var servicesByParam = masterPos.Services.Select(x => x.MPServID).ToList();

                        var newServIds = servicesByParam.Except(currentServices).ToList();
                        var newServToInsert = newServIds.Select(x => new MasterPOS_MPServ { MPServices_MPServID = x, MasterPOS_MasterPOSID = masterPos.MasterPOSID }).ToList();
                        db.MasterPOS_MPServ.AddRange(newServToInsert);

                        var oldServToDelete = currentServices.Except(servicesByParam).ToList();
                        foreach (var sr in oldServToDelete)
                        {
                            var toDelete = await db.MasterPOS_MPServ.FirstOrDefaultAsync(x => x.MPServices_MPServID == sr && x.MasterPOS_MasterPOSID == masterPos.MasterPOSID);
                            if (toDelete != null)
                            {
                                db.MasterPOS_MPServ.Remove(toDelete);
                                audiLogs.Add(new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    AuditAction = "D",
                                    TableName = "MasterPOS_MPServ",
                                    ModelPKey = toDelete.MasterPosMPServID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "MPServices_MPServID", OldValue = toDelete.MPServices_MPServID.ToString()},
                                        new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", OldValue = toDelete.MasterPOS_MasterPOSID.ToString()},
                                    }
                                });

                            }
                        }

                        var currentForms = posNameStoredInDb.Forms_sent.Select(x => x.FormsDict_FormsID).ToList();
                        var formsByParam = masterPos.Forms_Sents.Select(x => x.FormsID).ToList();

                        var newFormsIds = formsByParam.Except(currentForms).ToList();
                        var newFormsToInsert = newFormsIds.Select(x => new Forms_sent { FormsDict_FormsID = x, MasterPOS_MasterPOSID = masterPos.MasterPOSID }).ToList();
                        db.Forms_sent.AddRange(newFormsToInsert);

                        var oldFormsToDelete = currentForms.Except(formsByParam).ToList();
                        foreach (var f in oldFormsToDelete)
                        {
                            var toDelete = await db.Forms_sent.FirstOrDefaultAsync(x => x.FormsDict_FormsID == f && x.MasterPOS_MasterPOSID == masterPos.MasterPOSID);
                            if (toDelete != null)
                            {
                                db.Forms_sent.Remove(toDelete);

                                audiLogs.Add(new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    AuditAction = "D",
                                    TableName = "Forms_sent",
                                    ModelPKey = toDelete.FormSentID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "FormsDict_FormsID", OldValue = toDelete.FormsDict_FormsID.ToString()},
                                        new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", OldValue = toDelete.MasterPOS_MasterPOSID.ToString()},
                                    }
                                });

                            }
                        }

                        var newPosIdsToInsert = new List<ZoomDB_POSID_grp>();
                        if (posNameStoredInDb.DBList_DB_ID != masterPos.DB_ID)//Si la BD cambia se tienen que eliminar todos los POS_IDs realacionados con la BD anterior(posNameStoredInDb.DBList_DB_ID) y adicionar los nuevos(masterPos.DB_ID)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "DBList_DB_ID", OldValue = posNameStoredInDb.DBList_DB_ID.ToString(), NewValue = masterPos.DB_ID.ToString()});
                            posNameStoredInDb.DBList_DB_ID = masterPos.DB_ID;

                            var zoomPToDel = db.ZoomDB_POSID_grp.Where(x => x.MasterPOSID == posNameStoredInDb.MasterPOSID).ToList();
                            var zoomLog = zoomPToDel.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "ZoomDB_POSID_grp",
                                AuditAction = "D",
                                ModelPKey = x.ZoomDBPOSID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "ZoomPos_ID", OldValue = x.ZoomPos_ID.ToString()},
                                    new TableInfo{Field_ColumName = "Extra", OldValue = x.Extra},
                                    new TableInfo{Field_ColumName = "MasterPOSID", OldValue = x.MasterPOSID.ToString()},
                                    new TableInfo{Field_ColumName = "Active", OldValue = x.Active.ToString()},
                                }
                            });
                            audiLogs.AddRange(zoomLog);
                            db.ZoomDB_POSID_grp.RemoveRange(zoomPToDel);
                           

                            newPosIdsToInsert = masterPos.PosIDs.Select(x => new ZoomDB_POSID_grp
                            {
                                Active = true,
                                MasterPOSID = posNameStoredInDb.MasterPOSID,
                                ZoomPos_ID = x.ZoomPos_ID,
                                Extra = null
                            }).ToList();
                            db.ZoomDB_POSID_grp.AddRange(newPosIdsToInsert);
                        }
                        else
                        {
                            var currentPosIds = posNameStoredInDb.ZoomDB_POSID_grp.Select(x => x.ZoomPos_ID).ToList();
                            var posIdsByParam = masterPos.PosIDs.Select(x => x.ZoomPos_ID).ToList();

                            var newPosIds = posIdsByParam.Except(currentPosIds);
                            newPosIdsToInsert = newPosIds.Select(x => new ZoomDB_POSID_grp { MasterPOSID = masterPos.MasterPOSID, ZoomPos_ID = x, Active = true, Extra = null }).ToList();
                            db.ZoomDB_POSID_grp.AddRange(newPosIdsToInsert);
                           
                            var oldPosIdsToDelete = currentPosIds.Except(posIdsByParam).ToList();
                            foreach (var pi in oldPosIdsToDelete)
                            {
                                var toDelete = await db.ZoomDB_POSID_grp.FirstOrDefaultAsync(x => x.ZoomPos_ID == pi && x.MasterPOSID == masterPos.MasterPOSID);
                                if (toDelete != null)
                                {
                                    db.ZoomDB_POSID_grp.Remove(toDelete);

                                    audiLogs.Add(new AuditToStore
                                    {
                                        UserLogons = User.Identity.GetUserName(),
                                        AuditDateTime = DateTime.Now,
                                        AuditAction = "D",
                                        TableName = "ZoomDB_POSID_grp",
                                        ModelPKey = toDelete.ZoomDBPOSID,
                                        tableInfos = new List<TableInfo>
                                        {
                                            new TableInfo{Field_ColumName = "Extra", OldValue = toDelete.Extra},
                                            new TableInfo{Field_ColumName = "MasterPOSID", OldValue = toDelete.MasterPOSID.ToString()},
                                            new TableInfo{Field_ColumName = "Active", OldValue = toDelete.Active.ToString()},
                                            new TableInfo{Field_ColumName = "ZoomPos_ID", OldValue = toDelete.ZoomPos_ID.ToString()},
                                        }
                                    });

                                }
                            }
                        }

                        if (!posNameStoredInDb.PosMasterName.Equals(masterPos.PosMasterName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            signalRUpdates = true;

                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "PosMasterName", OldValue = posNameStoredInDb.PosMasterName, NewValue = masterPos.PosMasterName });
                            posNameStoredInDb.PosMasterName = masterPos.PosMasterName;
                        }
                        if (posNameStoredInDb.FvPList_FvPID != masterPos.FvPList_FvPID)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "FvPList_FvPID", OldValue = posNameStoredInDb.FvPList_FvPID.ToString(), NewValue = masterPos.FvPList_FvPID.ToString() });
                            posNameStoredInDb.FvPList_FvPID = masterPos.FvPList_FvPID;

                            switch (db.FvPLists.Find(masterPos.FvPList_FvPID).FvPName)
                            {
                                case "FAC":
                                    if (posNameStoredInDb.PHYGroups_PHYGrpID != null)
                                    {
                                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "PHYGroups_PHYGrpID", OldValue = posNameStoredInDb.PHYGroups_PHYGrpID.ToString() });
                                        posNameStoredInDb.PHYGroups_PHYGrpID = null;
                                    }
                                    break;
                                default:
                                    if (posNameStoredInDb.FACInfo_FACInfoID != null)
                                    {
                                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "FACInfo_FACInfoID", OldValue = posNameStoredInDb.FACInfo_FACInfoID.ToString() });
                                        posNameStoredInDb.FACInfo_FACInfoID = null;
                                    }
                                    if (posNameStoredInDb.InfoData_InfoDataID != null)
                                    {
                                        tableColumnInfo.Add(new TableInfo { Field_ColumName = "InfoData_InfoDataID", OldValue = posNameStoredInDb.InfoData_InfoDataID.ToString() });
                                        posNameStoredInDb.InfoData_InfoDataID = null;
                                    }
                                    if (await db.MasterPOS_LevOfCare.Where(p => p.MasterPOS_MasterPOSID == posNameStoredInDb.MasterPOSID).AnyAsync())
                                    {
                                        var levToDelete = db.MasterPOS_LevOfCare.Where(p => p.MasterPOS_MasterPOSID == posNameStoredInDb.MasterPOSID).ToList();

                                        audiLogs.AddRange(levToDelete.Select(x => new AuditToStore
                                        {
                                            UserLogons = User.Identity.GetUserName(),
                                            AuditAction = "D",
                                            AuditDateTime = DateTime.Now,
                                            TableName = "MasterPOS_LevOfCare",
                                            ModelPKey = x.MasterPosLocID,
                                            tableInfos = new List<TableInfo>
                                            {
                                                new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", OldValue = x.MasterPOS_MasterPOSID.ToString()},
                                                new TableInfo{Field_ColumName = "Lev_of_Care_LevOfCareID", OldValue = x.Lev_of_Care_LevOfCareID.ToString()}
                                            }
                                        }));

                                        db.MasterPOS_LevOfCare.RemoveRange(levToDelete);
                                    }
                                    break;

                            }
                            //if (db.FvPLists.Find(masterPos.FvPList_FvPID).FvPName == "FAC" && posNameStoredInDb.PHYGroups_PHYGrpID != null)
                            // {
                            //     tableColumnInfo.Add(new TableInfo { Field_ColumName = "PHYGroups_PHYGrpID", OldValue = posNameStoredInDb.PHYGroups_PHYGrpID.ToString()});
                            //     posNameStoredInDb.PHYGroups_PHYGrpID = null;
                            // }
                            //if (db.FvPLists.Find(masterPos.FvPList_FvPID).FvPName != "FAC" && posNameStoredInDb.FACInfo_FACInfoID != null)
                            //{
                            //    tableColumnInfo.Add(new TableInfo { Field_ColumName = "FACInfo_FACInfoID", OldValue = posNameStoredInDb.FACInfo_FACInfoID.ToString() });
                            //    posNameStoredInDb.FACInfo_FACInfoID = null;
                            //}
                        }
                        if (posNameStoredInDb.Manager_Master_ManagerID != masterPos.ManagerID)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "Manager_Master_ManagerID", OldValue = posNameStoredInDb.Manager_Master_ManagerID.ToString(), NewValue = masterPos.ManagerID.ToString() });
                            posNameStoredInDb.Manager_Master_ManagerID = masterPos.ManagerID;
                        }
                        if (posNameStoredInDb.Providers_ProvID != masterPos.ProvID)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "Providers_ProvID", OldValue = posNameStoredInDb.Providers_ProvID.ToString(), NewValue = masterPos.ProvID.ToString() });
                            posNameStoredInDb.Providers_ProvID = masterPos.ProvID;
                        }
                       
                        if (posNameStoredInDb.active != masterPos.active)
                        {
                            signalRUpdates = true;
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "active", OldValue = posNameStoredInDb.active.ToString(), NewValue = masterPos.active.ToString() });
                            posNameStoredInDb.active = masterPos.active;
                        }

                        db.MasterPOS.Attach(posNameStoredInDb);
                        db.Entry(posNameStoredInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        if (tableColumnInfo.Any())
                        {
                            audiLogs.Add(new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "U",
                                TableName = "MasterPOS",
                                ModelPKey = posNameStoredInDb.MasterPOSID,
                                tableInfos = tableColumnInfo
                            });
                        }

                        if (newLevToInsert.Any())
                        {
                            var levLog = newLevToInsert.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "MasterPOS_LevOfCare",
                                AuditAction = "I",
                                ModelPKey = x.MasterPosLocID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "Lev_of_Care_LevOfCareID", NewValue = x.Lev_of_Care_LevOfCareID.ToString()},
                                    new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                                }
                            });

                            audiLogs.AddRange(levLog);
                        }

                        if (newServToInsert.Any())
                        {
                            var servLog = newServToInsert.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "MasterPOS_MPServ",
                                AuditAction = "I",
                                ModelPKey = x.MasterPosMPServID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "MPServices_MPServID", NewValue = x.MPServices_MPServID.ToString()},
                                    new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                                }
                            });

                            audiLogs.AddRange(servLog);
                        }

                        if (newFormsToInsert.Any())
                        {
                            var formLog = newFormsToInsert.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "I",
                                TableName = "Forms_sent",
                                ModelPKey = x.FormSentID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "FormsDict_FormsID", NewValue = x.FormsDict_FormsID.ToString()},
                                    new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                                }
                            });

                            audiLogs.AddRange(formLog);
                        }

                        if (newPosIdsToInsert.Any())
                        {
                            var zoomLog = newPosIdsToInsert.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "I",
                                ModelPKey = x.ZoomDBPOSID,
                                TableName = "ZoomDB_POSID_grp",
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "Extra", NewValue = x.Extra},
                                    new TableInfo{Field_ColumName = "Active", NewValue = x.Active.ToString()},
                                    new TableInfo{Field_ColumName = "MasterPOSID", NewValue = x.MasterPOSID.ToString()},
                                    new TableInfo{Field_ColumName = "ZoomPos_ID", NewValue = x.ZoomPos_ID.ToString()}
                                }
                            });

                            audiLogs.AddRange(zoomLog);
                        }
                        new AuditLogRepository().SaveLogs(audiLogs);

                        dbTransaction.Commit();

                        //if (signalRUpdates && corporation != null)
                        //{
                        //    UpdatePosInCorHub.DoIfUpdatePos(corporation.corpID, posNameStoredInDb.MasterPOSID, posNameStoredInDb.PosMasterName, posNameStoredInDb.active);
                        //}

                    }
                    catch (Exception)
                    {
                         dbTransaction.Rollback();
                         ModelState.AddModelError("", "Something failed. Please try again!");
                         return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_MasterPOS([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,PosMasterName,active,FvPList_FvPID,ManagerID,ProvID,DB_ID,LevelOfCares,Services,PosIDs,Forms_Sents")]VMMasterPOS masterPos)
        {
            if (ModelState.IsValid)
            {
                if (await db.MasterPOS.AnyAsync(x => x.PosMasterName.Equals(masterPos.PosMasterName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ModelState.AddModelError("","Duplicate Data. Please try again!");
                    return Json(new[] {masterPos}.ToDataSourceResult(request, ModelState));
                }
                var toStore = new MasterPOS
                {
                    PosMasterName = masterPos.PosMasterName,
                    FvPList_FvPID = masterPos.FvPList_FvPID, //POS CLASSIFICATION
                    Manager_Master_ManagerID = masterPos.ManagerID, //ACCOUNT MANAGER
                    Providers_ProvID = masterPos.ProvID, //MEDICAL DIRECTOR
                    DBList_DB_ID = masterPos.DB_ID, //DATABASE
                    active = masterPos.active
                };
                var corporation = db.Corp_DBs.FirstOrDefault(d => d.DB_ID == masterPos.DB_ID);
                masterPos.Corporation = corporation != null ? corporation.CorporateMasterList.CorporateName : "";

                List<AuditToStore> auditLogs = new List<AuditToStore>();

                if (masterPos.LevelOfCares.Any() || masterPos.Services.Any() || masterPos.Forms_Sents.Any())
                {
                    using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.MasterPOS.Add(toStore);
                            await db.SaveChangesAsync();
                            masterPos.MasterPOSID = toStore.MasterPOSID;
                           
                            //masterPos.Corporation = db.Corp_DBs.FirstOrDefault(d => d.DB_ID == masterPos.DB_ID) != null ? db.Corp_DBs.FirstOrDefault(d => d.DB_ID == masterPos.DB_ID).CorporateMasterList.CorporateName : "";
                           
                            var zoomPosId = masterPos.PosIDs.Select(x => new ZoomDB_POSID_grp
                            {
                                Active = true,
                                MasterPOSID = toStore.MasterPOSID,
                                ZoomPos_ID = x.ZoomPos_ID,
                                Extra = null
                            }).ToList();
                            db.ZoomDB_POSID_grp.AddRange(zoomPosId);

                            var levOfCare = new List<MasterPOS_LevOfCare>();
                            var mpServices = new List<MasterPOS_MPServ>();
                            var formSent = new List<Forms_sent>();

                            if (masterPos.LevelOfCares.Any())
                            {
                                levOfCare = masterPos.LevelOfCares.Select(lv => new MasterPOS_LevOfCare
                                {
                                    MasterPOS_MasterPOSID = toStore.MasterPOSID,
                                    Lev_of_Care_LevOfCareID = lv.LevOfCareID
                                }).ToList();
                                db.MasterPOS_LevOfCare.AddRange(levOfCare);
                            }
                            
                            if (masterPos.Services.Any())
                            {
                                mpServices = masterPos.Services.Select(s => new MasterPOS_MPServ
                                {
                                    MasterPOS_MasterPOSID = toStore.MasterPOSID,
                                    MPServices_MPServID = s.MPServID
                                }).ToList();

                                db.MasterPOS_MPServ.AddRange(mpServices);
                            }

                            if (masterPos.Forms_Sents.Any())
                            {
                                formSent = masterPos.Forms_Sents.Select(x => new Forms_sent
                                            {
                                                FormsDict_FormsID = x.FormsID,
                                                MasterPOS_MasterPOSID = toStore.MasterPOSID
                                            }).ToList();

                                db.Forms_sent.AddRange(formSent);
                            }

                            await db.SaveChangesAsync();

                            auditLogs.Add(new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "MasterPOS",
                                AuditAction = "I",
                                ModelPKey = toStore.MasterPOSID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "PosMasterName", NewValue = toStore.PosMasterName},
                                    new TableInfo{Field_ColumName = "FvPList_FvPID", NewValue = toStore.FvPList_FvPID.ToString()},
                                    new TableInfo{Field_ColumName = "Manager_Master_ManagerID", NewValue = toStore.Manager_Master_ManagerID.ToString()},
                                    new TableInfo{Field_ColumName = "Providers_ProvID", NewValue = toStore.Providers_ProvID.ToString()},
                                    new TableInfo{Field_ColumName = "DBList_DB_ID", NewValue = toStore.DBList_DB_ID.ToString()},
                                    new TableInfo{Field_ColumName = "active", NewValue = toStore.active.ToString()},
                                }
                            });

                            var zoomLog = zoomPosId.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "ZoomDB_POSID_grp",
                                AuditAction = "I",
                                ModelPKey = x.ZoomDBPOSID,
                                tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "MasterPOSID", NewValue = x.MasterPOSID.ToString()},
                                        new TableInfo{Field_ColumName = "Extra", NewValue = x.Extra},
                                        new TableInfo{Field_ColumName = "Active", NewValue = x.Active.ToString()},
                                        new TableInfo{Field_ColumName = "ZoomPos_ID", NewValue = x.ZoomPos_ID.ToString()},
                                    }
                            });

                            auditLogs.AddRange(zoomLog);

                            if (levOfCare.Any())
                            {
                                var locLogs = levOfCare.Select(x => new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "MasterPOS_LevOfCare",
                                    ModelPKey = x.MasterPosLocID,
                                    AuditAction = "I",
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                                        new TableInfo{Field_ColumName = "Lev_of_Care_LevOfCareID", NewValue = x.Lev_of_Care_LevOfCareID.ToString()},
                                    }
                                });

                                auditLogs.AddRange(locLogs);
                            }

                            if (mpServices.Any())
                            {
                                var mpsLogs = mpServices.Select(x => new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "MasterPOS_MPServ",
                                    AuditAction = "I",
                                    ModelPKey = x.MasterPosMPServID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                                        new TableInfo{Field_ColumName = "MPServices_MPServID", NewValue = x.MPServices_MPServID.ToString()}
                                    }
                                });

                                auditLogs.AddRange(mpsLogs);
                            }
                           
                            if (formSent.Any())
                            {
                                var fsLogs = formSent.Select(x => new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "Forms_sent",
                                    AuditAction = "I",
                                    ModelPKey = x.FormSentID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                                        new TableInfo{Field_ColumName = "FormsDict_FormsID", NewValue = x.FormsDict_FormsID.ToString()},
                                    }
                                });
                                auditLogs.AddRange(fsLogs);
                            }
                            new AuditLogRepository().SaveLogs(auditLogs);

                            dbTransaction.Commit();

                            if (corporation != null)
                            {
                                UpdatePosInCorHub.DoIfCreateNewPos(corporation.corpID, toStore.MasterPOSID, toStore.PosMasterName, toStore.active);
                            }
                        }
                        catch (Exception)
                        {
                            dbTransaction.Rollback();
                            ModelState.AddModelError("", "Something failed. Please try again!");
                            return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                else
                {
                    using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.MasterPOS.Add(toStore);
                            await db.SaveChangesAsync();
                            masterPos.MasterPOSID = toStore.MasterPOSID;

                            var zoomPosId = masterPos.PosIDs.Select(x => new ZoomDB_POSID_grp
                            {
                                Active = true,
                                MasterPOSID = toStore.MasterPOSID,
                                ZoomPos_ID = x.ZoomPos_ID,
                                Extra = null
                            }).ToList();

                            db.ZoomDB_POSID_grp.AddRange(zoomPosId);
                            await db.SaveChangesAsync();

                            auditLogs.Add(new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "MasterPOS",
                                AuditAction = "I",
                                ModelPKey = toStore.MasterPOSID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "PosMasterName", NewValue = toStore.PosMasterName},
                                    new TableInfo{Field_ColumName = "FvPList_FvPID", NewValue = toStore.FvPList_FvPID.ToString()},
                                    new TableInfo{Field_ColumName = "Manager_Master_ManagerID", NewValue = toStore.Manager_Master_ManagerID.ToString()},
                                    new TableInfo{Field_ColumName = "Providers_ProvID", NewValue = toStore.Providers_ProvID.ToString()},
                                    new TableInfo{Field_ColumName = "DBList_DB_ID", NewValue = toStore.DBList_DB_ID.ToString()},
                                    new TableInfo{Field_ColumName = "active", NewValue = toStore.active.ToString()},
                                }
                            });
                            var zoomLog = zoomPosId.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "ZoomDB_POSID_grp",
                                AuditAction = "I",
                                ModelPKey = x.ZoomDBPOSID,
                                tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "MasterPOSID", NewValue = x.MasterPOSID.ToString()},
                                        new TableInfo{Field_ColumName = "Extra", NewValue = x.Extra},
                                        new TableInfo{Field_ColumName = "Active", NewValue = x.Active.ToString()},
                                        new TableInfo{Field_ColumName = "ZoomPos_ID", NewValue = x.ZoomPos_ID.ToString()},
                                    }
                            });
                            auditLogs.AddRange(zoomLog);
                            new AuditLogRepository().SaveLogs(auditLogs);

                            dbTransaction.Commit();

                            if (corporation != null)
                            {
                                UpdatePosInCorHub.DoIfCreateNewPos(corporation.corpID, toStore.MasterPOSID, toStore.PosMasterName, toStore.active);
                            }
                        }
                        catch (Exception)
                        {
                            dbTransaction.Rollback();
                            ModelState.AddModelError("", "Something failed. Please try again!");
                            return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }
            return Json(new[] {masterPos}.ToDataSourceResult(request, ModelState));
        }
      
        public ActionResult Download(string ImageName)
        {
            string ServerLocation = @"\\fl-nas02\MDMFiles\";
            // Retrieve the Windows account token for specific user.
            IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("hsuarez", "Medpro705!", "MEDPROBILL"); //Have permissions
            //IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("MDMTest", "Medpro1", "MEDPROBILL");  //Doesn't have permissions

            WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
            WindowsImpersonationContext wic = null;
            try
            {
                wic = wi.Impersonate();

                var dir = new System.IO.DirectoryInfo(ServerLocation);
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");

                if (fileNames.Any(x => x.Name == ImageName))
                {
                    /*-------------------Audit Log------------------------*/

                    MasterFile currentStoredInDb = db.MasterFiles.FirstOrDefault(x => x.FileName == ImageName);
                    if (currentStoredInDb != null)
                    {
                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            AuditAction = "D",
                            TableName = "POSFile",
                            ModelPKey = currentStoredInDb.FileID,
                            tableInfos = new List<TableInfo>
                                 {
                                     new TableInfo{Field_ColumName = "FileName", OldValue = ImageName, NewValue = ImageName}
                                 }
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);
                    }
                    /*-------------------Audit Log------------------------*/
                    return File(ServerLocation + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, ImageName);
                }
                else
                {
                    TempData["Error"] = "The file that you are trying to download doesn't exist in the directory.";
                    return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                }

            }
            catch (Exception)
            {
                TempData["Access"] = "Something failed. You need to have permissions to read/write in this directory.";
                return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
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
