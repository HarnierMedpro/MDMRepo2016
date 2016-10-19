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
    public class PHYGroupsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       
        public ActionResult Index()
        {
            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider{ProvID = x.ProvID,ProviderName = x.ProviderName,NPI_Num = x.NPI_Num});
            return View();
        }

        /*-----------------------------------KENDO UI FOR ASP.NET MVC 5 FUNCTIONS------------------------------*/
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

        public async Task<ActionResult> Read_POSGrp([DataSourceRequest] DataSourceRequest request, int? MasterPOSID)
        {
            var result = new List<VMPHYGrp>();
            if (MasterPOSID != null && MasterPOSID > 0)
            {
                var pos = await db.MasterPOS.FindAsync(MasterPOSID);
                if (pos != null && pos.PHYGroup != null)
                {
                    var phyG = pos.PHYGroup;
                    result.Add(new VMPHYGrp
                    {
                        MasterPOSID = MasterPOSID.Value,
                        PHYGrpID = phyG.PHYGrpID,
                        PHYGroupName = phyG.PHYGroupName,
                        PHYGrpNPI_Num = phyG.PHYGrpNPI_Num,
                        Physicians = from pInG in phyG.ProvidersInGrps
                                     join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                                     select new VMProvider
                                     {
                                         ProvID = prov.ProvID,
                                         ProviderName = prov.ProviderName,
                                         NPI_Num = prov.NPI_Num
                                     }
                    });
                }
            }
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PHYGrp_Info(int? MasterPOSID)
        {
            if (MasterPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            MasterPOS pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            var phyG = pos.PHYGroup;
            var toView = new List<VMPHYGrp>();
            if (phyG != null)
            {
                var docs = from pInG in phyG.ProvidersInGrps
                    join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                    select new VMProvider
                    {
                        ProvID = prov.ProvID,
                        ProviderName = prov.ProviderName,
                        NPI_Num = prov.NPI_Num
                    };
                toView.Add(new VMPHYGrp
                {
                    MasterPOSID = MasterPOSID.Value,
                    PHYGrpID = phyG.PHYGrpID,
                    PHYGroupName = phyG.PHYGroupName,
                    PHYGrpNPI_Num = phyG.PHYGrpNPI_Num,
                    Physicians = docs.ToList()
                });
            }
            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }

        public async Task<ActionResult> Create_Groups([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MasterPOSID,PHYGrpID,PHYGroupName,PHYGrpNPI_Num,Physicians")] VMPHYGrp toStore, int? ParentID)
        {
            if (ModelState.IsValid)
            {
                if (ParentID == null || ParentID == 0)
                {
                    ModelState.AddModelError("", "Something Failed. Please try again!");
                    return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                }
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
                        toStore.MasterPOSID = ParentID.Value;

                        var pos = await db.MasterPOS.FindAsync(ParentID);
                        pos.PHYGroups_PHYGrpID = toStore.PHYGrpID;
                        db.MasterPOS.Attach(pos);
                        db.Entry(pos).State = EntityState.Modified;
                      

                        var providersInGrp = toStore.Physicians.Select(doc => new ProvidersInGrp { PHYGroups_PHYGrpID = newGrp.PHYGrpID, Providers_ProvID = doc.ProvID }).ToList();
                        db.ProvidersInGrps.AddRange(providersInGrp);

                        //saves all above operations within one transaction
                        await db.SaveChangesAsync();

                        List<AuditToStore> auditLogs = new List<AuditToStore>
                        {
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "PHYGroups",
                                AuditAction = "I",
                                ModelPKey = newGrp.PHYGrpID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "PHYGroupName", NewValue = newGrp.PHYGroupName},
                                    new TableInfo{Field_ColumName = "PHYGrpNPI_Num", NewValue = newGrp.PHYGrpNPI_Num},
                                }
                            },
                            new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "MasterPOS",
                                AuditAction = "U",
                                ModelPKey = pos.MasterPOSID,
                                tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", NewValue = pos.PHYGroups_PHYGrpID.ToString()}
                                }
                            }
                        };
                        auditLogs.AddRange(providersInGrp.Select(x => new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "ProvidersInGrp",
                            ModelPKey = x.ProvInGrpID,
                            AuditAction = "I",
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", NewValue = x.PHYGroups_PHYGrpID.ToString()},
                                new TableInfo{Field_ColumName = "Providers_ProvID", NewValue = x.Providers_ProvID.ToString()},
                            }
                        }));

                        new AuditLogRepository().SaveLogs(auditLogs);

                        //commit transaction
                        dbTransaction.Commit();
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
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
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
                        var auditLogs = new List<AuditToStore>();

                        var physiciansByParam = toStore.Physicians.Select(x => x.ProvID).ToArray();
                        var currentPhysicians = storedInDb.ProvidersInGrps.Select(x => x.Providers_ProvID).ToArray();

                        var physicianIds = physiciansByParam.Except(currentPhysicians).ToList();
                        var physicianToStore = physicianIds.Select(x => new ProvidersInGrp { Providers_ProvID = x, PHYGroups_PHYGrpID = storedInDb.PHYGrpID }).ToList();
                        db.ProvidersInGrps.AddRange(physicianToStore);

                        var physicianToDelete = currentPhysicians.Except(physiciansByParam).ToList();
                        foreach (var provId in physicianToDelete)
                        {
                            var proInGrpToDelete = await db.ProvidersInGrps.FirstOrDefaultAsync(x => x.Providers_ProvID == provId && x.PHYGroups_PHYGrpID == storedInDb.PHYGrpID);
                            if (proInGrpToDelete != null)
                            {
                                db.ProvidersInGrps.Remove(proInGrpToDelete);
                                auditLogs.Add(new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "ProvidersInGrps",
                                    AuditAction = "D",
                                    ModelPKey = proInGrpToDelete.ProvInGrpID,
                                    tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", OldValue = proInGrpToDelete.PHYGroups_PHYGrpID.ToString()},
                                     new TableInfo{Field_ColumName = "Providers_ProvID", OldValue = proInGrpToDelete.Providers_ProvID.ToString()},
                                }
                                });
                            }
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

                        if (tableColumnInfos.Any())
                        {
                            auditLogs.Add(new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "PHYGroupName",
                                AuditAction = "U",
                                ModelPKey = storedInDb.PHYGrpID,
                                tableInfos = tableColumnInfos
                            });
                        }

                        if (physicianToStore.Any())
                        {
                            auditLogs.AddRange(physicianToStore.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                AuditAction = "I",
                                TableName = "ProvidersInGrp",
                                ModelPKey = x.ProvInGrpID,
                                tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", NewValue = x.PHYGroups_PHYGrpID.ToString()},
                                new TableInfo{Field_ColumName = "Providers_ProvID", NewValue = x.Providers_ProvID.ToString()},
                            }
                            }));
                        }

                        new AuditLogRepository().SaveLogs(auditLogs);

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

        [HttpPost]
        public async Task<ActionResult> Assign_PhyGrpToPos([DataSourceRequest] DataSourceRequest request, int? PHYGrpID, int? MasterPOSID)
        {
            if (PHYGrpID == null || MasterPOSID == null)
            {
                return Json(new List<VMPHYGrp>().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var pos = await db.MasterPOS.FindAsync(MasterPOSID);
                    var tableColumnInfo = new List<TableInfo>
                    {
                        new TableInfo
                        {
                            Field_ColumName = "PHYGroups_PHYGrpID",
                            OldValue = pos.PHYGroups_PHYGrpID.ToString(),
                            NewValue = PHYGrpID.ToString()
                        }
                    };
                    pos.PHYGroups_PHYGrpID = PHYGrpID;
                    db.MasterPOS.Attach(pos);
                    db.Entry(pos).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        TableName = "MasterPOS",
                        ModelPKey = pos.MasterPOSID,
                        AuditAction = "U",
                        tableInfos = tableColumnInfo
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return Json(new List<VMPHYGrp>().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
            }
            
            var storedInDb = await db.PHYGroups.FindAsync(PHYGrpID);
            List<VMPHYGrp> toView = new List<VMPHYGrp>
            {
                new VMPHYGrp
                {
                    MasterPOSID = MasterPOSID.Value,
                    PHYGrpID = PHYGrpID.Value,
                    PHYGroupName = storedInDb.PHYGroupName,
                    PHYGrpNPI_Num = storedInDb.PHYGrpNPI_Num,
                    Physicians = from pInG in storedInDb.ProvidersInGrps
                                 join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                                 select new VMProvider
                                    {
                                        ProvID = prov.ProvID,
                                        ProviderName = prov.ProviderName,
                                        NPI_Num = prov.NPI_Num
                                    }
                }
            };
            return Json(toView.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /*-----------------------------------KENDO UI FOR ASP.NET MVC 5 FUNCTIONS------------------------------*/

       
        /*---------------------------------Regular ASP.NET MVC 5---------------------------------------*/
        public async Task<ActionResult> Details(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            MasterPOS pos = await db.MasterPOS.FindAsync(locationPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
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
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            MasterPOS pos = await db.MasterPOS.FindAsync(locationPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
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

        /*---------------------------------Regular ASP.NET MVC 5---------------------------------------*/
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
