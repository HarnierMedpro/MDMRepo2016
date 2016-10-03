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
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class PHYGroupsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       
        public ActionResult Index()
        {
            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider{ProvID = x.ProvID,ProviderName = x.ProviderName,NPI_Num = x.NPI_Num});
            return View();
        }

        public ActionResult Read_Groups([DataSourceRequest] DataSourceRequest request)
        {
            /*See: C:\Users\hsuarez\Desktop\Documentation\Kendo UI for ASP.NET MVC5\ui-for-aspnet-mvc-examples-master\grid\multiselect-in-grid-popup*/
            return Json(db.PHYGroups.Include(x => x.ProvidersInGrps).ToDataSourceResult(request, x => new VMPHYGrp
            {
                PHYGrpID = x.PHYGrpID,
                PHYGroupName = x.PHYGroupName,
                PHYGrpNPI_Num = x.PHYGrpNPI_Num,
                Physicians = from pInG in x.ProvidersInGrps
                             join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                             select new VMProvider
                                 {
                                     ProvID = prov.ProvID,
                                     ProviderName = prov.ProviderName,
                                     NPI_Num = prov.NPI_Num
                                 }
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Groups([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,PHYGrpID,PHYGroupName,PHYGrpNPI_Num,Physicians")] VMPHYGrp toStore)
        {
            if (ModelState.IsValid)
            {
                //if (!toStore.Physicians.Any())
                //{
                //    ModelState.AddModelError("", "You have to select at least one Physician. Please try again!");
                //    return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                //}
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.PHYGroups.AnyAsync(grp => grp.PHYGroupName.Equals(toStore.PHYGroupName, StringComparison.InvariantCultureIgnoreCase)
                                        || grp.PHYGrpNPI_Num.Equals(toStore.PHYGrpNPI_Num, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                        }

                        var newGrp = new PHYGroup
                        {
                            PHYGroupName = toStore.PHYGroupName,
                            PHYGrpNPI_Num = toStore.PHYGrpNPI_Num
                        };
                        db.PHYGroups.Add(newGrp);
                        await db.SaveChangesAsync();
                        toStore.PHYGrpID = newGrp.PHYGrpID;

                        var providersInGrp = toStore.Physicians.Select(doc => new ProvidersInGrp { PHYGroups_PHYGrpID = newGrp.PHYGrpID, Providers_ProvID = doc.ProvID }).ToList();
                        db.ProvidersInGrps.AddRange(providersInGrp);

                        //saves all above operations within one transaction
                        await db.SaveChangesAsync();

                        //commit transaction
                        dbTransaction.Commit();

                        AuditLogRepository repository = new AuditLogRepository();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            ModelPKey = newGrp.PHYGrpID,
                            TableName = "PHYGroups",
                            AuditAction = "I",
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "PHYGroupName", NewValue = newGrp.PHYGroupName},
                                new TableInfo{Field_ColumName = "PHYGrpNPI_Num", NewValue = newGrp.PHYGrpNPI_Num}
                            }
                        };

                        repository.AddAuditLogs(auditLog);

                        foreach (var item in providersInGrp)
                        {
                            auditLog.ModelPKey = item.ProvInGrpID;
                            auditLog.TableName = "ProvidersInGrps";
                            auditLog.AuditAction = "I";
                            auditLog.tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", NewValue = item.PHYGroups_PHYGrpID.ToString()},
                                new TableInfo{Field_ColumName = "Providers_ProvID", NewValue = item.Providers_ProvID.ToString()}
                            };

                            repository.AddAuditLogs(auditLog);
                        }
                    }
                    catch (Exception)
                    {
                        //Rollback transaction if exception occurs
                        dbTransaction.Rollback();

                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Groups([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "Facitity_DBs_IDPK,PHYGrpID,PHYGroupName,PHYGrpNPI_Num,Physicians")] VMPHYGrp toStore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.PHYGroups.AnyAsync(grp => (grp.PHYGroupName.Equals(toStore.PHYGroupName, StringComparison.InvariantCultureIgnoreCase)
                                        || grp.PHYGrpNPI_Num.Equals(toStore.PHYGrpNPI_Num, StringComparison.InvariantCultureIgnoreCase)) && grp.PHYGrpID != toStore.PHYGrpID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.PHYGroups.FindAsync(toStore.PHYGrpID);
                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    var physiciansByParam = toStore.Physicians.Select(x => x.ProvID).ToArray();

                    var currentPhysicians = storedInDb.ProvidersInGrps.Select(x => x.Providers_ProvID).ToArray();

                    if (!currentPhysicians.Equals(physiciansByParam))
                    {
                        var physicianToStore = physiciansByParam.Except(currentPhysicians).ToList();

                        foreach (var item in physicianToStore)
                        {
                            storedInDb.ProvidersInGrps.Add(new ProvidersInGrp { Providers_ProvID = item, PHYGroups_PHYGrpID = storedInDb.PHYGrpID });
                        }

                        var physicianToDelete = currentPhysicians.Except(physiciansByParam).ToList();

                        foreach (var provId in physicianToDelete)
                        {
                            var proInGrpToDelete = await db.ProvidersInGrps.FirstOrDefaultAsync(x => x.Providers_ProvID == provId && x.PHYGroups_PHYGrpID == storedInDb.PHYGrpID);
                            if (proInGrpToDelete != null)
                            {
                                storedInDb.ProvidersInGrps.Remove(proInGrpToDelete);
                            }
                        }

                        var oldValue = string.Join(",", currentPhysicians); //C# convert array of integers to comma-separated string
                        var newValue = string.Join(",", storedInDb.ProvidersInGrps.Select(x => x.Providers_ProvID).ToArray()); //C# convert array of integers to comma-separated string
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "Providers", OldValue = oldValue, NewValue = newValue });
                    }
                    if (storedInDb.PHYGroupName != toStore.PHYGroupName)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PHYGroupName", OldValue = storedInDb.PHYGroupName, NewValue = toStore.PHYGroupName });
                        storedInDb.PHYGroupName = toStore.PHYGroupName;
                    }
                    if (storedInDb.PHYGrpNPI_Num != toStore.PHYGrpNPI_Num)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "PHYGrpNPI_Num", OldValue = storedInDb.PHYGrpNPI_Num, NewValue = toStore.PHYGrpNPI_Num });
                        storedInDb.PHYGrpNPI_Num = toStore.PHYGrpNPI_Num;
                    }
                    db.PHYGroups.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        TableName = "PHYGroups",
                        AuditAction = "U",
                        ModelPKey = toStore.PHYGrpID,
                        tableInfos = tableColumnInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> PHYGrp_Info(int? MasterPOSID)
        {
            if (MasterPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            MasterPOS pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var phyG = pos.PHYGroup;
            var toView = new List<VMPHYGrp>();
            if (phyG != null)
            {
                toView.Add(new VMPHYGrp
                {
                    MasterPOSID = MasterPOSID.Value,
                    PHYGrpID = phyG.PHYGrpID,
                    PHYGroupName = phyG.PHYGroupName,
                    PHYGrpNPI_Num = phyG.PHYGrpNPI_Num,
                    Physicians = from pInG in phyG.ProvidersInGrps join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                                 select new VMProvider
                                 {
                                     ProvID = prov.ProvID,
                                     ProviderName = prov.ProviderName,
                                     NPI_Num = prov.NPI_Num
                                 }
                });
            }
            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }
        
        public async Task<ActionResult> Details(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            MasterPOS pos = await db.MasterPOS.FindAsync(locationPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            if (pos.FvPList.FvPName == "FAC")
            {
                ViewBag.FAC = "A Facility doesn't have a Physician Group.";
                //return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
            }
            PHYGroup pHYGroup = pos.PHYGroup;
            VMPHYGrp toView = new VMPHYGrp();
            if (pHYGroup == null)
            {
                //pHYGroup = new PHYGroup();
                toView.MasterPOSID = locationPOSID.Value;
            }
            else
            {
                toView.PHYGrpID = pHYGroup.PHYGrpID;
                toView.PHYGroupName = pHYGroup.PHYGroupName;
                toView.PHYGrpNPI_Num = pHYGroup.PHYGrpNPI_Num;

                if (pHYGroup.ProvidersInGrps.Any())
                {
                    /*USING LINQ to dbo.ProvidersInGrps and dbo.Providers table*/
                    //var myCurrentProviders = from item in pHYGroup.ProvidersInGrps where db.Providers.Find(item.Providers_ProvID) != null select db.Providers.Find(item.Providers_ProvID);
                    //toView.Physicians = myCurrentProviders.Select(x => new VMProvider
                    //{
                    //    ProvID = x.ProvID,
                    //    ProviderName = x.ProviderName,
                    //    NPI_Num = x.NPI_Num
                    //}).ToList();

                    //OR

                    /*INNER JOIN Between dbo.ProvidersInGrps and dbo.Providers table*/
                    var myCurrentProviders = from pInG in pHYGroup.ProvidersInGrps
                                             join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                                             select new VMProvider
                                                 {
                                                     ProvID = prov.ProvID,
                                                     ProviderName = prov.ProviderName,
                                                     NPI_Num = prov.NPI_Num
                                                 };

                    toView.Physicians = myCurrentProviders.ToList();
                }
            }
            ViewBag.Facitity_DBs_IDPK = locationPOSID;
            return View(toView);
        }

        //[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseGrp([Bind(Include = "PHYGrpID,MasterPOSID")] VMChooseGrp toAssign)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var locPos = await db.MasterPOS.FindAsync(toAssign.MasterPOSID);
                    var currentGrp = locPos.PHYGroups_PHYGrpID;
                    var phyGroup = await db.PHYGroups.FindAsync(toAssign.PHYGrpID);
                    if (phyGroup.MasterPOS.Any())
                    {
                        TempData["Error"] = "This group is asociated with other POS. Please try again!";
                        return RedirectToAction("Create", new { locationPOSID = toAssign.MasterPOSID });
                    }

                    locPos.PHYGroup = phyGroup;
                    db.MasterPOS.Attach(locPos);
                    db.Entry(locPos).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        ModelPKey = locPos.MasterPOSID,
                        TableName = "LocationsPOS",
                        tableInfos = new List<TableInfo> { new TableInfo { Field_ColumName = "PHYGroups_PHYGrpID", OldValue = currentGrp.ToString(), NewValue = phyGroup.PHYGrpID.ToString() } }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                    return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                }
                catch (Exception)
                {
                    TempData["Error"] = "Something failed. Please try again!";
                    return RedirectToAction("Create", new { locationPOSID = toAssign.MasterPOSID });
                }
            }
            TempData["Error"] = "Something failed. Please try again!";
            return RedirectToAction("Create", new { locationPOSID = toAssign.MasterPOSID });
        }

        
        public async Task<ActionResult> Create(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            MasterPOS pos = await db.MasterPOS.FindAsync(locationPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            ViewBag.Facitity_DBs_IDPK = locationPOSID;

            /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
             pertenecer a un POS*/
            var groups = db.PHYGroups.Where(x => !x.MasterPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
            ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");

            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider{ ProvID = x.ProvID,ProviderName = x.ProviderName,NPI_Num = x.NPI_Num});

            return View(new VMPHYGrp{MasterPOSID = locationPOSID.Value});
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MasterPOSID,PHYGrpID,PHYGroupName,PHYGrpNPI_Num")] VMPHYGrp toStore, params int[] Physicians)
        {
            if (ModelState.IsValid && Physicians != null && await db.MasterPOS.FindAsync(toStore.MasterPOSID) != null)
            {

                if (await db.PHYGroups.AnyAsync(grp => grp.PHYGroupName.Equals(toStore.PHYGroupName, StringComparison.InvariantCultureIgnoreCase)
                                || grp.PHYGrpNPI_Num.Equals(toStore.PHYGrpNPI_Num, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ViewBag.Doc = "Duplicate data. Please try again!";

                    ViewBag.Facitity_DBs_IDPK = toStore.MasterPOSID;

                    /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
                    pertenecer a un POS*/
                    var phyGroups = db.PHYGroups.Where(x => !x.MasterPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
                    ViewBag.PHYGrpID = new SelectList(phyGroups, "PHYGrpID", "PHYGroupName");

                    var providersSelected1 = from doc in Physicians join prov in db.Providers on doc equals prov.ProvID
                                             select new VMProvider
                                                 {
                                                     ProvID = prov.ProvID,
                                                     ProviderName = prov.ProviderName,
                                                     NPI_Num = prov.NPI_Num
                                                 };

                    toStore.Physicians = providersSelected1.ToList();

                    ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider{ ProvID = x.ProvID,ProviderName = x.ProviderName,NPI_Num = x.NPI_Num});

                    return View(toStore);
                }

                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var newGrp = new PHYGroup
                        {
                            PHYGroupName = toStore.PHYGroupName,
                            PHYGrpNPI_Num = toStore.PHYGrpNPI_Num,
                            MasterPOS = new List<MasterPOS> { await db.MasterPOS.FindAsync(toStore.MasterPOSID) }
                        };
                        db.PHYGroups.Add(newGrp);
                        await db.SaveChangesAsync();

                        var providersInGrp = Physicians.Select(doc => new ProvidersInGrp { PHYGroups_PHYGrpID = newGrp.PHYGrpID, Providers_ProvID = doc }).ToList();

                        db.ProvidersInGrps.AddRange(providersInGrp);
                        await db.SaveChangesAsync();
                        dbTransaction.Commit();

                        AuditLogRepository repository = new AuditLogRepository();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            ModelPKey = toStore.MasterPOSID,
                            TableName = "LocationsPOS",
                            AuditAction = "U",
                            tableInfos = new List<TableInfo> { new TableInfo { Field_ColumName = "PHYGroups_PHYGrpID", NewValue = newGrp.PHYGrpID.ToString() } }
                        };

                        repository.AddAuditLogs(auditLog);

                        auditLog.ModelPKey = newGrp.PHYGrpID;
                        auditLog.TableName = "PHYGroups";
                        auditLog.AuditAction = "I";
                        auditLog.tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "PHYGroupName", NewValue = newGrp.PHYGroupName},
                            new TableInfo{Field_ColumName = "PHYGrpNPI_Num", NewValue = newGrp.PHYGrpNPI_Num}
                        };

                        repository.AddAuditLogs(auditLog);

                        foreach (var item in providersInGrp)
                        {
                            auditLog.ModelPKey = item.ProvInGrpID;
                            auditLog.TableName = "ProvidersInGrps";
                            auditLog.AuditAction = "I";
                            auditLog.tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", NewValue = item.PHYGroups_PHYGrpID.ToString()},
                                new TableInfo{Field_ColumName = "Providers_ProvID", NewValue = item.Providers_ProvID.ToString()}
                            };

                            repository.AddAuditLogs(auditLog);
                        }

                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                    }
                    catch (Exception)
                    {
                       
                        dbTransaction.Rollback();

                        ViewBag.Doc = "Something failed. Please try again!";

                        ViewBag.Facitity_DBs_IDPK = toStore.MasterPOSID;

                        var phyGroups = db.PHYGroups.Where(x => !x.MasterPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
                        ViewBag.PHYGrpID = new SelectList(phyGroups, "PHYGrpID", "PHYGroupName");

                        var providersSelected1 = from doc in Physicians
                                                 join prov in db.Providers on doc equals prov.ProvID
                                                 select new VMProvider { ProvID = prov.ProvID, ProviderName = prov.ProviderName, NPI_Num = prov.NPI_Num };

                        toStore.Physicians = providersSelected1.ToList();

                        ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
                        {
                            ProvID = x.ProvID,
                            ProviderName = x.ProviderName,
                            NPI_Num = x.NPI_Num
                        });

                        return View(toStore);
                    }
                }
            }
            if (Physicians == null)
            {
                ViewBag.Doc = "You have to select at least one PHYSICIAN. Please try again!";
            }
            else
            {
                var providersSelected = from doc in Physicians
                                        join prov in db.Providers on doc equals prov.ProvID
                                        select new VMProvider { ProvID = prov.ProvID, ProviderName = prov.ProviderName, NPI_Num = prov.NPI_Num };

                toStore.Physicians = providersSelected.ToList();
            }

            if (db.MasterPOS.Find(toStore.MasterPOSID) == null)
            {
                ViewBag.Doc = "This POS is not stored in DB. Please try again!";
            }
            ViewBag.Facitity_DBs_IDPK = toStore.MasterPOSID;

           
            var groups = db.PHYGroups.Where(x => !x.MasterPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
            ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");



            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
            {
                ProvID = x.ProvID,
                ProviderName = x.ProviderName,
                NPI_Num = x.NPI_Num
            });

            return View(toStore);
        }

        //// GET: PlaceOfServices/PHYGroups/Edit/5
        //public async Task<ActionResult> Edit(/*int? id,*/ int? locationPOSID)
        //{
        //    if (locationPOSID == null)
        //    {
        //        return RedirectToAction("Index", "Error", new { area = "Error" });
        //    }
        //    LocationsPOS currrentPos = await db.LocationsPOS.FindAsync(locationPOSID);
        //    if (currrentPos == null)
        //    {
        //        return RedirectToAction("Index", "Error", new { area = "Error" });
        //    }
        //    if (currrentPos.FvPList.FvPName == "FAC")
        //    {
        //        TempData["Error"] = "You can not assign a Physician Group to a Facility. Please try again!";
        //        return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
        //    }
        //    PHYGroup pHYGroup = currrentPos.PHYGroup;
        //    if (pHYGroup == null)
        //    {
        //        return RedirectToAction("Index", "Error", new { area = "Error" });
        //    }
        //    var toView = new VMPHYGrp
        //    {
        //        PHYGrpID = pHYGroup.PHYGrpID,
        //        PHYGroupName = pHYGroup.PHYGroupName,
        //        PHYGrpNPI_Num = pHYGroup.PHYGrpNPI_Num,
        //        Facitity_DBs_IDPK = locationPOSID.Value
        //    };

        //    var currentProviders = from doc in pHYGroup.ProvidersInGrps
        //                           join prov in db.Providers on doc.Providers_ProvID equals prov.ProvID
        //                           select new VMProvider
        //                           {
        //                               ProvID = prov.ProvID,
        //                               ProviderName = prov.ProviderName,
        //                               NPI_Num = prov.NPI_Num
        //                           };

        //    toView.Physicians = currentProviders.ToList();

        //    /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
        //     pertenecer a un POS*/
        //    var groups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
        //    ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");

        //    ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
        //    {
        //        ProvID = x.ProvID,
        //        ProviderName = x.ProviderName,
        //        NPI_Num = x.NPI_Num
        //    });

        //    return View(toView);
        //}

        //// POST: PlaceOfServices/PHYGroups/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Facitity_DBs_IDPK,PHYGrpID,PHYGroupName,PHYGrpNPI_Num")] VMPHYGrp toStore, params int[] Physicians)
        //{
        //    if (ModelState.IsValid && Physicians != null && await db.LocationsPOS.FindAsync(toStore.Facitity_DBs_IDPK) != null)
        //    {
        //        try
        //        {
        //            if (await db.PHYGroups.AnyAsync(grp => (grp.PHYGroupName.Equals(toStore.PHYGroupName, StringComparison.InvariantCultureIgnoreCase)
        //                            || grp.PHYGrpNPI_Num.Equals(toStore.PHYGrpNPI_Num, StringComparison.InvariantCultureIgnoreCase)) && grp.PHYGrpID != toStore.PHYGrpID))
        //            {
        //                ViewBag.Doc = "Duplicate data. Please try again!";

        //                /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
        //                pertenecer a un POS*/
        //                var phyGroups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
        //                ViewBag.PHYGrpID = new SelectList(phyGroups, "PHYGrpID", "PHYGroupName");

        //                var providersSelected1 = from doc in Physicians
        //                                         join prov in db.Providers on doc equals prov.ProvID
        //                                         select new VMProvider
        //                                         {
        //                                             ProvID = prov.ProvID,
        //                                             ProviderName = prov.ProviderName,
        //                                             NPI_Num = prov.NPI_Num
        //                                         };

        //                toStore.Physicians = providersSelected1.ToList();

        //                ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
        //                {
        //                    ProvID = x.ProvID,
        //                    ProviderName = x.ProviderName,
        //                    NPI_Num = x.NPI_Num
        //                });

        //                return View(toStore);
        //            }

        //            var storedInDb = await db.PHYGroups.FindAsync(toStore.PHYGrpID);

        //            List<TableInfo> tableColumnInfos = new List<TableInfo>();

        //            var currentPhysicians = storedInDb.ProvidersInGrps.Select(x => x.Providers_ProvID).ToArray();

        //            if (!currentPhysicians.Equals(Physicians))
        //            {
        //                var physicianToStore = Physicians.Except(currentPhysicians).ToList();

        //                foreach (var item in physicianToStore)
        //                {
        //                    storedInDb.ProvidersInGrps.Add(new ProvidersInGrp { Providers_ProvID = item, PHYGroups_PHYGrpID = storedInDb.PHYGrpID });
        //                }

        //                var physicianToDelete = currentPhysicians.Except(Physicians).ToList();

        //                foreach (var provId in physicianToDelete)
        //                {
        //                    var proInGrpToDelete = await db.ProvidersInGrps.FirstOrDefaultAsync(x => x.Providers_ProvID == provId && x.PHYGroups_PHYGrpID == storedInDb.PHYGrpID);
        //                    if (proInGrpToDelete != null)
        //                    {
        //                        storedInDb.ProvidersInGrps.Remove(proInGrpToDelete);
        //                    }
        //                }

        //                var oldValue = string.Join(",", currentPhysicians); //C# convert array of integers to comma-separated string
        //                var newValue = string.Join(",", storedInDb.ProvidersInGrps.Select(x => x.Providers_ProvID).ToArray()); //C# convert array of integers to comma-separated string
        //                tableColumnInfos.Add(new TableInfo { Field_ColumName = "Providers", OldValue = oldValue, NewValue = newValue });
        //            }
        //            if (storedInDb.PHYGroupName != toStore.PHYGroupName)
        //            {
        //                tableColumnInfos.Add(new TableInfo { Field_ColumName = "PHYGroupName", OldValue = storedInDb.PHYGroupName, NewValue = toStore.PHYGroupName });
        //                storedInDb.PHYGroupName = toStore.PHYGroupName;
        //            }
        //            if (storedInDb.PHYGrpNPI_Num != toStore.PHYGrpNPI_Num)
        //            {
        //                tableColumnInfos.Add(new TableInfo { Field_ColumName = "PHYGrpNPI_Num", OldValue = storedInDb.PHYGrpNPI_Num, NewValue = toStore.PHYGrpNPI_Num });
        //                storedInDb.PHYGrpNPI_Num = toStore.PHYGrpNPI_Num;
        //            }

        //            db.PHYGroups.Attach(storedInDb);
        //            db.Entry(storedInDb).State = EntityState.Modified;
        //            await db.SaveChangesAsync();

        //            AuditToStore auditLog = new AuditToStore
        //            {
        //                UserLogons = User.Identity.GetUserName(),
        //                AuditDateTime = DateTime.Now,
        //                TableName = "PHYGroups",
        //                AuditAction = "U",
        //                ModelPKey = toStore.PHYGrpID,
        //                tableInfos = tableColumnInfos
        //            };

        //            new AuditLogRepository().AddAuditLogs(auditLog);

        //            return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
        //        }
        //        catch (Exception)
        //        {
        //            ViewBag.Doc = "Something failed. Please try again!";

        //            var phyGroups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
        //            ViewBag.PHYGrpID = new SelectList(phyGroups, "PHYGrpID", "PHYGroupName");

        //            var providersSelected1 = from doc in Physicians
        //                                     join prov in db.Providers on doc equals prov.ProvID
        //                                     select new VMProvider { ProvID = prov.ProvID, ProviderName = prov.ProviderName, NPI_Num = prov.NPI_Num };

        //            toStore.Physicians = providersSelected1.ToList();

        //            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
        //            {
        //                ProvID = x.ProvID,
        //                ProviderName = x.ProviderName,
        //                NPI_Num = x.NPI_Num
        //            });

        //            return View(toStore);
        //        }
        //    }
        //    if (Physicians == null)
        //    {
        //        ViewBag.Doc = "You have to select at least one PHYSICIAN. Please try again!";
        //    }
        //    else
        //    {
        //        var providersSelected = from doc in Physicians
        //                                join prov in db.Providers on doc equals prov.ProvID
        //                                select new VMProvider { ProvID = prov.ProvID, ProviderName = prov.ProviderName, NPI_Num = prov.NPI_Num };

        //        toStore.Physicians = providersSelected.ToList();
        //    }

        //    if (db.LocationsPOS.Find(toStore.Facitity_DBs_IDPK) == null)
        //    {
        //        ViewBag.Doc = "This POS is not stored in DB. Please try again!";
        //    }
        //    /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
        //     pertenecer a un POS*/
        //    var groups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
        //    ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");

        //    ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
        //    {
        //        ProvID = x.ProvID,
        //        ProviderName = x.ProviderName,
        //        NPI_Num = x.NPI_Num
        //    });

        //    return View(toStore);
        //}



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
