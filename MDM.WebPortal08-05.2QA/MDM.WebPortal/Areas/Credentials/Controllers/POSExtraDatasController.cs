using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class POSExtraDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> ExtraData_Detail(int? MasterPOSID)
        {
            if (MasterPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            var extraData = pos.POSExtraData;
            var toView = new List<VMPOSLOCExtraData>();
            if (extraData != null)
            {
                toView.Add(new VMPOSLOCExtraData
                {
                    MasterPOSID = MasterPOSID.Value,
                    Availavility_completed = extraData.Availavility_completed,
                    Portal_training_setup = extraData.Portal_training_setup,
                    In_Service_scheduled = extraData.In_Service_scheduled,
                    Mental_License = extraData.Mental_License,
                    Out_of_Network_In_Network = extraData.Out_of_Network_In_Network,
                    Have_24hrs_nursing = extraData.Have_24hrs_nursing,
                    Licensure_on_File = extraData.Licensure_on_File,
                    Regulations_on_File = extraData.Regulations_on_File,
                    Has_facility_billed_ins_before = extraData.Has_facility_billed_ins_before,
                    Fee_schedule_in_binder = extraData.Fee_schedule_in_binder,
                    Navinet_request_completed = extraData.Navinet_request_completed,
                    Copies_of_all_managed_care_contracts_on_file = extraData.Copies_of_all_managed_care_contracts_on_file,
                    Database_set_up = extraData.Database_set_up,
                    email_regarding_conference_set_up = extraData.email_regarding_conference_set_up,
                    Forms_created = extraData.Forms_created,
                    W_9_on_File = extraData.W_9_on_File,
                    Availavility_request_sent_out = extraData.Availavility_completed,
                    JACHO_CARF = extraData.JACHO_CARF,
                    State_of_MD_or_PhyGrp = extraData.State_of_MD_or_PhyGrp,
                    Lab_Name = extraData.Lab_Name,
                    Manage_care_contracts = extraData.Manage_care_contracts,
                    In_Service_date_time = extraData.In_Service_date_time,
                    how_many_days_week_open = extraData.how_many_days_week_open,
                    UPIN_Number = extraData.UPIN_Number,
                    Fax_Number = extraData.Fax_Number,
                    BCBS_ID_Number = extraData.BCBS_ID_Number,
                    Number_of_beds = extraData.Number_of_beds,
                    Website = extraData.Website,
                    Medicaid_Number = extraData.Medicaid_Number,
                    Phone_Number = extraData.Phone_Number,
                    Ancillary_outpatient_services = extraData.Ancillary_outpatient_services,
                    AcreditationOnFile = extraData.AcreditationOnFile,
                    AdmissionPhone = extraData.AdmissionPhone,
                    AverageLenOfStay = extraData.AverageLenOfStay,
                    HighComplexityCLIA = extraData.HighComplexityCLIA,
                    HowManyUAPanels = extraData.HowManyUAPanels,
                    MedicareNumber = extraData.MedicareNumber,
                    POSExtraDataID = extraData.POSExtraDataID,
                    PaidToPatientState = extraData.PaidToPatientState,
                    RegistrationAnalyzer = extraData.RegistrationAnalyzer,
                    ScholarshipRate = extraData.ScholarshipRate
                });
            }

            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }

        public async Task<ActionResult> ExtraInfo_Detail(int? MasterPOSID)
        {
            var toView = new List<VMPOSLOCExtraData>();
            if (MasterPOSID == null)
            {
                ViewBag.MasterPOS = 0;
                return View();
            }
            var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                ViewBag.MasterPOS = 0;
                return View();
            }
            var extraData = pos.POSExtraData;
            if (extraData != null)
            {
                toView.Add(new VMPOSLOCExtraData
                {
                    MasterPOSID = MasterPOSID.Value,
                    POSExtraDataID = extraData.POSExtraDataID,
                    Phone_Number = extraData.Phone_Number ?? "",
                    AdmissionPhone = extraData.AdmissionPhone ?? "",
                    Fax_Number = extraData.Fax_Number ?? "",
                    Website = extraData.Website ?? "",
                    ScholarshipRate = extraData.ScholarshipRate ?? "",
                    AverageLenOfStay = extraData.AverageLenOfStay ?? "",
                    HowManyUAPanels = extraData.HowManyUAPanels ?? "",
                    PaidToPatientState = extraData.PaidToPatientState ?? "",
                    MedicareNumber = extraData.MedicareNumber ?? "",
                    Number_of_beds = extraData.Number_of_beds ?? "",
                    how_many_days_week_open = extraData.how_many_days_week_open ?? "",
                    Ancillary_outpatient_services = extraData.Ancillary_outpatient_services ?? "",
                    Out_of_Network_In_Network = extraData.Out_of_Network_In_Network ?? "",
                    Lab_Name = extraData.Lab_Name ?? "",
                    BCBS_ID_Number = extraData.BCBS_ID_Number ?? "",
                    UPIN_Number = extraData.UPIN_Number ?? "",
                    Medicaid_Number = extraData.Medicaid_Number ?? "",
                    State_of_MD_or_PhyGrp = extraData.State_of_MD_or_PhyGrp ?? "",
                    JACHO_CARF = extraData.JACHO_CARF ?? "",
                    In_Service_date_time = extraData.In_Service_date_time,
                    Manage_care_contracts = extraData.Manage_care_contracts ?? "",
                    //booleans
                  
                    W_9_on_File = extraData.W_9_on_File ?? false,
                    Have_24hrs_nursing = extraData.Have_24hrs_nursing ?? false,
                    Licensure_on_File = extraData.Licensure_on_File ?? false,
                    Mental_License = extraData.Mental_License ?? false,
                    Regulations_on_File = extraData.Regulations_on_File ?? false,
                    Has_facility_billed_ins_before = extraData.Has_facility_billed_ins_before ?? false,
                    Copies_of_all_managed_care_contracts_on_file = extraData.Copies_of_all_managed_care_contracts_on_file ?? false,
                    Forms_created = extraData.Forms_created ?? false,
                    In_Service_scheduled = extraData.In_Service_scheduled ?? false,
                    Portal_training_setup = extraData.Portal_training_setup ?? false,
                    email_regarding_conference_set_up = extraData.email_regarding_conference_set_up ?? false,
                    Database_set_up = extraData.Database_set_up ?? false,
                    Availavility_request_sent_out = extraData.Availavility_request_sent_out ?? false,
                    Availavility_completed = extraData.Availavility_completed ?? false,
                    Navinet_request_completed = extraData.Navinet_request_completed ?? false,
                    Fee_schedule_in_binder = extraData.Fee_schedule_in_binder ?? false,
                    AcreditationOnFile = extraData.AcreditationOnFile ?? false,
                    HighComplexityCLIA = extraData.HighComplexityCLIA ?? false,
                    RegistrationAnalyzer = extraData.RegistrationAnalyzer ?? false
                });
            }
            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }



        public async Task<ActionResult> ExtraQuestion_Detail(int? MasterPOSID)
        {
            var toView = new List<VMPOSLOCExtraData>();
            if (MasterPOSID == null)
            {
                ViewBag.MasterPOS = 0;
                return View();
            }
            var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                ViewBag.MasterPOS = 0;
                return View();
            }
            var extraData = pos.POSExtraData;
            if (extraData != null)
            {
                toView.Add(new VMPOSLOCExtraData
                {
                    MasterPOSID = MasterPOSID.Value,
                    POSExtraDataID = extraData.POSExtraDataID,
                    W_9_on_File = extraData.W_9_on_File ?? false,
                    Have_24hrs_nursing = extraData.Have_24hrs_nursing ?? false,
                    Licensure_on_File = extraData.Licensure_on_File ?? false,
                    Mental_License = extraData.Mental_License ?? false,
                    Regulations_on_File = extraData.Regulations_on_File ?? false,
                    Has_facility_billed_ins_before = extraData.Has_facility_billed_ins_before ?? false,
                    Copies_of_all_managed_care_contracts_on_file = extraData.Copies_of_all_managed_care_contracts_on_file ?? false,
                    Forms_created = extraData.Forms_created ?? false,
                    In_Service_scheduled = extraData.In_Service_scheduled ?? false,
                    Portal_training_setup = extraData.Portal_training_setup ?? false,
                    email_regarding_conference_set_up = extraData.email_regarding_conference_set_up ?? false,
                    Database_set_up = extraData.Database_set_up ?? false,
                    Availavility_request_sent_out = extraData.Availavility_request_sent_out ?? false,
                    Availavility_completed = extraData.Availavility_completed ?? false,
                    Navinet_request_completed = extraData.Navinet_request_completed ?? false,
                    Fee_schedule_in_binder = extraData.Fee_schedule_in_binder ?? false,
                    AcreditationOnFile = extraData.AcreditationOnFile ?? false,
                    HighComplexityCLIA = extraData.HighComplexityCLIA ?? false,
                    RegistrationAnalyzer = extraData.RegistrationAnalyzer ?? false,
                    //freetext
                    Phone_Number = extraData.Phone_Number ?? "",
                    AdmissionPhone = extraData.AdmissionPhone ?? "",
                    Fax_Number = extraData.Fax_Number ?? "",
                    Website = extraData.Website ?? "",
                    ScholarshipRate = extraData.ScholarshipRate ?? "",
                    AverageLenOfStay = extraData.AverageLenOfStay ?? "",
                    HowManyUAPanels = extraData.HowManyUAPanels ?? "",
                    PaidToPatientState = extraData.PaidToPatientState ?? "",
                    MedicareNumber = extraData.MedicareNumber ?? "",
                    Number_of_beds = extraData.Number_of_beds ?? "",
                    how_many_days_week_open = extraData.how_many_days_week_open ?? "",
                    Ancillary_outpatient_services = extraData.Ancillary_outpatient_services ?? "",
                    Out_of_Network_In_Network = extraData.Out_of_Network_In_Network ?? "",
                    Lab_Name = extraData.Lab_Name ?? "",
                    BCBS_ID_Number = extraData.BCBS_ID_Number ?? "",
                    UPIN_Number = extraData.UPIN_Number ?? "",
                    Medicaid_Number = extraData.Medicaid_Number ?? "",
                    State_of_MD_or_PhyGrp = extraData.State_of_MD_or_PhyGrp ?? "",
                    JACHO_CARF = extraData.JACHO_CARF ?? "",
                    In_Service_date_time = extraData.In_Service_date_time,
                    Manage_care_contracts = extraData.Manage_care_contracts ?? "",
                });
            }
            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }

        public async Task<ActionResult> Read_ExtraData([DataSourceRequest] DataSourceRequest request, int? MasterPOSID)
        {
            var result = new List<VMPOSLOCExtraData>();
            if (MasterPOSID != null)
            {
                var masterPos = await db.MasterPOS.FindAsync(MasterPOSID);
                if (masterPos != null && masterPos.POSExtraData != null)
                {
                    var extraData = masterPos.POSExtraData;
                    result.Add(new VMPOSLOCExtraData
                    {
                        MasterPOSID = MasterPOSID.Value,
                        POSExtraDataID = extraData.POSExtraDataID,
                        W_9_on_File = extraData.W_9_on_File ?? false,
                        Have_24hrs_nursing = extraData.Have_24hrs_nursing ?? false,
                        Licensure_on_File = extraData.Licensure_on_File ?? false,
                        Mental_License = extraData.Mental_License ?? false,
                        Regulations_on_File = extraData.Regulations_on_File ?? false,
                        Has_facility_billed_ins_before = extraData.Has_facility_billed_ins_before ?? false,
                        Copies_of_all_managed_care_contracts_on_file = extraData.Copies_of_all_managed_care_contracts_on_file ?? false,
                        Forms_created = extraData.Forms_created ?? false,
                        In_Service_scheduled = extraData.In_Service_scheduled ?? false,
                        Portal_training_setup = extraData.Portal_training_setup ?? false,
                        email_regarding_conference_set_up = extraData.email_regarding_conference_set_up ?? false,
                        Database_set_up = extraData.Database_set_up ?? false,
                        Availavility_request_sent_out = extraData.Availavility_request_sent_out ?? false,
                        Availavility_completed = extraData.Availavility_completed ?? false,
                        Navinet_request_completed = extraData.Navinet_request_completed ?? false,
                        Fee_schedule_in_binder = extraData.Fee_schedule_in_binder ?? false,
                        AcreditationOnFile = extraData.AcreditationOnFile ?? false,
                        HighComplexityCLIA = extraData.HighComplexityCLIA ?? false,
                        RegistrationAnalyzer = extraData.RegistrationAnalyzer ?? false,
                        //freetext
                        Phone_Number = extraData.Phone_Number ?? "",
                        AdmissionPhone = extraData.AdmissionPhone ?? "",
                        Fax_Number = extraData.Fax_Number ?? "",
                        Website = extraData.Website ?? "",
                        ScholarshipRate = extraData.ScholarshipRate ?? "",
                        AverageLenOfStay = extraData.AverageLenOfStay ?? "",
                        HowManyUAPanels = extraData.HowManyUAPanels ?? "",
                        PaidToPatientState = extraData.PaidToPatientState ?? "",
                        MedicareNumber = extraData.MedicareNumber ?? "",
                        Number_of_beds = extraData.Number_of_beds ?? "",
                        how_many_days_week_open = extraData.how_many_days_week_open ?? "",
                        Ancillary_outpatient_services = extraData.Ancillary_outpatient_services ?? "",
                        Out_of_Network_In_Network = extraData.Out_of_Network_In_Network ?? "",
                        Lab_Name = extraData.Lab_Name ?? "",
                        BCBS_ID_Number = extraData.BCBS_ID_Number ?? "",
                        UPIN_Number = extraData.UPIN_Number ?? "",
                        Medicaid_Number = extraData.Medicaid_Number ?? "",
                        State_of_MD_or_PhyGrp = extraData.State_of_MD_or_PhyGrp ?? "",
                        JACHO_CARF = extraData.JACHO_CARF ?? "",
                        In_Service_date_time = extraData.In_Service_date_time,
                        Manage_care_contracts = extraData.Manage_care_contracts ?? "",
                    });
                }
            }
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_ExtraData([DataSourceRequest] DataSourceRequest request, 
            [Bind(Include = "MasterPOSID,POSExtraDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Ancillary_outpatient_services,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder,AdmissionPhone,ScholarshipRate,AcreditationOnFile,AverageLenOfStay,HowManyUAPanels,HighComplexityCLIA,RegistrationAnalyzer,PaidToPatientState,MedicareNumber")]
            VMPOSLOCExtraData pOSExtraData, int ParentID)
        {
            if (ModelState.IsValid)
            {
                if (ParentID == 0)
                {
                    ModelState.AddModelError("", "The POS doesn't exist in our system anymore. Please try again!");
                    return Json(new[] { pOSExtraData }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var toStore = new POSExtraData
                        {
                            W_9_on_File = pOSExtraData.W_9_on_File ?? false,
                            Have_24hrs_nursing = pOSExtraData.Have_24hrs_nursing ?? false,
                            Licensure_on_File = pOSExtraData.Licensure_on_File ?? false,
                            Mental_License = pOSExtraData.Mental_License ?? false,
                            Regulations_on_File = pOSExtraData.Regulations_on_File ?? false,
                            Has_facility_billed_ins_before = pOSExtraData.Has_facility_billed_ins_before ?? false,
                            Copies_of_all_managed_care_contracts_on_file = pOSExtraData.Copies_of_all_managed_care_contracts_on_file ?? false,
                            Forms_created = pOSExtraData.Forms_created ?? false,
                            In_Service_scheduled = pOSExtraData.In_Service_scheduled ?? false,
                            Portal_training_setup = pOSExtraData.Portal_training_setup ?? false,
                            email_regarding_conference_set_up = pOSExtraData.email_regarding_conference_set_up ?? false,
                            Database_set_up = pOSExtraData.Database_set_up ?? false,
                            Availavility_request_sent_out = pOSExtraData.Availavility_request_sent_out ?? false,
                            Availavility_completed = pOSExtraData.Availavility_completed ?? false,
                            Navinet_request_completed = pOSExtraData.Navinet_request_completed ?? false,
                            Fee_schedule_in_binder = pOSExtraData.Fee_schedule_in_binder ?? false,
                            AcreditationOnFile = pOSExtraData.AcreditationOnFile ?? false,
                            HighComplexityCLIA = pOSExtraData.HighComplexityCLIA ?? false,
                            RegistrationAnalyzer = pOSExtraData.RegistrationAnalyzer ?? false,

                           
                            JACHO_CARF = pOSExtraData.JACHO_CARF,
                            State_of_MD_or_PhyGrp = pOSExtraData.State_of_MD_or_PhyGrp,
                            Lab_Name = pOSExtraData.Lab_Name,
                            Manage_care_contracts = pOSExtraData.Manage_care_contracts,
                            In_Service_date_time = pOSExtraData.In_Service_date_time,
                            how_many_days_week_open = pOSExtraData.how_many_days_week_open,
                            UPIN_Number = pOSExtraData.UPIN_Number,
                            Fax_Number = pOSExtraData.Fax_Number,
                            BCBS_ID_Number = pOSExtraData.BCBS_ID_Number,
                            Number_of_beds = pOSExtraData.Number_of_beds,
                            Website = pOSExtraData.Website,
                            Medicaid_Number = pOSExtraData.Medicaid_Number,
                            Phone_Number = pOSExtraData.Phone_Number,
                            Ancillary_outpatient_services = pOSExtraData.Ancillary_outpatient_services,
                            AdmissionPhone = pOSExtraData.AdmissionPhone,
                            AverageLenOfStay = pOSExtraData.AverageLenOfStay,
                            HowManyUAPanels = pOSExtraData.HowManyUAPanels,
                            MedicareNumber = pOSExtraData.MedicareNumber,
                            PaidToPatientState = pOSExtraData.PaidToPatientState,
                            ScholarshipRate = pOSExtraData.ScholarshipRate,
                            Out_of_Network_In_Network = pOSExtraData.Out_of_Network_In_Network

                        };

                        db.POSExtraDatas.Add(toStore);
                        await db.SaveChangesAsync();
                        pOSExtraData.MasterPOSID = ParentID;
                        pOSExtraData.POSExtraDataID = toStore.POSExtraDataID;

                        var pos = await db.MasterPOS.FindAsync(ParentID);
                        var tableColumnInfo = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "", OldValue = pos.POSExtraData_POSExtraDataID.ToString(), NewValue = toStore.POSExtraDataID.ToString()}
                        };
                        pos.POSExtraData_POSExtraDataID = toStore.POSExtraDataID;

                        db.MasterPOS.Attach(pos);
                        db.Entry(pos).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        var auditLogs = new List<AuditToStore>
                        {
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "POSExtraDatas",
                                AuditAction = "I",
                                ModelPKey = toStore.POSExtraDataID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "AdmissionPhone", NewValue = toStore.AdmissionPhone},

                                    new TableInfo{Field_ColumName = "Ancillary_outpatient_services", NewValue = toStore.Ancillary_outpatient_services},

                                    new TableInfo{Field_ColumName = "AverageLenOfStay", NewValue = toStore.AverageLenOfStay},

                                    new TableInfo{Field_ColumName = "BCBS_ID_Number", NewValue = toStore.BCBS_ID_Number},

                                    new TableInfo{Field_ColumName = "Fax_Number", NewValue = toStore.Fax_Number},

                                    new TableInfo{Field_ColumName = "HowManyUAPanels", NewValue = toStore.HowManyUAPanels},

                                    new TableInfo{Field_ColumName = "JACHO_CARF", NewValue = toStore.JACHO_CARF},

                                    new TableInfo{Field_ColumName = "Lab_Name", NewValue = toStore.Lab_Name},

                                    new TableInfo{Field_ColumName = "Manage_care_contracts", NewValue = toStore.Manage_care_contracts},

                                    new TableInfo{Field_ColumName = "Medicaid_Number", NewValue = toStore.Medicaid_Number},

                                    new TableInfo{Field_ColumName = "MedicareNumber", NewValue = toStore.MedicareNumber},

                                    new TableInfo{Field_ColumName = "Number_of_beds", NewValue = toStore.Number_of_beds},

                                    new TableInfo{Field_ColumName = "Out_of_Network_In_Network", NewValue = toStore.Out_of_Network_In_Network},

                                    new TableInfo{Field_ColumName = "PaidToPatientState", NewValue = toStore.PaidToPatientState},

                                    new TableInfo{Field_ColumName = "Phone_Number", NewValue = toStore.Phone_Number},

                                    new TableInfo{Field_ColumName = "ScholarshipRate", NewValue = toStore.ScholarshipRate},

                                    new TableInfo{Field_ColumName = "State_of_MD_or_PhyGrp", NewValue = toStore.State_of_MD_or_PhyGrp},

                                    new TableInfo{Field_ColumName = "UPIN_Number", NewValue = toStore.UPIN_Number},

                                    new TableInfo{Field_ColumName = "Website", NewValue = toStore.Website},

                                    new TableInfo{Field_ColumName = "how_many_days_week_open", NewValue = toStore.how_many_days_week_open},

                                    new TableInfo{Field_ColumName = "AcreditationOnFile", NewValue = toStore.AcreditationOnFile.ToString()},

                                    new TableInfo{Field_ColumName = "Availavility_completed", NewValue = toStore.Availavility_completed.ToString()},

                                    new TableInfo{Field_ColumName = "Availavility_request_sent_out", NewValue = toStore.Availavility_request_sent_out.ToString()},

                                    new TableInfo{Field_ColumName = "Copies_of_all_managed_care_contracts_on_file", NewValue = toStore.Copies_of_all_managed_care_contracts_on_file.ToString()},

                                    new TableInfo{Field_ColumName = "Database_set_up", NewValue = toStore.Database_set_up.ToString()},

                                    new TableInfo{Field_ColumName = "Fee_schedule_in_binder", NewValue = toStore.Fee_schedule_in_binder.ToString()},

                                    new TableInfo{Field_ColumName = "Forms_created", NewValue = toStore.Forms_created.ToString()},

                                    new TableInfo{Field_ColumName = "Has_facility_billed_ins_before", NewValue = toStore.Has_facility_billed_ins_before.ToString()},

                                    new TableInfo{Field_ColumName = "Have_24hrs_nursing", NewValue = toStore.Have_24hrs_nursing.ToString()},

                                    new TableInfo{Field_ColumName = "HighComplexityCLIA", NewValue = toStore.HighComplexityCLIA.ToString()},

                                    new TableInfo{Field_ColumName = "RegistrationAnalyzer", NewValue = toStore.RegistrationAnalyzer.ToString()},

                                    new TableInfo{Field_ColumName = "In_Service_date_time", NewValue = toStore.In_Service_date_time.ToString()},

                                    new TableInfo{Field_ColumName = "Navinet_request_completed", NewValue = toStore.Navinet_request_completed.ToString()},

                                    new TableInfo{Field_ColumName = "W_9_on_File", NewValue = toStore.W_9_on_File.ToString()},

                                    new TableInfo{Field_ColumName = "email_regarding_conference_set_up", NewValue = toStore.email_regarding_conference_set_up.ToString()},

                                    new TableInfo{Field_ColumName = "Licensure_on_File", NewValue = toStore.Licensure_on_File.ToString()},

                                    new TableInfo{Field_ColumName = "In_Service_scheduled", NewValue = toStore.In_Service_scheduled.ToString()},

                                    new TableInfo{Field_ColumName = "Mental_License", NewValue = toStore.Mental_License.ToString()},

                                    new TableInfo{Field_ColumName = "Regulations_on_File", NewValue = toStore.Regulations_on_File.ToString()},

                                    new TableInfo{Field_ColumName = "Portal_training_setup", NewValue = toStore.Portal_training_setup.ToString()},
                                }
                            },
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "MasterPOS",
                                AuditAction = "U",
                                ModelPKey = pos.MasterPOSID,
                                tableInfos = tableColumnInfo
                            }
                        };

                        new AuditLogRepository().SaveLogs(auditLogs);

                        dbTransaction.Commit();

                        pOSExtraData.JACHO_CARF = pOSExtraData.JACHO_CARF ?? "";
                        pOSExtraData.State_of_MD_or_PhyGrp = pOSExtraData.State_of_MD_or_PhyGrp ?? "";
                        pOSExtraData.Lab_Name = pOSExtraData.Lab_Name ?? "";
                        pOSExtraData.Manage_care_contracts = pOSExtraData.Manage_care_contracts ?? "";
                        //pOSExtraData.In_Service_date_time = pOSExtraData.In_Service_date_time,
                        pOSExtraData.how_many_days_week_open = pOSExtraData.how_many_days_week_open ?? "";
                        pOSExtraData.UPIN_Number = pOSExtraData.UPIN_Number ?? "";
                        pOSExtraData.Fax_Number = pOSExtraData.Fax_Number ?? "";
                        pOSExtraData.BCBS_ID_Number = pOSExtraData.BCBS_ID_Number ?? "";
                        pOSExtraData.Number_of_beds = pOSExtraData.Number_of_beds ?? "";
                        pOSExtraData.Website = pOSExtraData.Website ?? "";
                        pOSExtraData.Medicaid_Number = pOSExtraData.Medicaid_Number ?? "";
                        pOSExtraData.Phone_Number = pOSExtraData.Phone_Number ?? "";
                        pOSExtraData.Ancillary_outpatient_services = pOSExtraData.Ancillary_outpatient_services ?? "";
                        pOSExtraData.AdmissionPhone = pOSExtraData.AdmissionPhone ?? "";
                        pOSExtraData.AverageLenOfStay = pOSExtraData.AverageLenOfStay ?? "";
                        pOSExtraData.HowManyUAPanels = pOSExtraData.HowManyUAPanels ?? "";
                        pOSExtraData.MedicareNumber = pOSExtraData.MedicareNumber ?? "";
                        pOSExtraData.PaidToPatientState = pOSExtraData.PaidToPatientState ?? "";
                        pOSExtraData.ScholarshipRate = pOSExtraData.ScholarshipRate ?? "";
                        pOSExtraData.Out_of_Network_In_Network = pOSExtraData.Out_of_Network_In_Network ?? "";
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("","Something failed. Please try again!");
                        return Json(new[] { pOSExtraData }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { pOSExtraData }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ExtraData([DataSourceRequest] DataSourceRequest request,
          [Bind(Include = "MasterPOSID,POSExtraDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Ancillary_outpatient_services,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder,AdmissionPhone,ScholarshipRate,AcreditationOnFile,AverageLenOfStay,HowManyUAPanels,HighComplexityCLIA,RegistrationAnalyzer,PaidToPatientState,MedicareNumber")]
            VMPOSLOCExtraData toStore)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var storedInDb = await db.POSExtraDatas.FindAsync(toStore.POSExtraDataID);
                        List<TableInfo> tableColumInfos = new List<TableInfo>();

                        if (storedInDb.Phone_Number != toStore.Phone_Number)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Phone_Number", OldValue = storedInDb.Phone_Number, NewValue = toStore.Phone_Number });
                            storedInDb.Phone_Number = toStore.Phone_Number;
                        }
                        if (storedInDb.Fax_Number != toStore.Fax_Number)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Fax_Number", OldValue = storedInDb.Fax_Number, NewValue = toStore.Fax_Number });
                            storedInDb.Fax_Number = toStore.Fax_Number;
                        }
                        if (storedInDb.Website != toStore.Website)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Website", OldValue = storedInDb.Website, NewValue = toStore.Website });
                            storedInDb.Website = toStore.Website;
                        }
                        if (storedInDb.W_9_on_File != toStore.W_9_on_File)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "W_9_on_File", OldValue = storedInDb.W_9_on_File.ToString(), NewValue = toStore.W_9_on_File.ToString() });
                            storedInDb.W_9_on_File = toStore.W_9_on_File;
                        }
                        if (storedInDb.Number_of_beds != toStore.Number_of_beds)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Number_of_beds", OldValue = storedInDb.Number_of_beds, NewValue = toStore.Number_of_beds });
                            storedInDb.Number_of_beds = toStore.Number_of_beds;
                        }
                        if (storedInDb.Have_24hrs_nursing != toStore.Have_24hrs_nursing)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Have_24hrs_nursing", OldValue = storedInDb.Have_24hrs_nursing.ToString(), NewValue = toStore.Have_24hrs_nursing.ToString() });
                            storedInDb.Have_24hrs_nursing = toStore.Have_24hrs_nursing;
                        }
                        if (storedInDb.how_many_days_week_open != toStore.how_many_days_week_open)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "how_many_days_week_open", OldValue = storedInDb.how_many_days_week_open, NewValue = toStore.how_many_days_week_open });
                            storedInDb.how_many_days_week_open = toStore.how_many_days_week_open;
                        }
                        if (storedInDb.Lab_Name != toStore.Lab_Name)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Lab_Name", OldValue = storedInDb.Lab_Name, NewValue = toStore.Lab_Name });
                            storedInDb.Lab_Name = toStore.Lab_Name;
                        }
                        if (storedInDb.Out_of_Network_In_Network != toStore.Out_of_Network_In_Network)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Out_of_Network_In_Network", OldValue = storedInDb.Out_of_Network_In_Network, NewValue = toStore.Out_of_Network_In_Network });
                            storedInDb.Out_of_Network_In_Network = toStore.Out_of_Network_In_Network;
                        }
                        if (storedInDb.Licensure_on_File != toStore.Licensure_on_File)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Licensure_on_File", OldValue = storedInDb.Licensure_on_File.ToString(), NewValue = toStore.Licensure_on_File.ToString() });
                            storedInDb.Licensure_on_File = toStore.Licensure_on_File;
                        }
                        if (storedInDb.Mental_License != toStore.Mental_License)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Mental_License", OldValue = storedInDb.Mental_License.ToString(), NewValue = toStore.Mental_License.ToString() });
                            storedInDb.Mental_License = toStore.Mental_License;
                        }
                        if (storedInDb.BCBS_ID_Number != toStore.BCBS_ID_Number)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "BCBS_ID_Number", OldValue = storedInDb.BCBS_ID_Number, NewValue = toStore.BCBS_ID_Number });
                            storedInDb.BCBS_ID_Number = toStore.BCBS_ID_Number;
                        }
                        if (storedInDb.UPIN_Number != toStore.UPIN_Number)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "UPIN_Number", OldValue = storedInDb.UPIN_Number, NewValue = toStore.UPIN_Number });
                            storedInDb.UPIN_Number = toStore.UPIN_Number;
                        }
                        if (storedInDb.Medicaid_Number != toStore.Medicaid_Number)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Medicaid_Number", OldValue = storedInDb.Medicaid_Number, NewValue = toStore.Medicaid_Number });
                            storedInDb.Medicaid_Number = toStore.Medicaid_Number;
                        }
                        if (storedInDb.State_of_MD_or_PhyGrp != toStore.State_of_MD_or_PhyGrp)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "State_of_MD_or_PhyGrp", OldValue = storedInDb.State_of_MD_or_PhyGrp, NewValue = toStore.State_of_MD_or_PhyGrp });
                            storedInDb.State_of_MD_or_PhyGrp = toStore.State_of_MD_or_PhyGrp;
                        }
                        if (storedInDb.Regulations_on_File != toStore.Regulations_on_File)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Regulations_on_File", OldValue = storedInDb.Regulations_on_File.ToString(), NewValue = toStore.Regulations_on_File.ToString() });
                            storedInDb.Regulations_on_File = toStore.Regulations_on_File;
                        }
                        if (storedInDb.JACHO_CARF != toStore.JACHO_CARF)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "JACHO_CARF", OldValue = storedInDb.JACHO_CARF, NewValue = toStore.JACHO_CARF });
                            storedInDb.JACHO_CARF = toStore.JACHO_CARF;
                        }
                        if (storedInDb.Has_facility_billed_ins_before != toStore.Has_facility_billed_ins_before)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Has_facility_billed_ins_before", OldValue = storedInDb.Has_facility_billed_ins_before.ToString(), NewValue = toStore.Has_facility_billed_ins_before.ToString() });
                            storedInDb.Has_facility_billed_ins_before = toStore.Has_facility_billed_ins_before;
                        }
                        if (storedInDb.Manage_care_contracts != toStore.Manage_care_contracts)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Manage_care_contracts", OldValue = storedInDb.Manage_care_contracts, NewValue = toStore.Manage_care_contracts });
                            storedInDb.Manage_care_contracts = toStore.Manage_care_contracts;
                        }
                        if (storedInDb.Copies_of_all_managed_care_contracts_on_file != toStore.Copies_of_all_managed_care_contracts_on_file)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Copies_of_all_managed_care_contracts_on_file", OldValue = storedInDb.Copies_of_all_managed_care_contracts_on_file.ToString(), NewValue = toStore.Copies_of_all_managed_care_contracts_on_file.ToString() });
                            storedInDb.Manage_care_contracts = toStore.Manage_care_contracts;
                        }
                        if (storedInDb.Forms_created != toStore.Forms_created)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Forms_created", OldValue = storedInDb.Forms_created.ToString(), NewValue = toStore.Forms_created.ToString() });
                            storedInDb.Forms_created = toStore.Forms_created;
                        }
                        if (storedInDb.In_Service_scheduled != toStore.In_Service_scheduled)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "In_Service_scheduled", OldValue = storedInDb.In_Service_scheduled.ToString(), NewValue = toStore.In_Service_scheduled.ToString() });
                            storedInDb.In_Service_scheduled = toStore.In_Service_scheduled;
                        }
                        if (storedInDb.In_Service_date_time != toStore.In_Service_date_time)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "In_Service_date_time", OldValue = storedInDb.In_Service_date_time.ToString(), NewValue = toStore.In_Service_date_time.ToString() });
                            storedInDb.In_Service_date_time = toStore.In_Service_date_time;
                        }
                        if (storedInDb.Portal_training_setup != toStore.Portal_training_setup)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Portal_training_setup", OldValue = storedInDb.Portal_training_setup.ToString(), NewValue = toStore.Portal_training_setup.ToString() });
                            storedInDb.Portal_training_setup = toStore.Portal_training_setup;
                        }
                        if (storedInDb.email_regarding_conference_set_up != toStore.email_regarding_conference_set_up)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "email_regarding_conference_set_up", OldValue = storedInDb.email_regarding_conference_set_up.ToString(), NewValue = toStore.email_regarding_conference_set_up.ToString() });
                            storedInDb.email_regarding_conference_set_up = toStore.email_regarding_conference_set_up;
                        }
                        if (storedInDb.Database_set_up != toStore.Database_set_up)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Database_set_up", OldValue = storedInDb.Database_set_up.ToString(), NewValue = toStore.Database_set_up.ToString() });
                            storedInDb.Database_set_up = toStore.Database_set_up;
                        }
                        if (storedInDb.Availavility_request_sent_out != toStore.Availavility_request_sent_out)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Availavility_request_sent_out", OldValue = storedInDb.Availavility_request_sent_out.ToString(), NewValue = toStore.Availavility_request_sent_out.ToString() });
                            storedInDb.Availavility_request_sent_out = toStore.Availavility_request_sent_out;
                        }
                        if (storedInDb.Availavility_completed != toStore.Availavility_completed)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Availavility_completed", OldValue = storedInDb.Availavility_completed.ToString(), NewValue = toStore.Availavility_completed.ToString() });
                            storedInDb.Availavility_completed = toStore.Availavility_completed;
                        }
                        if (storedInDb.Navinet_request_completed != toStore.Navinet_request_completed)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Navinet_request_completed", OldValue = storedInDb.Navinet_request_completed.ToString(), NewValue = toStore.Navinet_request_completed.ToString() });
                            storedInDb.Navinet_request_completed = toStore.Navinet_request_completed;
                        }
                        if (storedInDb.Fee_schedule_in_binder != toStore.Fee_schedule_in_binder)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Fee_schedule_in_binder", OldValue = storedInDb.Fee_schedule_in_binder.ToString(), NewValue = toStore.Fee_schedule_in_binder.ToString() });
                            storedInDb.Fee_schedule_in_binder = toStore.Fee_schedule_in_binder;
                        }

                        if (storedInDb.AdmissionPhone != toStore.AdmissionPhone)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "AdmissionPhone", OldValue = storedInDb.AdmissionPhone, NewValue = toStore.AdmissionPhone });
                            storedInDb.AdmissionPhone = toStore.AdmissionPhone;
                        }
                        if (storedInDb.ScholarshipRate != toStore.ScholarshipRate)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "ScholarshipRate", OldValue = storedInDb.ScholarshipRate, NewValue = toStore.ScholarshipRate });
                            storedInDb.ScholarshipRate = toStore.ScholarshipRate;
                        }
                        if (storedInDb.AcreditationOnFile != toStore.AcreditationOnFile)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "AcreditationOnFile", OldValue = storedInDb.AcreditationOnFile.ToString(), NewValue = toStore.AcreditationOnFile.ToString() });
                            storedInDb.AcreditationOnFile = toStore.AcreditationOnFile;
                        }
                        if (storedInDb.AverageLenOfStay != toStore.AverageLenOfStay)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "AverageLenOfStay", OldValue = storedInDb.AverageLenOfStay, NewValue = toStore.AverageLenOfStay });
                            storedInDb.AverageLenOfStay = toStore.AverageLenOfStay;
                        }
                        if (storedInDb.HowManyUAPanels != toStore.HowManyUAPanels)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "HowManyUAPanels", OldValue = storedInDb.HowManyUAPanels, NewValue = toStore.HowManyUAPanels });
                            storedInDb.HowManyUAPanels = toStore.HowManyUAPanels;
                        }
                        if (storedInDb.HighComplexityCLIA != toStore.HighComplexityCLIA)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "HighComplexityCLIA", OldValue = storedInDb.HighComplexityCLIA.ToString(), NewValue = toStore.HighComplexityCLIA.ToString() });
                            storedInDb.HighComplexityCLIA = toStore.HighComplexityCLIA;
                        }
                        if (storedInDb.RegistrationAnalyzer != toStore.RegistrationAnalyzer)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "RegistrationAnalyzer", OldValue = storedInDb.RegistrationAnalyzer.ToString(), NewValue = toStore.RegistrationAnalyzer.ToString() });
                            storedInDb.RegistrationAnalyzer = toStore.RegistrationAnalyzer;
                        }
                        if (storedInDb.PaidToPatientState != toStore.PaidToPatientState)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "PaidToPatientState", OldValue = storedInDb.PaidToPatientState, NewValue = toStore.PaidToPatientState });
                            storedInDb.PaidToPatientState = toStore.PaidToPatientState;
                        }
                        if (storedInDb.MedicareNumber != toStore.MedicareNumber)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "MedicareNumber", OldValue = storedInDb.MedicareNumber, NewValue = toStore.MedicareNumber });
                            storedInDb.MedicareNumber = toStore.MedicareNumber;
                        }
                        if (storedInDb.Ancillary_outpatient_services != toStore.Ancillary_outpatient_services)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "Ancillary_outpatient_services", OldValue = storedInDb.Ancillary_outpatient_services, NewValue = toStore.Ancillary_outpatient_services });
                            storedInDb.Ancillary_outpatient_services = toStore.Ancillary_outpatient_services;
                        }

                        db.POSExtraDatas.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                     
                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "POSExtraDatas",
                            AuditAction = "U",
                            ModelPKey = storedInDb.POSExtraDataID,
                            tableInfos = tableColumInfos
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                      
                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                    } 
                }
            }
            return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ExtraInfo([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,POSExtraDataID,Phone_Number,AdmissionPhone,Fax_Number,Website,ScholarshipRate,AverageLenOfStay,HowManyUAPanels,PaidToPatientState,MedicareNumber,Number_of_beds,how_many_days_week_open,Ancillary_outpatient_services,Out_of_Network_In_Network,Lab_Name,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,JACHO_CARF,In_Service_date_time,Manage_care_contracts")] 
            VMPOSLOCExtraData toStore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.POSExtraDatas.FindAsync(toStore.POSExtraDataID);
                    List<TableInfo> tableColumInfos = new List<TableInfo>();

                    if (storedInDb.Phone_Number != toStore.Phone_Number)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Phone_Number", OldValue = storedInDb.Phone_Number, NewValue = toStore.Phone_Number });
                        storedInDb.Phone_Number = toStore.Phone_Number;
                    }

                    if (storedInDb.Fax_Number != toStore.Fax_Number)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Fax_Number", OldValue = storedInDb.Fax_Number, NewValue = toStore.Fax_Number });
                        storedInDb.Fax_Number = toStore.Fax_Number;
                    }

                    if (storedInDb.Website != toStore.Website)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Website", OldValue = storedInDb.Website, NewValue = toStore.Website });
                        storedInDb.Website = toStore.Website;
                    }

                    if (storedInDb.Number_of_beds != toStore.Number_of_beds)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Number_of_beds", OldValue = storedInDb.Number_of_beds, NewValue = toStore.Number_of_beds });
                        storedInDb.Number_of_beds = toStore.Number_of_beds;
                    }

                    if (storedInDb.how_many_days_week_open != toStore.how_many_days_week_open)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "how_many_days_week_open", OldValue = storedInDb.how_many_days_week_open, NewValue = toStore.how_many_days_week_open });
                        storedInDb.how_many_days_week_open = toStore.how_many_days_week_open;
                    }

                    if (storedInDb.Lab_Name != toStore.Lab_Name)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Lab_Name", OldValue = storedInDb.Lab_Name, NewValue = toStore.Lab_Name });
                        storedInDb.Lab_Name = toStore.Lab_Name;
                    }

                    if (storedInDb.Out_of_Network_In_Network != toStore.Out_of_Network_In_Network)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Out_of_Network_In_Network", OldValue = storedInDb.Out_of_Network_In_Network, NewValue = toStore.Out_of_Network_In_Network });
                        storedInDb.Out_of_Network_In_Network = toStore.Out_of_Network_In_Network;
                    }

                    if (storedInDb.BCBS_ID_Number != toStore.BCBS_ID_Number)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "BCBS_ID_Number", OldValue = storedInDb.BCBS_ID_Number, NewValue = toStore.BCBS_ID_Number });
                        storedInDb.BCBS_ID_Number = toStore.BCBS_ID_Number;
                    }
                    if (storedInDb.UPIN_Number != toStore.UPIN_Number)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "UPIN_Number", OldValue = storedInDb.UPIN_Number, NewValue = toStore.UPIN_Number });
                        storedInDb.UPIN_Number = toStore.UPIN_Number;
                    }
                    if (storedInDb.Medicaid_Number != toStore.Medicaid_Number)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Medicaid_Number", OldValue = storedInDb.Medicaid_Number, NewValue = toStore.Medicaid_Number });
                        storedInDb.Medicaid_Number = toStore.Medicaid_Number;
                    }
                    if (storedInDb.State_of_MD_or_PhyGrp != toStore.State_of_MD_or_PhyGrp)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "State_of_MD_or_PhyGrp", OldValue = storedInDb.State_of_MD_or_PhyGrp, NewValue = toStore.State_of_MD_or_PhyGrp });
                        storedInDb.State_of_MD_or_PhyGrp = toStore.State_of_MD_or_PhyGrp;
                    }

                    if (storedInDb.JACHO_CARF != toStore.JACHO_CARF)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "JACHO_CARF", OldValue = storedInDb.JACHO_CARF, NewValue = toStore.JACHO_CARF });
                        storedInDb.JACHO_CARF = toStore.JACHO_CARF;
                    }

                    if (storedInDb.Manage_care_contracts != toStore.Manage_care_contracts)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Manage_care_contracts", OldValue = storedInDb.Manage_care_contracts, NewValue = toStore.Manage_care_contracts });
                        storedInDb.Manage_care_contracts = toStore.Manage_care_contracts;
                    }

                    if (storedInDb.In_Service_date_time != toStore.In_Service_date_time)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "In_Service_date_time", OldValue = storedInDb.In_Service_date_time.ToString(), NewValue = toStore.In_Service_date_time.ToString() });
                        storedInDb.In_Service_date_time = toStore.In_Service_date_time;
                    }

                    if (storedInDb.AdmissionPhone != toStore.AdmissionPhone)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "AdmissionPhone", OldValue = storedInDb.AdmissionPhone, NewValue = toStore.AdmissionPhone });
                        storedInDb.AdmissionPhone = toStore.AdmissionPhone;
                    }

                    if (storedInDb.ScholarshipRate != toStore.ScholarshipRate)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "ScholarshipRate", OldValue = storedInDb.ScholarshipRate, NewValue = toStore.ScholarshipRate });
                        storedInDb.ScholarshipRate = toStore.ScholarshipRate;
                    }

                    if (storedInDb.AverageLenOfStay != toStore.AverageLenOfStay)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "AverageLenOfStay", OldValue = storedInDb.AverageLenOfStay, NewValue = toStore.AverageLenOfStay });
                        storedInDb.AverageLenOfStay = toStore.AverageLenOfStay;
                    }

                    if (storedInDb.HowManyUAPanels != toStore.HowManyUAPanels)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "HowManyUAPanels", OldValue = storedInDb.HowManyUAPanels, NewValue = toStore.HowManyUAPanels });
                        storedInDb.HowManyUAPanels = toStore.HowManyUAPanels;
                    }

                    if (storedInDb.PaidToPatientState != toStore.PaidToPatientState)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "PaidToPatientState", OldValue = storedInDb.PaidToPatientState, NewValue = toStore.PaidToPatientState });
                        storedInDb.PaidToPatientState = toStore.PaidToPatientState;
                    }

                    if (storedInDb.MedicareNumber != toStore.MedicareNumber)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "MedicareNumber", OldValue = storedInDb.MedicareNumber, NewValue = toStore.MedicareNumber });
                        storedInDb.MedicareNumber = toStore.MedicareNumber;
                    }

                    if (storedInDb.Ancillary_outpatient_services != toStore.Ancillary_outpatient_services)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Ancillary_outpatient_services", OldValue = storedInDb.Ancillary_outpatient_services, NewValue = toStore.Ancillary_outpatient_services });
                        storedInDb.Ancillary_outpatient_services = toStore.Ancillary_outpatient_services;
                    }

                    db.POSExtraDatas.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    if (tableColumInfos.Any())
                    {
                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "POSExtraDatas",
                            AuditAction = "U",
                            ModelPKey = storedInDb.POSExtraDataID,
                            tableInfos = tableColumInfos
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                    }
                }
                catch (Exception)
                {
                  ModelState.AddModelError("","Something failed. Please try again!");
                  return Json(new[] {toStore}.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_ExtraQuestion([DataSourceRequest] DataSourceRequest request,
            [Bind(Exclude = "MasterPOSID,POSExtraDataID,W_9_on_File,Have_24hrs_nursing,Licensure_on_File,Mental_License,Regulations_on_File,Has_facility_billed_ins_before,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder,AcreditationOnFile,HighComplexityCLIA,RegistrationAnalyzer")] 
            VMPOSLOCExtraData toStore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.POSExtraDatas.FindAsync(toStore.POSExtraDataID);
                    List<TableInfo> tableColumInfos = new List<TableInfo>();

                    if (storedInDb. W_9_on_File != toStore.W_9_on_File)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "W_9_on_File", OldValue = storedInDb.W_9_on_File.ToString(), NewValue = toStore.W_9_on_File.ToString() });
                        storedInDb.W_9_on_File = toStore.W_9_on_File;
                    }

                    if (storedInDb.Have_24hrs_nursing != toStore.Have_24hrs_nursing)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Have_24hrs_nursing", OldValue = storedInDb.Have_24hrs_nursing.ToString(), NewValue = toStore.Have_24hrs_nursing.ToString() });
                        storedInDb.Have_24hrs_nursing = toStore.Have_24hrs_nursing;
                    }

                    if (storedInDb.Licensure_on_File != toStore.Licensure_on_File)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Licensure_on_File", OldValue = storedInDb.Licensure_on_File.ToString(), NewValue = toStore.Licensure_on_File.ToString() });
                        storedInDb.Licensure_on_File = toStore.Licensure_on_File;
                    }

                    if (storedInDb.Mental_License != toStore.Mental_License)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Mental_License", OldValue = storedInDb.Mental_License.ToString(), NewValue = toStore.Mental_License.ToString() });
                        storedInDb.Mental_License = toStore.Mental_License;
                    }

                    if (storedInDb.Regulations_on_File != toStore.Regulations_on_File)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Regulations_on_File", OldValue = storedInDb.Regulations_on_File.ToString(), NewValue = toStore.Regulations_on_File.ToString() });
                        storedInDb.Regulations_on_File = toStore.Regulations_on_File;
                    }

                    if (storedInDb.Has_facility_billed_ins_before != toStore.Has_facility_billed_ins_before)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Has_facility_billed_ins_before", OldValue = storedInDb.Has_facility_billed_ins_before.ToString(), NewValue = toStore.Has_facility_billed_ins_before.ToString() });
                        storedInDb.Has_facility_billed_ins_before = toStore.Has_facility_billed_ins_before;
                    }

                    if (storedInDb.Copies_of_all_managed_care_contracts_on_file != toStore.Copies_of_all_managed_care_contracts_on_file)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Copies_of_all_managed_care_contracts_on_file", OldValue = storedInDb.Copies_of_all_managed_care_contracts_on_file.ToString(), NewValue = toStore.Copies_of_all_managed_care_contracts_on_file.ToString() });
                        storedInDb.Copies_of_all_managed_care_contracts_on_file = toStore.Copies_of_all_managed_care_contracts_on_file;
                    }

                    if (storedInDb. Forms_created != toStore.Forms_created)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Forms_created", OldValue = storedInDb.Forms_created.ToString(), NewValue = toStore.Forms_created.ToString() });
                        storedInDb.Forms_created = toStore.Forms_created;
                    }

                    if (storedInDb.In_Service_scheduled != toStore.In_Service_scheduled)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "In_Service_scheduled", OldValue = storedInDb.In_Service_scheduled.ToString(), NewValue = toStore.In_Service_scheduled.ToString() });
                        storedInDb.In_Service_scheduled = toStore.In_Service_scheduled;
                    }

                    if (storedInDb.Portal_training_setup != toStore.Portal_training_setup)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Portal_training_setup", OldValue = storedInDb.Portal_training_setup.ToString(), NewValue = toStore.Portal_training_setup.ToString() });
                        storedInDb.Portal_training_setup = toStore.Portal_training_setup;
                    }

                    if (storedInDb.email_regarding_conference_set_up != toStore.email_regarding_conference_set_up)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "email_regarding_conference_set_up", OldValue = storedInDb.email_regarding_conference_set_up.ToString(), NewValue = toStore.email_regarding_conference_set_up.ToString() });
                        storedInDb.email_regarding_conference_set_up = toStore.email_regarding_conference_set_up;
                    }

                    if (storedInDb.Database_set_up != toStore.Database_set_up)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Database_set_up", OldValue = storedInDb.Database_set_up.ToString(), NewValue = toStore.Database_set_up.ToString() });
                        storedInDb.Database_set_up = toStore.Database_set_up; 
                    }

                    if (storedInDb.Availavility_request_sent_out != toStore.Availavility_request_sent_out)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Availavility_request_sent_out", OldValue = storedInDb.Availavility_request_sent_out.ToString(), NewValue = toStore.Availavility_request_sent_out.ToString() });
                        storedInDb.Availavility_request_sent_out = toStore.Availavility_request_sent_out; 
                    }

                    if (storedInDb.Availavility_completed != toStore.Availavility_completed)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Availavility_completed", OldValue = storedInDb.Availavility_completed.ToString(), NewValue = toStore.Availavility_completed.ToString() });
                        storedInDb.Availavility_completed = toStore.Availavility_completed; 
                    }

                    if (storedInDb.Navinet_request_completed != toStore.Navinet_request_completed)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Navinet_request_completed", OldValue = storedInDb.Navinet_request_completed.ToString(), NewValue = toStore.Navinet_request_completed.ToString() });
                        storedInDb.Navinet_request_completed = toStore.Navinet_request_completed; 
                    }

                    if (storedInDb.Fee_schedule_in_binder != toStore.Fee_schedule_in_binder)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Fee_schedule_in_binder", OldValue = storedInDb.Fee_schedule_in_binder.ToString(), NewValue = toStore.Fee_schedule_in_binder.ToString() });
                        storedInDb.Fee_schedule_in_binder = toStore.Fee_schedule_in_binder; 
                    }

                    if (storedInDb.AcreditationOnFile != toStore.AcreditationOnFile)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "AcreditationOnFile", OldValue = storedInDb.AcreditationOnFile.ToString(), NewValue = toStore.AcreditationOnFile.ToString() });
                        storedInDb.AcreditationOnFile = toStore.AcreditationOnFile; 
                    }

                    if (storedInDb.HighComplexityCLIA != toStore.HighComplexityCLIA)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "HighComplexityCLIA", OldValue = storedInDb.HighComplexityCLIA.ToString(), NewValue = toStore.HighComplexityCLIA.ToString() });
                        storedInDb.HighComplexityCLIA = toStore.HighComplexityCLIA;
                    }

                    if (storedInDb.RegistrationAnalyzer != toStore.RegistrationAnalyzer)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "RegistrationAnalyzer", OldValue = storedInDb.RegistrationAnalyzer.ToString(), NewValue = toStore.RegistrationAnalyzer.ToString() });
                        storedInDb.RegistrationAnalyzer = toStore.RegistrationAnalyzer;
                    }

                    db.POSExtraDatas.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    if (tableColumInfos.Any())
                    {
                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "POSExtraDatas",
                            AuditAction = "U",
                            ModelPKey = storedInDb.POSExtraDataID,
                            tableInfos = tableColumInfos
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] {toStore}.ToDataSourceResult(request, ModelState));
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
