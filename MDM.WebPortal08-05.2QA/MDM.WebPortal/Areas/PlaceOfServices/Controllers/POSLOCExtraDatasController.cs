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
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;

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
        public async Task<ActionResult> Create([Bind(Include = "FACInfoData_FACInfoDataID,Phone_Number,Fax_Number,Website,W_9_on_File,Number_of_beds,Have_24hrs_nursing,how_many_days_week_open,Lab_Name,Out_of_Network_In_Network,Licensure_on_File,Mental_License,BCBS_ID_Number,UPIN_Number,Medicaid_Number,State_of_MD_or_PhyGrp,Regulations_on_File,JACHO_CARF,Has_facility_billed_ins_before,Manage_care_contracts,Copies_of_all_managed_care_contracts_on_file,Forms_created,In_Service_scheduled,In_Service_date_time,Portal_training_setup,email_regarding_conference_set_up,Database_set_up,Availavility_request_sent_out,Availavility_completed,Navinet_request_completed,Fee_schedule_in_binder")] POSLOCExtraData pOSLOCExtraData,
            params int[] SelectedForms)
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
                    if (SelectedForms != null)
                    {
                        
                    }
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
