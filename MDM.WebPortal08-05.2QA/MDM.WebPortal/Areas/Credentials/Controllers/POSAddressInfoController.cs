using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class POSAddressInfoController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        //// GET: PlaceOfServices/POSAddressInfo
        public ActionResult Index()
        {
            return View();
        }
        // GET: OurTeams/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var posName = await db.MasterPOS.FindAsync(id);
            if (posName == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var posAddress = posName.POSAddr;
            if (posAddress == null)
            {
                return View(new VMLocationPOS_AddressInfo{MasterPOSID = id.Value});
            }
            var toView = new VMLocationPOS_AddressInfo
            {
                MasterPOSID = id.Value,
                POSAddrID = posAddress.POSAddrID,
                DBA_Name = posAddress.DBA_Name,
                Payment_Addr1 = posAddress.Payment_Addr1,
                Payment_Addr2 = posAddress.Payment_Addr2,
                Payment_City = posAddress.Payment_City,
                Payment_state = posAddress.Payment_state,
                Payment_Zip = posAddress.Payment_Zip,

                Physical_Addr1 = posAddress.Physical_Addr1,
                Physical_Addr2 = posAddress.Physical_Addr2,
                Physical_City = posAddress.Physical_City,
                Physical_state = posAddress.Physical_state,
                Physical_Zip = posAddress.Physical_Zip,

                Notes = posAddress.Notes,
                Time_Zone = posAddress.Time_Zone
            };
            return View(toView);
        }


        // GET: PlaceOfServices/POSAddressInfo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var posAddress = await db.POSAddrs.FindAsync(id);

            if (posAddress == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var toView = new VMLocationPOS_AddressInfo
            {
                MasterPOSID = id.Value,
                POSAddrID = posAddress.POSAddrID,
                DBA_Name = posAddress.DBA_Name,

                Payment_Addr1 = posAddress.Payment_Addr1,
                Payment_Addr2 = posAddress.Payment_Addr2,
                Payment_City = posAddress.Payment_City,
                Payment_state = posAddress.Payment_state,
                Payment_Zip = posAddress.Payment_Zip,

                Physical_Addr1 = posAddress.Physical_Addr1,
                Physical_Addr2 = posAddress.Physical_Addr2,
                Physical_City = posAddress.Physical_City,
                Physical_state = posAddress.Physical_state,
                Physical_Zip = posAddress.Physical_Zip,

                Notes = posAddress.Notes,
                Time_Zone = posAddress.Time_Zone
            };
            var allStates = new AllUSStates().states;
            var allStates1 = new AllUSStates().states;

            if (toView.Payment_state != null)
            {
                allStates.Find(x => x.Value == toView.Payment_state).Selected = true;
            }

            if (toView.Physical_state != null)
            {
                allStates1.Find(x => x.Value == toView.Physical_state).Selected = true;
            }

            ViewBag.Payment_state = allStates;
            ViewBag.Physical_state = allStates1;
            ViewBag.Time_Zone = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "Central Time Zone", Selected = toView.Time_Zone != null && toView.Time_Zone.Contains("CST")},
                new SelectListItem{Text = "Eastern Time Zone", Value = "EST", Selected = toView.Time_Zone != null && toView.Time_Zone.Contains("EST")}, 
                new SelectListItem{Text = "Mountain Time Zone",Value = "MST", Selected = toView.Time_Zone != null && toView.Time_Zone.Contains("MST")},
                new SelectListItem{Value = "PST", Text = "Pacific Time Zone", Selected = toView.Time_Zone != null && toView.Time_Zone.Contains("PST")},
            };

            return View(toView);
        }

        // POST: PlaceOfServices/POSAddressInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MasterPOSID, Payment_Addr1, Payment_Addr2, Payment_City, Payment_state, Payment_Zip, Physical_Addr1, Physical_Addr2, Physical_City, Physical_state, Physical_Zip, Notes, Time_Zone")] 
            VMLocationPOS_AddressInfo locPosAddressInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storeInDb = await db.POSAddrs.FindAsync(locPosAddressInfo.MasterPOSID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storeInDb.Payment_Addr1 != locPosAddressInfo.Payment_Addr1)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_Addr1", OldValue = storeInDb.Payment_Addr1, NewValue = locPosAddressInfo.Payment_Addr1 });
                        storeInDb.Payment_Addr1 = locPosAddressInfo.Payment_Addr1;
                    }
                    if (storeInDb.Payment_Addr2 != locPosAddressInfo.Payment_Addr2)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_Addr2", OldValue = storeInDb.Payment_Addr2, NewValue = locPosAddressInfo.Payment_Addr2 });
                        storeInDb.Payment_Addr2 = locPosAddressInfo.Payment_Addr2;
                    }
                    if (storeInDb.Payment_City != locPosAddressInfo.Payment_City)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_City", OldValue = storeInDb.Payment_City, NewValue = locPosAddressInfo.Payment_City });
                        storeInDb.Payment_City = locPosAddressInfo.Payment_City;
                    }
                    if (storeInDb.Payment_state != locPosAddressInfo.Payment_state)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_state", OldValue = storeInDb.Payment_state, NewValue = locPosAddressInfo.Payment_state });
                        storeInDb.Payment_state = locPosAddressInfo.Payment_state;
                    }
                    if (storeInDb.Payment_Zip != locPosAddressInfo.Payment_Zip)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_Zip", OldValue = storeInDb.Payment_Zip, NewValue = locPosAddressInfo.Payment_Zip });
                        storeInDb.Payment_Zip = locPosAddressInfo.Payment_Zip;
                    }
                    if (storeInDb.Physical_Addr1 != locPosAddressInfo.Physical_Addr1)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_Addr1", OldValue = storeInDb.Physical_Addr1, NewValue = locPosAddressInfo.Physical_Addr1 });
                        storeInDb.Physical_Addr1 = locPosAddressInfo.Physical_Addr1;
                    }
                    if (storeInDb.Physical_Addr2 != locPosAddressInfo.Physical_Addr2)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_Addr2", OldValue = storeInDb.Physical_Addr2, NewValue = locPosAddressInfo.Physical_Addr2 });
                        storeInDb.Physical_Addr2 = locPosAddressInfo.Physical_Addr2;
                    }
                    if (storeInDb.Physical_City != locPosAddressInfo.Physical_City)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_City", OldValue = storeInDb.Physical_City, NewValue = locPosAddressInfo.Physical_City });
                        storeInDb.Physical_City = locPosAddressInfo.Physical_City;
                    }
                    if (storeInDb.Physical_state != locPosAddressInfo.Physical_state)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_state", OldValue = storeInDb.Physical_state, NewValue = locPosAddressInfo.Physical_state });
                        storeInDb.Physical_state = locPosAddressInfo.Physical_state;
                    }
                    if (storeInDb.Physical_Zip != locPosAddressInfo.Physical_Zip)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_Zip", OldValue = storeInDb.Physical_Zip, NewValue = locPosAddressInfo.Physical_Zip });
                        storeInDb.Physical_Zip = locPosAddressInfo.Physical_Zip;
                    }
                    if (storeInDb.Time_Zone != locPosAddressInfo.Time_Zone)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Time_Zone", OldValue = storeInDb.Time_Zone, NewValue = locPosAddressInfo.Time_Zone });
                        storeInDb.Time_Zone = locPosAddressInfo.Time_Zone;
                    }
                    //if (storeInDb.Notes.Equals(locPosAddressInfo.Notes, StringComparison.CurrentCulture))
                    //{
                    //    tableColumnInfos.Add(new TableInfo { Field_ColumName = "Notes", OldValue = storeInDb.Notes, NewValue = locPosAddressInfo.Notes });
                    storeInDb.Notes = locPosAddressInfo.Notes;
                    //}

                    db.POSAddrs.Attach(storeInDb);
                    db.Entry(storeInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        TableName = "POSAddrs",
                        AuditAction = "U",
                        ModelPKey = storeInDb.POSAddrID,
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                    return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                }
                catch
                {
                    ViewBag.Error = "Something failed. Please try again!";

                    var allStatesPay = new AllUSStates().states;
                    var allStatesPhy = new AllUSStates().states;

                    if (locPosAddressInfo.Payment_state != null)
                    {
                        allStatesPay.Find(x => x.Value == locPosAddressInfo.Payment_state).Selected = true;
                    }

                    if (locPosAddressInfo.Physical_state != null)
                    {
                        allStatesPhy.Find(x => x.Value == locPosAddressInfo.Physical_state).Selected = true;
                    }

                    ViewBag.Payment_state = allStatesPay;
                    ViewBag.Physical_state = allStatesPhy;
                    ViewBag.Time_Zone = new List<SelectListItem>
                        {
                            new SelectListItem{Value = "CST", Text = "Central Time Zone", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("CST")},
                            new SelectListItem{Text = "Eastern Time Zone", Value = "EST", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("EST")}, 
                            new SelectListItem{Text = "Mountain Time Zone",Value = "MST", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("MST")},
                            new SelectListItem{Value = "PST", Text = "Pacific Time Zone", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("PST")},
                        };

                    return View(locPosAddressInfo);
                }
            }
            var allStates = new AllUSStates().states;
            var allStates1 = new AllUSStates().states;

            if (locPosAddressInfo.Payment_state != null)
            {
                allStates.Find(x => x.Value == locPosAddressInfo.Payment_state).Selected = true;
            }

            if (locPosAddressInfo.Physical_state != null)
            {
                allStates1.Find(x => x.Value == locPosAddressInfo.Physical_state).Selected = true;
            }

            ViewBag.Payment_state = allStates;
            ViewBag.Physical_state = allStates1;
            ViewBag.Time_Zone = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "Central Time Zone", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("CST")},
                new SelectListItem{Text = "Eastern Time Zone", Value = "EST", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("EST")}, 
                new SelectListItem{Text = "Mountain Time Zone",Value = "MST", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("MST")},
                new SelectListItem{Value = "PST", Text = "Pacific Time Zone", Selected = locPosAddressInfo.Time_Zone != null && locPosAddressInfo.Time_Zone.Contains("PST")},
            };

            return View(locPosAddressInfo);

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
