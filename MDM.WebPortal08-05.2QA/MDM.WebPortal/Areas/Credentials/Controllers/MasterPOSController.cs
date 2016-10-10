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
    //[SetPermissions]
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
                        var posNameStoredInDb = await db.MasterPOS.FindAsync(masterPos.MasterPOSID);

                        var currentLevOfCare = posNameStoredInDb.MasterPOS_LevOfCare.Select(x => x.Lev_of_Care_LevOfCareID).ToList();
                        var levOfCareByParam = masterPos.LevelOfCares.Select(x => x.LevOfCareID).ToList();
                        var newLevToInsert = levOfCareByParam.Except(currentLevOfCare).ToList();
                        db.MasterPOS_LevOfCare.AddRange(newLevToInsert.Select(x => new MasterPOS_LevOfCare{Lev_of_Care_LevOfCareID = x, MasterPOS_MasterPOSID = masterPos.MasterPOSID}));
                        var oldLevToDelete = currentLevOfCare.Except(levOfCareByParam).ToList();
                        foreach (var lc in oldLevToDelete)
                        {
                            var toDelete = await db.MasterPOS_LevOfCare.FirstOrDefaultAsync(x => x.Lev_of_Care_LevOfCareID == lc && x.MasterPOS_MasterPOSID == masterPos.MasterPOSID);
                            if (toDelete != null)
                            {
                                db.MasterPOS_LevOfCare.Remove(toDelete);
                            }
                        }

                        var currentServices = posNameStoredInDb.MasterPOS_MPServ.Select(x => x.MPServices_MPServID).ToList();
                        var servicesByParam = masterPos.Services.Select(x => x.MPServID).ToList();
                        var newServToInsert = servicesByParam.Except(currentServices);
                        db.MasterPOS_MPServ.AddRange(newServToInsert.Select(x => new MasterPOS_MPServ{MPServices_MPServID = x, MasterPOS_MasterPOSID = masterPos.MasterPOSID}));
                        var oldServToDelete = currentServices.Except(servicesByParam).ToList();
                        foreach (var sr in oldServToDelete)
                        {
                            var toDelete = await db.MasterPOS_MPServ.FirstOrDefaultAsync(x => x.MPServices_MPServID == sr && x.MasterPOS_MasterPOSID == masterPos.MasterPOSID);
                            if (toDelete != null)
                            {
                                db.MasterPOS_MPServ.Remove(toDelete);
                            }
                        }

                        var currentForms = posNameStoredInDb.Forms_sent.Select(x => x.FormsDict_FormsID).ToList();
                        var formsByParam = masterPos.Forms_Sents.Select(x => x.FormsID).ToList();
                        var newFormsToInsert = formsByParam.Except(currentForms);
                        db.Forms_sent.AddRange(newFormsToInsert.Select(x => new Forms_sent { FormsDict_FormsID = x, MasterPOS_MasterPOSID = masterPos.MasterPOSID }));
                        var oldFormsToDelete = currentForms.Except(formsByParam).ToList();
                        foreach (var f in oldFormsToDelete)
                        {
                            var toDelete = await db.Forms_sent.FirstOrDefaultAsync(x => x.FormsDict_FormsID == f && x.MasterPOS_MasterPOSID == masterPos.MasterPOSID);
                            if (toDelete != null)
                            {
                                db.Forms_sent.Remove(toDelete);
                            }
                        }

                        if (posNameStoredInDb.DBList_DB_ID != masterPos.DB_ID)//Si la BD cambia se tienen que eliminar todos los POS_IDs realacionados con la BD anterior(posNameStoredInDb.DBList_DB_ID) y adicionar los nuevos(masterPos.DB_ID)
                        {
                            posNameStoredInDb.DBList_DB_ID = masterPos.DB_ID;
                            db.ZoomDB_POSID_grp.RemoveRange(db.ZoomDB_POSID_grp.Where(x => x.MasterPOSID == posNameStoredInDb.MasterPOSID));

                            db.ZoomDB_POSID_grp.AddRange(masterPos.PosIDs.Select(x => new ZoomDB_POSID_grp
                            {
                                Active = true,
                                MasterPOSID = posNameStoredInDb.MasterPOSID,
                                ZoomPos_ID = x.ZoomPos_ID,
                                Extra = null
                            }));
                        }
                        else
                        {
                            var currentPosIds = posNameStoredInDb.ZoomDB_POSID_grp.Select(x => x.ZoomPos_ID).ToList();
                            var posIdsByParam = masterPos.PosIDs.Select(x => x.ZoomPos_ID).ToList();
                            var newPosIdsToInsert = posIdsByParam.Except(currentPosIds);
                            db.ZoomDB_POSID_grp.AddRange(newPosIdsToInsert.Select(x => new ZoomDB_POSID_grp { MasterPOSID = masterPos.MasterPOSID, ZoomPos_ID = x, Active = true, Extra = null }));
                            var oldPosIdsToDelete = currentPosIds.Except(posIdsByParam).ToList();
                            foreach (var pi in oldPosIdsToDelete)
                            {
                                var toDelete = await db.ZoomDB_POSID_grp.FirstOrDefaultAsync(x => x.ZoomPos_ID == pi && x.MasterPOSID == masterPos.MasterPOSID);
                                if (toDelete != null)
                                {
                                    db.ZoomDB_POSID_grp.Remove(toDelete);
                                }
                            }
                        }

                        if (posNameStoredInDb.PosMasterName.Equals(masterPos.PosMasterName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            posNameStoredInDb.PosMasterName = masterPos.PosMasterName;
                        }
                        if (posNameStoredInDb.FvPList_FvPID != masterPos.FvPList_FvPID)
                        {
                            posNameStoredInDb.FvPList_FvPID = masterPos.FvPList_FvPID;
                            if (db.FvPLists.Find(masterPos.FvPList_FvPID).FvPName == "FAC" && posNameStoredInDb.PHYGroups_PHYGrpID != null)
                             {
                                 posNameStoredInDb.PHYGroups_PHYGrpID = null;
                             }
                            if (db.FvPLists.Find(masterPos.FvPList_FvPID).FvPName == "PHY" && posNameStoredInDb.FACInfo_FACInfoID != null)
                            {
                                posNameStoredInDb.FACInfo_FACInfoID = null;
                            }
                        }
                        if (posNameStoredInDb.Manager_Master_ManagerID != masterPos.ManagerID)
                        {
                            posNameStoredInDb.Manager_Master_ManagerID = masterPos.ManagerID;
                        }
                        if (posNameStoredInDb.Providers_ProvID != masterPos.ProvID)
                        {
                            posNameStoredInDb.Providers_ProvID = masterPos.ProvID;
                        }
                       
                        if (posNameStoredInDb.active != masterPos.active)
                        {
                            posNameStoredInDb.active = masterPos.active;
                        }

                        db.MasterPOS.Attach(posNameStoredInDb);
                        db.Entry(posNameStoredInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        dbTransaction.Commit();
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
                if (masterPos.LevelOfCares.Any() || masterPos.Services.Any() || masterPos.PosIDs.Any() || masterPos.Forms_Sents.Any())
                {
                    using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.MasterPOS.Add(toStore);
                            await db.SaveChangesAsync();
                            masterPos.MasterPOSID = toStore.MasterPOSID;
                            masterPos.Corporation = db.Corp_DBs.FirstOrDefault(d => d.DB_ID == masterPos.DB_ID) != null ? db.Corp_DBs.FirstOrDefault(d => d.DB_ID == masterPos.DB_ID).CorporateMasterList.CorporateName : "";
                            if (masterPos.LevelOfCares.Any())
                            {
                                db.MasterPOS_LevOfCare.AddRange( masterPos.LevelOfCares.Select(lv => new MasterPOS_LevOfCare
                                    {
                                        MasterPOS_MasterPOSID = toStore.MasterPOSID,
                                        Lev_of_Care_LevOfCareID = lv.LevOfCareID
                                    }));
                            }
                            if (masterPos.Services.Any())
                            {
                                db.MasterPOS_MPServ.AddRange(masterPos.Services.Select(s => new MasterPOS_MPServ
                                {
                                    MasterPOS_MasterPOSID = toStore.MasterPOSID,
                                    MPServices_MPServID = s.MPServID
                                }));
                            }
                            if (masterPos.PosIDs.Any())
                            {
                                //List<VMZoomDB_POSID> availables = new List<VMZoomDB_POSID>();
                                //List<VMZoomDB_POSID> posTaken = new List<VMZoomDB_POSID>();

                                //var DB = db.DBLists.Find(toStore.DBList_DB_ID).DB;
                                //var allPosIdOfThisDb = db.Facitity_DBs.Where(x => x.DB == DB)
                                //                         .Select(z => new VMZoomDB_POSID
                                //                         {
                                //                             ZoomPos_ID = z.Facility_ID, 
                                //                             ZoomPos_Name = z.Fac_NAME
                                //                         }).ToList();
                                
                                //var aux = db.MasterPOS.Include(p => p.ZoomDB_POSID_grp)
                                //                      .Where(x => x.DBList_DB_ID == toStore.DBList_DB_ID)
                                //                      .Select(z => z.ZoomDB_POSID_grp).ToList();
                              
                                //foreach (var item in aux)
                                //{
                                //    var converted = item.Select(c => new VMZoomDB_POSID
                                //    {
                                //        ZoomPos_ID = c.ZoomPos_ID, 
                                //        ZoomPos_Name = db.Facitity_DBs.First(g => g.DB == DB && g.Facility_ID == c.ZoomPos_ID).Fac_NAME
                                //    }).ToList();

                                //    posTaken.AddRange(converted);
                                //}

                                //availables = allPosIdOfThisDb.Except(posTaken).ToList();

                                //var toInsert = availables.Intersect(masterPos.PosIDs).ToList();

                                //var unavailables = masterPos.PosIDs.Except(availables).ToList();

                                db.ZoomDB_POSID_grp.AddRange(masterPos.PosIDs.Select(x => new ZoomDB_POSID_grp
                                {
                                    Active = true,
                                    MasterPOSID = toStore.MasterPOSID,
                                    ZoomPos_ID = x.ZoomPos_ID,
                                    Extra = null
                                }));

                                //if (unavailables.Any())
                                //{
                                //    string error = string.Join(",", unavailables.Select(f => f.ZoomPos_ID).ToArray()); //C# convert array of integers to comma-separated string
                                //    PosNameMasterListHub.DoIfDuplicatePosIds(error);
                                //}
                            }

                            if (masterPos.Forms_Sents.Any())
                            {
                                db.Forms_sent.AddRange(masterPos.Forms_Sents.Select(x => new Forms_sent{FormsDict_FormsID = x.FormsID, MasterPOS_MasterPOSID = toStore.MasterPOSID}));
                            }

                            await db.SaveChangesAsync();
                            dbTransaction.Commit();
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
                    try
                    {
                        db.MasterPOS.Add(toStore);
                        await db.SaveChangesAsync();
                        masterPos.MasterPOSID = toStore.MasterPOSID;
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { masterPos }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] {masterPos}.ToDataSourceResult(request, ModelState));
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
                                StringComparison.InvariantCultureIgnoreCase)
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
