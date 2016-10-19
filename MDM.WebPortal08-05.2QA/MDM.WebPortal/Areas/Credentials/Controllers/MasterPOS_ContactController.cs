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
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class MasterPOS_ContactController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();
        
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
            if (MasterPOSID <= 0 || !Contacts.Any())
            {
                return Json(new List<MasterPOS_Contact>(), JsonRequestBehavior.AllowGet);
            }
            using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var currentContacts = db.MasterPOS_Contact.Where(x => x.MasterPOS_MasterPOSID == MasterPOSID).Select(x => x.ContactID).ToArray();
                    var newCnt = Contacts.Except(currentContacts).ToList();
                    var toStore = newCnt.Select(x => new MasterPOS_Contact
                    {
                        MasterPOS_MasterPOSID = MasterPOSID, 
                        ContactID = x
                    }).ToList();

                    db.MasterPOS_Contact.AddRange(toStore);
                    await db.SaveChangesAsync();

                    var auditLogs = toStore.Select(x => new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        TableName = "MasterPOS_Contact",
                        AuditAction = "I",
                        ModelPKey = x.MasterPOSContactID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "ContactID", NewValue = x.ContactID.ToString()},
                            new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = x.MasterPOS_MasterPOSID.ToString()},
                        }
                    }).ToList();

                    new AuditLogRepository().SaveLogs(auditLogs);

                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return Json(new List<MasterPOS_Contact>(), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(Contacts.Select(x => new MasterPOS_Contact{ContactID = x, MasterPOS_MasterPOSID = MasterPOSID}), JsonRequestBehavior.AllowGet);
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
