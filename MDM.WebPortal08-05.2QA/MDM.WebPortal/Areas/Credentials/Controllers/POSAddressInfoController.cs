using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using eMedServiceCorp.Tools;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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

        /*-------------------------KENDO UI FOR ASP.NET MVC5------------------------------------------*/

        public ActionResult Index_Address()
        {
            var result = db.POSAddrs.Select(x => new VMLocationPOS_AddressInfo
            {
                DBA_Name = x.DBA_Name,
                Notes = x.Notes,
                POSAddrID = x.POSAddrID,
                Payment_Addr1 = x.Payment_Addr1,
                Payment_Addr2 = x.Payment_Addr2,
                Payment_City = x.Payment_City,
                Payment_Zip = x.Payment_City,
                Payment_state = x.Payment_state,
                Physical_Addr1 = x.Physical_Addr1,
                Physical_Addr2 = x.Physical_Addr2,
                Physical_City = x.Physical_City,
                Physical_Zip = x.Physical_Zip,
                Physical_state = x.Physical_state,
                Time_Zone = x.Time_Zone
            });
            return View(result);
        }

        public ActionResult Read_AddressInfo([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.POSAddrs.ToList();
            return Json(result.ToDataSourceResult(request, x => new VMLocationPOS_AddressInfo
            {
                DBA_Name = x.DBA_Name,
                Notes = x.Notes,
                POSAddrID = x.POSAddrID,
                Payment_Addr1 = x.Payment_Addr1,
                Payment_Addr2 = x.Payment_Addr2,
                Payment_City = x.Payment_City,
                Payment_Zip = x.Payment_City,
                Payment_state = x.Payment_state,
                Physical_Addr1 = x.Physical_Addr1,
                Physical_Addr2 = x.Physical_Addr2,
                Physical_City = x.Physical_City,
                Physical_Zip = x.Physical_Zip,
                Physical_state = x.Physical_state,
                Time_Zone = x.Time_Zone
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddressInfo(int? MasterPOSID)
        {
            if (MasterPOSID != null && MasterPOSID > 0)
            {
                ViewBag.MasterPOS = MasterPOSID.Value;
            }
            ViewData["TimeZone"] = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "Central Time Zone"},
                new SelectListItem{Text = "Eastern Time Zone", Value = "EST"}, 
                new SelectListItem{Text = "Mountain Time Zone",Value = "MST"},
                new SelectListItem{Value = "PST", Text = "Pacific Time Zone"}
            };
            ViewData["States"] = new AllUSStates().states;
            return View();
        }

        public async Task<ActionResult> Address(int? MasterPOSID)
        {
            if (MasterPOSID == null)
            {
              return RedirectToAction("Error","Error", new{area ="Error"});  
            }
            var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                return RedirectToAction("Error", "Error", new { area = "Error" });
            }
            var addOfThisPos = pos.POSAddr;
            List<VMLocationPOS_AddressInfo> toView = new List<VMLocationPOS_AddressInfo>();
            if (addOfThisPos != null)
            {
                toView.Add(new VMLocationPOS_AddressInfo
                {
                    MasterPOSID = MasterPOSID.Value,
                    DBA_Name = addOfThisPos.DBA_Name,
                    Notes = addOfThisPos.Notes,
                    POSAddrID = addOfThisPos.POSAddrID,
                    Payment_Addr1 = addOfThisPos.Payment_Addr1,
                    Payment_Addr2 = addOfThisPos.Payment_Addr2 ?? "",
                    Payment_City = addOfThisPos.Payment_City,
                    Payment_Zip = addOfThisPos.Payment_Zip,
                    Payment_state = addOfThisPos.Payment_state,
                    Physical_Addr1 = addOfThisPos.Physical_Addr1,
                    Physical_Addr2 = addOfThisPos.Physical_Addr2 ?? "",
                    Physical_City = addOfThisPos.Physical_City,
                    Physical_Zip = addOfThisPos.Physical_Zip,
                    Physical_state = addOfThisPos.Physical_state,
                    Time_Zone = addOfThisPos.Time_Zone
                });
            }
            else
            {
                toView.Add(new VMLocationPOS_AddressInfo{MasterPOSID = MasterPOSID.Value});
            }
            ViewBag.MasterPOS = MasterPOSID.Value;
            ViewData["TimeZone"] = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "Central Time Zone"},
                new SelectListItem{Text = "Eastern Time Zone", Value = "EST"}, 
                new SelectListItem{Text = "Mountain Time Zone",Value = "MST"},
                new SelectListItem{Value = "PST", Text = "Pacific Time Zone"}
            };
            ViewData["States"] = new AllUSStates().states;
            return View(toView);
        }

        public ActionResult Read_States([DataSourceRequest] DataSourceRequest request)
        {
            var result = new AllUSStates().states;
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Read_Address([DataSourceRequest] DataSourceRequest request, int? MasterPOSID)
        {
            //var result = new POSAddr();
            //if (MasterPOSID != null && MasterPOSID > 0)
            //{
            //    var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            //    if (pos != null)
            //    {
            //        var addr = pos.POSAddr;
            //        if (addr != null)
            //        {
            //            result = addr;
            //        }
            //    }
            //}
            //return Json(new[] {result}.ToDataSourceResult(request, x => new VMLocationPOS_AddressInfo
            //{
            //   DBA_Name = x.DBA_Name,
            //   Notes = x.Notes,
            //   POSAddrID = x.POSAddrID,
            //   Payment_Addr1 = x.Payment_Addr1,
            //   Payment_Addr2 = x.Payment_Addr2 ?? "",
            //   Payment_City = x.Payment_City,
            //   Payment_Zip = x.Payment_Zip,
            //   Payment_state = x.Payment_state,
            //   Physical_Addr1 = x.Physical_Addr1,
            //   Physical_Addr2 = x.Physical_Addr2 ?? "",
            //   Physical_City = x.Physical_City,
            //   Physical_Zip = x.Physical_Zip,
            //   Physical_state = x.Physical_state,
            //   Time_Zone = x.Time_Zone
            //}),JsonRequestBehavior.AllowGet);

            var result = new List<VMLocationPOS_AddressInfo>();
            if (MasterPOSID != null && MasterPOSID > 0)
            {
                var pos = await db.MasterPOS.FindAsync(MasterPOSID);
                if (pos != null && pos.POSAddr != null)
                {
                    var addr = pos.POSAddr;
                    result.Add(new VMLocationPOS_AddressInfo
                    {
                        MasterPOSID = MasterPOSID.Value,
                        DBA_Name = addr.DBA_Name,
                        Notes = addr.Notes,
                        POSAddrID = addr.POSAddrID,
                        Payment_Addr1 = addr.Payment_Addr1,
                        Payment_Addr2 = addr.Payment_Addr2 ?? "",
                        Payment_City = addr.Payment_City,
                        Payment_Zip = addr.Payment_Zip,
                        Payment_state = addr.Payment_state,
                        Physical_Addr1 = addr.Physical_Addr1,
                        Physical_Addr2 = addr.Physical_Addr2 ?? "",
                        Physical_City = addr.Physical_City,
                        Physical_Zip = addr.Physical_Zip,
                        Physical_state = addr.Physical_state,
                        Time_Zone = addr.Time_Zone
                    });
                }
            }
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Address([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,POSAddrID,DBA_Name,Payment_Addr1,Payment_Addr2,Payment_City,Payment_state,Payment_Zip,Physical_Addr1,Physical_Addr2,Physical_City,Physical_state,Physical_Zip,Notes,Time_Zone")]
            VMLocationPOS_AddressInfo addressInfo, int ParentID)
        {
            if (ModelState.IsValid)
            {
                if (ParentID == 0)
                {
                    ModelState.AddModelError("", "You are trying to asociate address info to a POS that doesn't exist anymore in our system. Please try again!");
                    return Json(new[] { addressInfo }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbtTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var toStore = new POSAddr
                        {
                            DBA_Name = addressInfo.DBA_Name,
                            Notes = addressInfo.Notes,
                            Payment_Addr1 = addressInfo.Payment_Addr1,
                            Payment_Addr2 = addressInfo.Payment_Addr2,
                            Payment_City = addressInfo.Payment_City,
                            Payment_Zip = addressInfo.Payment_Zip,
                            Payment_state = addressInfo.Payment_state,
                            Physical_Addr1 = addressInfo.Physical_Addr1,
                            Physical_Addr2 = addressInfo.Physical_Addr2,
                            Physical_City = addressInfo.Physical_City,
                            Physical_Zip = addressInfo.Physical_Zip,
                            Physical_state = addressInfo.Physical_state,
                            Time_Zone = addressInfo.Time_Zone
                        };

                        db.POSAddrs.Add(toStore);
                        await db.SaveChangesAsync();
                        addressInfo.POSAddrID = toStore.POSAddrID;
                        addressInfo.MasterPOSID = ParentID;

                        var masterPOS = db.MasterPOS.Find(ParentID);
                        masterPOS.POSAddr_POSAddrID = toStore.POSAddrID;

                        db.MasterPOS.Attach(masterPOS);
                        db.Entry(masterPOS).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        dbtTransaction.Commit();
                    }
                    catch (Exception)
                    {
                      dbtTransaction.Rollback();
                      ModelState.AddModelError("","Something failed. Please try again!");
                      return Json(new[] { addressInfo }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] {addressInfo}.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Address([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,POSAddrID,DBA_Name,Payment_Addr1,Payment_Addr2,Payment_City,Payment_state,Payment_Zip,Physical_Addr1,Physical_Addr2,Physical_City,Physical_state,Physical_Zip,Notes,Time_Zone")]
            VMLocationPOS_AddressInfo addressInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storeInDb = await db.POSAddrs.FindAsync(addressInfo.POSAddrID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storeInDb.Payment_Addr1 != addressInfo.Payment_Addr1)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_Addr1", OldValue = storeInDb.Payment_Addr1, NewValue = addressInfo.Payment_Addr1 });
                        storeInDb.Payment_Addr1 = addressInfo.Payment_Addr1;
                    }
                    if (storeInDb.Payment_Addr2 != addressInfo.Payment_Addr2)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_Addr2", OldValue = storeInDb.Payment_Addr2, NewValue = addressInfo.Payment_Addr2 });
                        storeInDb.Payment_Addr2 = addressInfo.Payment_Addr2;
                    }
                    if (storeInDb.Payment_City != addressInfo.Payment_City)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_City", OldValue = storeInDb.Payment_City, NewValue = addressInfo.Payment_City });
                        storeInDb.Payment_City = addressInfo.Payment_City;
                    }
                    if (storeInDb.Payment_state != addressInfo.Payment_state)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_state", OldValue = storeInDb.Payment_state, NewValue = addressInfo.Payment_state });
                        storeInDb.Payment_state = addressInfo.Payment_state;
                    }
                    if (storeInDb.Payment_Zip != addressInfo.Payment_Zip)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Payment_Zip", OldValue = storeInDb.Payment_Zip, NewValue = addressInfo.Payment_Zip });
                        storeInDb.Payment_Zip = addressInfo.Payment_Zip;
                    }
                    if (storeInDb.Physical_Addr1 != addressInfo.Physical_Addr1)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_Addr1", OldValue = storeInDb.Physical_Addr1, NewValue = addressInfo.Physical_Addr1 });
                        storeInDb.Physical_Addr1 = addressInfo.Physical_Addr1;
                    }
                    if (storeInDb.Physical_Addr2 != addressInfo.Physical_Addr2)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_Addr2", OldValue = storeInDb.Physical_Addr2, NewValue = addressInfo.Physical_Addr2 });
                        storeInDb.Physical_Addr2 = addressInfo.Physical_Addr2;
                    }
                    if (storeInDb.Physical_City != addressInfo.Physical_City)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_City", OldValue = storeInDb.Physical_City, NewValue = addressInfo.Physical_City });
                        storeInDb.Physical_City = addressInfo.Physical_City;
                    }
                    if (storeInDb.Physical_state != addressInfo.Physical_state)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_state", OldValue = storeInDb.Physical_state, NewValue = addressInfo.Physical_state });
                        storeInDb.Physical_state = addressInfo.Physical_state;
                    }
                    if (storeInDb.Physical_Zip != addressInfo.Physical_Zip)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Physical_Zip", OldValue = storeInDb.Physical_Zip, NewValue = addressInfo.Physical_Zip });
                        storeInDb.Physical_Zip = addressInfo.Physical_Zip;
                    }
                    if (storeInDb.Time_Zone != addressInfo.Time_Zone)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Time_Zone", OldValue = storeInDb.Time_Zone, NewValue = addressInfo.Time_Zone });
                        storeInDb.Time_Zone = addressInfo.Time_Zone;
                    }
                    if (!storeInDb.Notes.Equals(addressInfo.Notes, StringComparison.CurrentCulture))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Notes", OldValue = storeInDb.Notes, NewValue = addressInfo.Notes });
                        storeInDb.Notes = addressInfo.Notes;
                    }
                    if (!storeInDb.DBA_Name.Equals(addressInfo.DBA_Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "DBA_Name", OldValue = storeInDb.DBA_Name, NewValue = addressInfo.DBA_Name });
                        storeInDb.DBA_Name = addressInfo.DBA_Name;
                    }

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
                }
                catch (Exception)
                {
                   ModelState.AddModelError("","Something failed. Please try again!");
                   return Json(new[] { addressInfo }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { addressInfo }.ToDataSourceResult(request, ModelState));
        }
        /*-------------------------KENDO UI FOR ASP.NET MVC5------------------------------------------*/


        /*-----------------------------CLASSIC ASP.NET MVC5------------------------------------------*/
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

        public async Task<ActionResult> Create(int? masterPOSID)
        {
            if (masterPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var posName = await db.MasterPOS.FindAsync(masterPOSID);
            if (posName == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            ViewBag.Time_Zone = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "Central Time Zone"},
                new SelectListItem{Text = "Eastern Time Zone", Value = "EST"}, 
                new SelectListItem{Text = "Mountain Time Zone",Value = "MST"},
                new SelectListItem{Value = "PST", Text = "Pacific Time Zone"},
            };
            ViewBag.Payment_state = new AllUSStates().states;
            ViewBag.Physical_state = new AllUSStates().states;
            return View(new VMLocationPOS_AddressInfo{MasterPOSID = masterPOSID.Value});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MasterPOSID,POSAddrID,DBA_Name,Payment_Addr1,Payment_Addr2,Payment_City,Payment_state,Payment_Zip,Physical_Addr1,Physical_Addr2,Physical_City,Physical_state,Physical_Zip,Notes,Time_Zone")]
            VMLocationPOS_AddressInfo addressInfo)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbtTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var toStore = new POSAddr
                        {
                            DBA_Name = addressInfo.DBA_Name,
                            Notes = addressInfo.Notes,
                            Payment_Addr1 = addressInfo.Payment_Addr1,
                            Payment_Addr2 = addressInfo.Payment_Addr2,
                            Payment_City = addressInfo.Payment_City,
                            Payment_Zip = addressInfo.Payment_Zip,
                            Payment_state = addressInfo.Payment_state,
                            Physical_Addr1 = addressInfo.Physical_Addr1,
                            Physical_Addr2 = addressInfo.Physical_Addr2,
                            Physical_City = addressInfo.Physical_City,
                            Physical_Zip = addressInfo.Physical_Zip,
                            Physical_state = addressInfo.Physical_state,
                            Time_Zone = addressInfo.Time_Zone
                        };

                        db.POSAddrs.Add(toStore);
                        await db.SaveChangesAsync();

                        var masterPOS = db.MasterPOS.Find(addressInfo.MasterPOSID);
                        masterPOS.POSAddr_POSAddrID = toStore.POSAddrID;

                        db.MasterPOS.Attach(masterPOS);
                        db.Entry(masterPOS).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        dbtTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbtTransaction.Rollback();
                        TempData["Error"] = "Something failed. Plasease try again!";
                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new {area = "Credentials"});
                    }
                }
            }
            TempData["Error"] = "Invalid Data. Plasease try again!";
            return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
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
        public async Task<ActionResult> Edit([Bind(Include = "MasterPOSID,POSAddrID,DBA_Name,Payment_Addr1,Payment_Addr2,Payment_City,Payment_state,Payment_Zip,Physical_Addr1,Physical_Addr2,Physical_City,Physical_state,Physical_Zip,Notes,Time_Zone")] 
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
                    if (storeInDb.Notes.Equals(locPosAddressInfo.Notes, StringComparison.CurrentCulture))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Notes", OldValue = storeInDb.Notes, NewValue = locPosAddressInfo.Notes });
                        storeInDb.Notes = locPosAddressInfo.Notes;
                    }
                    if (storeInDb.DBA_Name.Equals(locPosAddressInfo.DBA_Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "DBA_Name", OldValue = storeInDb.DBA_Name, NewValue = locPosAddressInfo.DBA_Name });
                        storeInDb.DBA_Name = locPosAddressInfo.DBA_Name;
                    }

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

        /*-----------------------------CLASSIC ASP.NET MVC5------------------------------------------*/

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
