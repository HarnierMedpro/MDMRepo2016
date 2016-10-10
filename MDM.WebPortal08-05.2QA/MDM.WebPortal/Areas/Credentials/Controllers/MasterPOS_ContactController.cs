using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class MasterPOS_ContactController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

      

        // GET: Credentials/MasterPOS_Contact/Create
        public async Task<ActionResult> Create(int? masterPOSID)
        {
            if (masterPOSID == null)
            {
               return RedirectToAction("Index","Error", new{area="BadRequest"}); 
            }
            var masterPOS = await db.MasterPOS.FindAsync(masterPOSID);
            if (masterPOS == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" }); 
            }
            var toView = new VMMasterPOS_Contact {MasterPOS_MasterPOSID = masterPOSID.Value};
            ViewBag.MasterPOS = masterPOSID;
            return View(toView);
        }

        // POST: Credentials/MasterPOS_Contact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MasterPOSContactID,MasterPOS_MasterPOSID")] VMMasterPOS_Contact masterPosContact, params int[] corpCnt)
        {
            if (ModelState.IsValid)
            {
                if (!corpCnt.Any())
                {
                    TempData["Error"] = "You have to choose at least one CONTACT. Please try again!";
                    return RedirectToAction("Index_MasterPOS", "MasterPOS", new {area = "Credentials"});
                }
                try
                {
                    var toStore = new List<MasterPOS_Contact>();
                    foreach (var cnt in corpCnt)
                    {
                        if (!await db.MasterPOS_Contact.AnyAsync( x => x.ContactID == cnt && x.MasterPOS_MasterPOSID == masterPosContact.MasterPOS_MasterPOSID))
                        {
                            toStore.Add(new MasterPOS_Contact
                            {
                                ContactID = cnt,
                                MasterPOS_MasterPOSID = masterPosContact.MasterPOS_MasterPOSID
                            });
                        }
                    }
                    if (toStore.Any())
                    {
                        db.MasterPOS_Contact.AddRange(toStore);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new {area = "Credentials"});
                    }
                    else
                    {
                        TempData["Error"] = "All the Contacts choosen are already asociated to this POS.";
                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                    }
                }

                catch (Exception)
                {
                    TempData["Error"] = "Something failed. Please try again!";
                    return RedirectToAction("Index_MasterPOS", "MasterPOS", new {area = "Credentials"});
                }
            }
            return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
        }


        public async Task<ActionResult> Save_POSContacts([DataSourceRequest] DataSourceRequest request, int MasterPOSID, int[] Contacts)
        {
            if (MasterPOSID < 0 || !Contacts.Any())
            {
                ModelState.AddModelError("","Something failed. Please try again!");
            }
            try
            {
                var toStore = Contacts.Select(x => new MasterPOS_Contact { MasterPOS_MasterPOSID = MasterPOSID, ContactID = x }).ToList();
                db.MasterPOS_Contact.AddRange(toStore);
                await db.SaveChangesAsync();
                //ModelState.AddModelError("","Success.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something failed. Please try again!");
            }
            
            return Json(new List<VMContact>());
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
