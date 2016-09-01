using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel.Delete;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class LocationsPOSController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/LocationsPOS
        //public async Task<ActionResult> Index()
        //{
        //    var locationsPOS = db.LocationsPOS.Include(l => l.FACInfoData).Include(l => l.Facitity_DBs).Include(l => l.FvPList).Include(l => l.PHYGroup);
        //    return View(await locationsPOS.ToListAsync());
        //}

        //public ActionResult Index()
        //{
        //    FACInfoData facInfoData = new FACInfoData
        //    {
        //        DocProviderName = "Test Doctor FacInfoData",
        //        LicType = "Test LicType FacInfoData",
        //        StateLic = "Fl",
        //        LicNumCLIA_waiver = "Test LicNumCLIA_waiver FacInfoData",
        //        LicEffectiveDate = DateTime.Now,
        //        LicExpireDate = DateTime.Now,
        //        Taxonomy = "Test Taxonomy FacInfoData",
        //        FAC_NPI_Num = "Test NPI Number FacInfoData",               
        //    };

        //    POSLOCExtraData poslocExtraData = new POSLOCExtraData
        //    {
        //        Phone_Number = "8134080105",
        //        Fax_Number = "7863683851",
        //        Website = "www.google.com",
        //        W_9_on_File = true,
        //        Number_of_beds = "20",
        //        Have_24hrs_nursing = true,
        //        how_many_days_week_open = "78",
        //        Ancillary_outpatient_services = "Test poslocExtraData",
        //        Lab_Name = "Test Lab poslocExtraData",
        //        Out_of_Network_In_Network = "Test Out_of_Network_In_Network poslocExtraData",
        //        Licensure_on_File = true,
        //        Mental_License = true,
        //        BCBS_ID_Number = "123456789",
        //        UPIN_Number = "1235445",
        //        Medicaid_Number = "7896413",
        //        State_of_MD_or_PhyGrp = "Test State of MD",
        //        Regulations_on_File = true,
        //        JACHO_CARF = "Test JACHO_CARF",
        //        Has_facility_billed_ins_before = true,
        //        Manage_care_contracts = "Test Manage_care_contracts",
        //        Copies_of_all_managed_care_contracts_on_file = true,
        //        Forms_created = true,
        //        In_Service_scheduled = true,
        //        In_Service_date_time = DateTime.Now,
        //        Portal_training_setup = true,
        //        email_regarding_conference_set_up = true,
        //        Database_set_up = true,
        //        Availavility_request_sent_out = true,
        //        Availavility_completed = true,
        //        Navinet_request_completed = true,
        //        Fee_schedule_in_binder = true,
        //        FACInfoData = facInfoData,
        //        Forms_sent = new List<Forms_sent>
        //       {
        //           new Forms_sent{FormsDict_FormsID = 1, POSLOCExtraData_FACInfoData_FACInfoDataID = facInfoData.FACInfoDataID},
        //           new Forms_sent{FormsDict_FormsID = 3, POSLOCExtraData_FACInfoData_FACInfoDataID = facInfoData.FACInfoDataID},
        //       }
        //    }; 

        //    LocationsPOS location = new LocationsPOS
        //    {
        //         PosName = "Test Name",
        //         FvPList_FvPID = 2, //FAC this comes from dbo.FvPList
        //         FACInfoData = facInfoData,
        //         TaxID = "Test TaxID",
        //         DBA_Name = "DBA_Name",
        //         Payment_Addr1 = "Address1",
        //         Payment_Addr2 = "Address2",
        //         Payment_City = "Miami",
        //         Payment_state = "Fl",
        //         Payment_Zip = "1234",
        //         Physical_Addr1 = "Physical Address 1",
        //         Physical_Addr2 = "Physical Address 2",
        //         Physical_City = "Miami",
        //         Physical_state = "Florida",
        //         Physical_Zip = "78889",
        //         POSFAC_Manager = "Test Manager",
        //         Notes = "Test Notes",
        //         Facitity_DBs_IDPK = 701, //This comes from dbo.Facility_DBs
        //         Time_Zone = "EST",
        //         LocPOS_MPServ = new List<LocPOS_MPServ>
        //         {
        //             new LocPOS_MPServ{LocationsPOS_Facitity_DBs_IDPK = 701, MPServices_MPServID = 1},
        //             new LocPOS_MPServ{LocationsPOS_Facitity_DBs_IDPK = 701, MPServices_MPServID = 3}
        //         },
        //         LocPOS_LevOfCare = new List<LocPOS_LevOfCare>
        //         {
        //             new LocPOS_LevOfCare{LocationsPOS_Facitity_DBs_IDPK = 701, Lev_of_Care_LevOfCareID = 1},
        //             new LocPOS_LevOfCare{LocationsPOS_Facitity_DBs_IDPK = 701, Lev_of_Care_LevOfCareID = 6}
        //         }
        //    };

        //    db.LocationsPOS.Add(location);
        //    db.SaveChanges();

           

        //    return View();
        //}

        public ActionResult Index()
        {
            ViewData["FvPLists"] = db.FvPLists.OrderBy(x => x.FvPName).Select(x => new VMFvpList {id = x.FvPID, FvpName = x.FvPName});

            ViewData["TimeZone"] = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "CST"},
                new SelectListItem{Text = "EST", Value = "EST"}, 
                new SelectListItem{Text = "MST",Value = "MST"},
                new SelectListItem{Value = "PST", Text = "PST"},
            };

            ViewData["LevOfCare"] = db.Lev_of_Care.OrderBy(x => x.LevOfCareName).Select(x => new VMLevelOfCare {LevOfCareID = x.LevOfCareID, LevOfCareName = x.LevOfCareName});

            ViewData["Service"] = db.MPServices.OrderBy(x => x.ServName).Select(x => new VMMPService {MPServID = x.MPServID, ServName = x.ServName});
            
            return View();
        }

        public ActionResult Read_LocationPOS([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.LocationsPOS.ToDataSourceResult(request, x => new VMLocationsPOS
            {
                Facitity_DBs_IDPK = x.Facitity_DBs_IDPK, //PK
                PosName = x.PosName,
                FvPList_FvPID = x.FvPList_FvPID, //FK from dbo.FvPLists
                TaxID = x.TaxID,
                DBA_Name = x.DBA_Name,
                POSFAC_Manager = x.POSFAC_Manager
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_LocationPOS([DataSourceRequest] DataSourceRequest request, [Bind(Include = "Facitity_DBs_IDPK, FvPList_FvPID, PosName, TaxID, DBA_Name, POSFAC_Manager")] VMLocationsPOS locToStore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.LocationsPOS.FindAsync(locToStore.Facitity_DBs_IDPK);

                    List<TableInfo> tableColumInfos = new List<TableInfo>();

                    if (! storedInDb.PosName.Equals(locToStore.PosName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "PosName", OldValue = storedInDb.PosName, NewValue = locToStore.PosName});
                        storedInDb.PosName = locToStore.PosName;
                    }
                    if (storedInDb.FvPList_FvPID != locToStore.FvPList_FvPID)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "FvPList_FvPID", OldValue = storedInDb.FvPList_FvPID.ToString(), NewValue = locToStore.FvPList_FvPID.ToString()});
                        storedInDb.FvPList_FvPID = locToStore.FvPList_FvPID;
                       
                        if (db.FvPLists.Find(locToStore.FvPList_FvPID).FvPName == "FAC")
                        {
                            storedInDb.PHYGroups_PHYGrpID = null;
                        }
                    }
                    if (storedInDb.TaxID == null)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "TaxID", NewValue = locToStore.TaxID});
                        storedInDb.TaxID = locToStore.TaxID;
                    }
                    else
                    {
                        if (storedInDb.TaxID != locToStore.TaxID)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "TaxID", OldValue = storedInDb.TaxID,NewValue = locToStore.TaxID });
                            storedInDb.TaxID = locToStore.TaxID;
                        }
                    }
                    if (storedInDb.DBA_Name == null)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "DBA_Name", NewValue = locToStore.DBA_Name });
                        storedInDb.DBA_Name = locToStore.DBA_Name;
                    }
                    else
                    {
                        if (storedInDb.DBA_Name != locToStore.DBA_Name)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "DBA_Name", OldValue = storedInDb.DBA_Name, NewValue = locToStore.DBA_Name });
                            storedInDb.DBA_Name = locToStore.DBA_Name;
                        }
                    }
                    if (storedInDb.POSFAC_Manager == null)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "POSFAC_Manager", NewValue = locToStore.POSFAC_Manager });
                        storedInDb.POSFAC_Manager = locToStore.POSFAC_Manager;
                    }
                    else
                    {
                        if (storedInDb.POSFAC_Manager != locToStore.POSFAC_Manager)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "POSFAC_Manager", OldValue = storedInDb.POSFAC_Manager, NewValue = locToStore.POSFAC_Manager });
                            storedInDb.POSFAC_Manager = locToStore.POSFAC_Manager;
                        }
                    }

                    db.LocationsPOS.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "U",
                        TableName = "LocationsPOS",
                        ModelPKey = locToStore.Facitity_DBs_IDPK,
                        tableInfos = tableColumInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] {locToStore}.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { locToStore }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_LevelsOfCareByLocPOS(int locPOSID, [DataSourceRequest] DataSourceRequest request)
        {
            var result = db.LocPOS_LevOfCare.Where(pos => pos.LocationsPOS_Facitity_DBs_IDPK == locPOSID);
           
            return Json(result.ToDataSourceResult(request, x => new VMLocPOS_LevOfCare
            {
                LocPosLocID = x.LocPosLocID, //PK
                LocationsPOS_Facitity_DBs_IDPK = x.LocationsPOS_Facitity_DBs_IDPK, //LocationsPOS
                Lev_of_Care_LevOfCareID = x.Lev_of_Care_LevOfCareID //LevelOfCare
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_ServicesByLocPOS(int locPOSID, [DataSourceRequest] DataSourceRequest request)
        {
            var result = db.LocPOS_MPServ.Where(x => x.LocationsPOS_Facitity_DBs_IDPK == locPOSID);
            return Json(result.ToDataSourceResult(request, x => new VMLocPOS_MPServ
            {
                LocPosMPServID = x.LocPosMPServID, //PK
                MPServices_MPServID = x.MPServices_MPServID, //Servicios
                LocationsPOS_Facitity_DBs_IDPK = x.LocationsPOS_Facitity_DBs_IDPK //LocationsPOS
            }), JsonRequestBehavior.AllowGet);
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
