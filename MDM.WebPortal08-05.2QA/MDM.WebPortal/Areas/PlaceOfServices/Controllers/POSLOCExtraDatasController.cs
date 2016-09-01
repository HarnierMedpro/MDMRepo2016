using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eMedServiceCorp.Tools;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class POSLOCExtraDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/POSLOCExtraDatas
        public async Task<ActionResult> Index()
        {
            var pOSLOCExtraDatas = db.POSLOCExtraDatas.Include(p => p.FACInfoData);
            return View(await pOSLOCExtraDatas.ToListAsync());
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Details/5
        /*Una vez se haya creado el FACInfoData de un LocationsPOS, se dibuja un boton Next que muestra los detalles de POSLOCExtraData del recien creado
         FACInfoData, por eso es que se pasa como parametro al Details "parentFacInfoDataID" */
        public async Task<ActionResult> Details(int? parentFacInfoDataID)
        {
            if (parentFacInfoDataID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var FACInfoData = await db.FACInfoDatas.FindAsync(parentFacInfoDataID);
            if (FACInfoData == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var PosLOCExtraData = FACInfoData.POSLOCExtraData;

            if (PosLOCExtraData == null)
            {
                PosLOCExtraData = new POSLOCExtraData();
            }
            else
            {
                string formSent = PosLOCExtraData.Forms_sent.Aggregate("", (current, item) => current + "," + " " + db.FormsDicts.Find(item.FormsDict_FormsID).FormName);
                ViewBag.FormSent = formSent;
            }
            ViewBag.FACInfoDataID = parentFacInfoDataID;
            return View(PosLOCExtraData);
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Create
        /*Para crear un POSLOCExtraDatas, se necesita saber previamente cual es el FACInfoData con el cual va a estar relacionado;
         en caso de que facInfoData sea nulo o el objeto no se encuentre en la BD se tiene que mostrar un error*/
        public async Task<ActionResult> Create(int? facInfoData)
        {
            if (facInfoData != null && await db.FACInfoDatas.FindAsync(facInfoData) != null)
            {
                ViewBag.FACInfoDataID = facInfoData;
                ViewBag.FormsDict = new SelectList(db.FormsDicts.OrderBy(x => x.FormName),"FormsID","FormName");
                //ViewBag.State_of_MD_or_PhyGrp = new AllUSStates().states;   
                return View();
            }
            return RedirectToAction("Index", "Error", new { area = "Error" });
        }

        // POST: PlaceOfServices/POSLOCExtraDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FACInfoData_FACInfoDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder")] 
            POSLOCExtraData pOSLOCExtraData, params int[] SelectedForms)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (SelectedForms != null)
                    {
                        foreach (var item in SelectedForms)
                        {
                            var formsDict = await db.FormsDicts.FindAsync(item);
                            if (formsDict != null)
                            {
                                var formSent = new Forms_sent
                                {
                                    FormsDict_FormsID = item,
                                    POSLOCExtraData_FACInfoData_FACInfoDataID = pOSLOCExtraData.FACInfoData_FACInfoDataID
                                };
                                pOSLOCExtraData.Forms_sent.Add(formSent);
                            }
                        }
                    }
                    db.POSLOCExtraDatas.Add(pOSLOCExtraData);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index","LocationsPOS",new {area="PlaceOfServices"});
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    ViewBag.FACInfoDataID = pOSLOCExtraData.FACInfoData_FACInfoDataID;
                    ViewBag.FormsDict = new SelectList(db.FormsDicts.OrderBy(x => x.FormName), "FormsID", "FormName");
                    return View(pOSLOCExtraData);
                }
            }
            ViewBag.FACInfoDataID = pOSLOCExtraData.FACInfoData_FACInfoDataID;
            ViewBag.FormsDict = new SelectList(db.FormsDicts.OrderBy(x => x.FormName), "FormsID", "FormName");
            return View(pOSLOCExtraData);
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            POSLOCExtraData storedInDb = await db.POSLOCExtraDatas.FindAsync(id);
            if (storedInDb == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var forms = db.FormsDicts.Select(x => new {x.FormsID, x.FormName}).ToList();
            List<string> myCurrentForms = new List<string>();

            if (storedInDb.Forms_sent.Any())
            {
                myCurrentForms.AddRange(from item in storedInDb.Forms_sent where db.FormsDicts.Find(item.FormsDict_FormsID) != null select db.FormsDicts.Find(item.FormsDict_FormsID).FormName);
            }

            VMPOSLOCExtraData toView = new VMPOSLOCExtraData
            {
                FACInfoData_FACInfoDataID = storedInDb.FACInfoData_FACInfoDataID,
                Phone_Number = storedInDb.Phone_Number,
                Fax_Number = storedInDb.Fax_Number,
                Website = storedInDb.Website,
                W_9_on_File = storedInDb.W_9_on_File,
                Number_of_beds = storedInDb.Number_of_beds,
                Have_24hrs_nursing = storedInDb.Have_24hrs_nursing,
                how_many_days_week_open = storedInDb.how_many_days_week_open,
                Lab_Name = storedInDb.Lab_Name,
                Out_of_Network_In_Network = storedInDb.Out_of_Network_In_Network,
                Licensure_on_File = storedInDb.Licensure_on_File,
                Mental_License = storedInDb.Mental_License,
                BCBS_ID_Number = storedInDb.BCBS_ID_Number,
                UPIN_Number = storedInDb.UPIN_Number,
                Medicaid_Number = storedInDb.Medicaid_Number,
                State_of_MD_or_PhyGrp = storedInDb.State_of_MD_or_PhyGrp,
                Regulations_on_File = storedInDb.Regulations_on_File,
                JACHO_CARF = storedInDb.JACHO_CARF,
                Has_facility_billed_ins_before = storedInDb.Has_facility_billed_ins_before,
                Manage_care_contracts = storedInDb.Manage_care_contracts,
                Copies_of_all_managed_care_contracts_on_file = storedInDb.Copies_of_all_managed_care_contracts_on_file,
                Forms_created = storedInDb.Forms_created,
                In_Service_scheduled = storedInDb.In_Service_scheduled,
                In_Service_date_time = storedInDb.In_Service_date_time,
                Portal_training_setup = storedInDb.Portal_training_setup,
                email_regarding_conference_set_up = storedInDb.email_regarding_conference_set_up,
                Database_set_up = storedInDb.Database_set_up,
                Availavility_request_sent_out = storedInDb.Availavility_completed,
                Availavility_completed = storedInDb.Availavility_completed,
                Navinet_request_completed = storedInDb.Navinet_request_completed,
                Fee_schedule_in_binder = storedInDb.Fee_schedule_in_binder,
                formsDictionaries = forms.Select(x => new SelectListItem
                {
                    Text = x.FormName,
                    Value = x.FormsID.ToString(),
                    Selected = myCurrentForms.Contains(x.FormName)
                }).ToList()
            };
            return View(toView);
        }

        // POST: PlaceOfServices/POSLOCExtraDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FACInfoData_FACInfoDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder")] 
            VMPOSLOCExtraData toStore, params int[] SelectedForms)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    /*Obtengo el objeto que se va a modificar de la BD*/
                    var storedInDb = await db.POSLOCExtraDatas.FindAsync(toStore.FACInfoData_FACInfoDataID);

                    /*Obtengo los FormDict del objeto anterior*/
                    var currentForm = storedInDb.Forms_sent.Select(x => x.FormsDict_FormsID != null ? x.FormsDict_FormsID.Value : 0).Where(c => c > 0).ToArray();
                   
                    /*Creo una lista para registrar los campos modificados*/
                    List<TableInfo> tableColumInfos = new List<TableInfo>();

                    if (SelectedForms != null && !SelectedForms.Equals(currentForm))
                    {
                        var formToStore = SelectedForms.Except(currentForm).ToList();

                        foreach (var item in formToStore)
                        {
                            var newForm_Sent = new Forms_sent { FormsDict_FormsID = item, POSLOCExtraData_FACInfoData_FACInfoDataID = storedInDb.FACInfoData_FACInfoDataID };
                            storedInDb.Forms_sent.Add(newForm_Sent);
                        }

                        var formToDelete = currentForm.Except(SelectedForms).ToList();

                        foreach (var item in formToDelete)
                        {
                          storedInDb.Forms_sent.Remove(db.Forms_sent.First(x => x.POSLOCExtraData_FACInfoData_FACInfoDataID == storedInDb.FACInfoData_FACInfoDataID && x.FormsDict_FormsID == item));
                        }

                        var newcurrentForm = storedInDb.Forms_sent.Select(x => x.FormsDict_FormsID != null ? x.FormsDict_FormsID.Value : 0).Where(c => c > 0).ToArray();

                        var oldValue = string.Join(",", currentForm); //C# convert array of integers to comma-separated string
                        var newValue = string.Join(",", newcurrentForm); //C# convert array of integers to comma-separated string

                        TableInfo logForms_Sent = new TableInfo { Field_ColumName = "FormsDict", OldValue = oldValue, NewValue = newValue };
                        tableColumInfos.Add(logForms_Sent);
                       
                    }

                    if (storedInDb.Phone_Number != toStore.Phone_Number)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "Phone_Number", OldValue = storedInDb.Phone_Number, NewValue = toStore.Phone_Number});
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
                   

                    db.POSLOCExtraDatas.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    if (tableColumInfos.Any())
                    {
                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "POSLOCExtraData",
                            AuditAction = "U",
                            ModelPKey = storedInDb.FACInfoData_FACInfoDataID,
                            tableInfos = tableColumInfos
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);
                    }

                    return RedirectToAction("Index","LocationsPOS", new {area ="PlaceOfServices"});

                    
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";

                    var forms1 = db.FormsDicts.Select(x => new { x.FormsID, x.FormName }).ToList();
                    List<string> myCurrentForms1 = new List<string>();
                    if (SelectedForms != null)
                    {
                        myCurrentForms1.AddRange(from item in SelectedForms where db.FormsDicts.Find(item) != null select db.FormsDicts.Find(item).FormName);
                    }
                    toStore.formsDictionaries = forms1.Select(x => new SelectListItem
                    {
                        Text = x.FormName,
                        Value = x.FormsID.ToString(),
                        Selected = myCurrentForms1.Contains(x.FormName)
                    }).ToList();
                    return View(toStore);
                }
                
            }

            var forms = db.FormsDicts.Select(x => new { x.FormsID, x.FormName }).ToList();
            List<string> myCurrentForms = new List<string>();
            if (SelectedForms != null)
            {
                myCurrentForms.AddRange(from item in SelectedForms where db.FormsDicts.Find(item) != null select db.FormsDicts.Find(item).FormName);
            }
            toStore.formsDictionaries = forms.Select(x => new SelectListItem
            {
                Text = x.FormName,
                Value = x.FormsID.ToString(),
                Selected = myCurrentForms.Contains(x.FormName)
            }).ToList();
            return View(toStore);
        }

        // GET: PlaceOfServices/POSLOCExtraDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            POSLOCExtraData pOSLOCExtraData = await db.POSLOCExtraDatas.FindAsync(id);
            if (pOSLOCExtraData == null)
            {
                return HttpNotFound();
            }
            return View(pOSLOCExtraData);
        }

        // POST: PlaceOfServices/POSLOCExtraDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            POSLOCExtraData pOSLOCExtraData = await db.POSLOCExtraDatas.FindAsync(id);
            db.POSLOCExtraDatas.Remove(pOSLOCExtraData);
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
