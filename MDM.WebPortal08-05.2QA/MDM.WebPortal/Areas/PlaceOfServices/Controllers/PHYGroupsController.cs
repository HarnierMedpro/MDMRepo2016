using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class PHYGroupsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/PHYGroups
        public ActionResult Index()
        {
            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
            {
                ProvID = x.ProvID,
                ProviderName = x.ProviderName,
                NPI_Num = x.NPI_Num
            });

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
            [Bind(Include = "Facitity_DBs_IDPK,PHYGrpID,PHYGroupName,PHYGrpNPI_Num,Physicians")] VMPHYGrp toStore)
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
                                        || grp.PHYGrpNPI_Num.Equals(toStore.PHYGrpNPI_Num, StringComparison.InvariantCultureIgnoreCase) ) && grp.PHYGrpID != toStore.PHYGrpID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.PHYGroups.FindAsync(toStore.PHYGrpID);

                    var currentProviders = from pInG in storedInDb.ProvidersInGrps
                        join prov in db.Providers on pInG.Providers_ProvID equals prov.ProvID
                        select new VMProvider
                        {
                            ProvID = prov.ProvID,
                            ProviderName = prov.ProviderName,
                            NPI_Num = prov.NPI_Num
                        };
                    List<int> ent2 = new List<int> { 1, 2 };
                    List<int> ent1 = new List<int>{1,2,3};

                    var resutl = ent2.Except(ent1).ToList();

                    if (resutl.Any())
                    {
                        
                    }
                    var excepto = currentProviders.ToList();
                    var newToStore = toStore.Physicians.ToList().Except(excepto).ToList();

                    foreach (var store in newToStore)
                    {
                        
                    }

                    //var newProvidersInGroup = toStore.Physicians.Select(x => new VMProvidersInGrp {Providers_ProvID = x.ProvID, PHYGroups_PHYGrpID = storedInDb.PHYGrpID}).ToList();

                    //var currentProvidersInGroup = storedInDb.ProvidersInGrps.Select(ob => new VMProvidersInGrp { Providers_ProvID = ob.Providers_ProvID, PHYGroups_PHYGrpID = ob.PHYGroups_PHYGrpID.Value }).ToList();

                    //var providerInGroupToStore = newProvidersInGroup.Except(currentProvidersInGroup).ToList();

                    //foreach (var store in providerInGroupToStore)
                    //{
                    //    storedInDb.ProvidersInGrps.Add(new ProvidersInGrp{Providers_ProvID = store.Providers_ProvID, PHYGroups_PHYGrpID = store.PHYGroups_PHYGrpID});
                    //}

                    //var providerInGroupToDelete = currentProvidersInGroup.Except(newProvidersInGroup).ToList();

                    //foreach (var delete in providerInGroupToDelete)
                    //{
                    //    storedInDb.ProvidersInGrps.Remove(storedInDb.ProvidersInGrps.First(x => x.Providers_ProvID == delete.Providers_ProvID && x.PHYGroups_PHYGrpID == delete.PHYGroups_PHYGrpID));
                    //}

                    storedInDb.PHYGroupName = toStore.PHYGroupName;
                    storedInDb.PHYGrpNPI_Num = toStore.PHYGrpNPI_Num;

                    db.PHYGroups.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { toStore }.ToDataSourceResult(request, ModelState));
        }

        // GET: PlaceOfServices/PHYGroups/Details/5
        public async Task<ActionResult> Details(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new {area = "Error"});
            }
            LocationsPOS pos = await db.LocationsPOS.FindAsync(locationPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            PHYGroup pHYGroup = pos.PHYGroup;
            VMPHYGrp toView = new VMPHYGrp();
            if (pHYGroup == null)
            {
                //pHYGroup = new PHYGroup();
                toView.Facitity_DBs_IDPK = locationPOSID.Value;
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

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseGrp([Bind(Include = "PHYGrpID,Facitity_DBs_IDPK")] VMChooseGrp toAssign)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var locPos = await db.LocationsPOS.FindAsync(toAssign.Facitity_DBs_IDPK);
                    var currentGrp = locPos.PHYGroups_PHYGrpID;
                    var phyGroup = await db.PHYGroups.FindAsync(toAssign.PHYGrpID);
                    if (phyGroup.LocationsPOS.Any())
                    {
                        TempData["Error"] = "This group is asociated with other POS. Please try again!";
                        return RedirectToAction("Create", new { locationPOSID = toAssign.Facitity_DBs_IDPK });
                    }

                    locPos.PHYGroup = phyGroup;
                    db.LocationsPOS.Attach(locPos);
                    db.Entry(locPos).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "U",
                        ModelPKey = locPos.Facitity_DBs_IDPK,
                        TableName = "LocationsPOS",
                        tableInfos = new List<TableInfo> { new TableInfo { Field_ColumName = "PHYGroups_PHYGrpID", OldValue = currentGrp.ToString(), NewValue = phyGroup.PHYGrpID.ToString()} }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                    return RedirectToAction("Index","LocationsPOS",new{area="PlaceOfServices"});
                }
                catch (Exception)
                {
                    TempData["Error"] = "Something failed. Please try again!";
                    return RedirectToAction("Create", new {locationPOSID = toAssign.Facitity_DBs_IDPK});
                }
            }
            TempData["Error"] = "Something failed. Please try again!";
            return RedirectToAction("Create", new { locationPOSID = toAssign.Facitity_DBs_IDPK });
        }


        


        // GET: PlaceOfServices/PHYGroups/Create
        public async Task<ActionResult> Create(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            LocationsPOS pos = await db.LocationsPOS.FindAsync(locationPOSID);
            if (pos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }

           // ViewBag.GroupCount = db.PHYGroups.Count();

            ViewBag.Facitity_DBs_IDPK = locationPOSID;
            
            /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
             pertenecer a un POS*/
            var groups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
            ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");

            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
            {
                ProvID = x.ProvID,
                ProviderName = x.ProviderName,
                NPI_Num = x.NPI_Num
            });

            return View();
        }

        // POST: PlaceOfServices/PHYGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Facitity_DBs_IDPK,PHYGrpID,PHYGroupName,PHYGrpNPI_Num,Physicians")] VMPHYGrp toStore, params int[] Physicians)
        {
            if (ModelState.IsValid && Physicians != null && await db.LocationsPOS.FindAsync(toStore.Facitity_DBs_IDPK) != null)
            {
                
                    if (await db.PHYGroups.AnyAsync( grp => grp.PHYGroupName.Equals(toStore.PHYGroupName, StringComparison.InvariantCultureIgnoreCase)
                                    || grp.PHYGrpNPI_Num.Equals(toStore.PHYGrpNPI_Num, StringComparison.InvariantCultureIgnoreCase))
                        )
                    {
                        ViewBag.Doc = "Duplicate data. Please try again!";

                        ViewBag.Facitity_DBs_IDPK = toStore.Facitity_DBs_IDPK;

                        /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
                        pertenecer a un POS*/
                        var phyGroups =
                            db.PHYGroups.Where(x => !x.LocationsPOS.Any())
                                .OrderBy(x => x.PHYGroupName)
                                .Select(x => new {x.PHYGrpID, x.PHYGroupName});
                        ViewBag.PHYGrpID = new SelectList(phyGroups, "PHYGrpID", "PHYGroupName");

                        var providersSelected1 = from doc in Physicians
                            join prov in db.Providers on doc equals prov.ProvID
                            select new VMProvider
                                {
                                    ProvID = prov.ProvID,
                                    ProviderName = prov.ProviderName,
                                    NPI_Num = prov.NPI_Num
                                };

                        toStore.Physicians = providersSelected1.ToList();

                        ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
                        {
                            ProvID = x.ProvID,
                            ProviderName = x.ProviderName,
                            NPI_Num = x.NPI_Num
                        });

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
                            LocationsPOS = new List<LocationsPOS> { await db.LocationsPOS.FindAsync(toStore.Facitity_DBs_IDPK) }
                        };
                        db.PHYGroups.Add(newGrp);
                        await db.SaveChangesAsync();

                        var providersInGrp = Physicians.Select(doc => new ProvidersInGrp { PHYGroups_PHYGrpID = newGrp.PHYGrpID, Providers_ProvID = doc }).ToList();

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
                            ModelPKey = toStore.Facitity_DBs_IDPK,
                            TableName = "LocationsPOS",
                            AuditAction = "U",
                            tableInfos = new List<TableInfo> { new TableInfo{Field_ColumName = "PHYGroups_PHYGrpID", NewValue = newGrp.PHYGrpID.ToString()}}
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

                        return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
                    }
                    catch (Exception)
                    {
                        //Rollback transaction if exception occurs
                        dbTransaction.Rollback();

                        ViewBag.Doc = "Something failed. Please try again!";

                        ViewBag.Facitity_DBs_IDPK = toStore.Facitity_DBs_IDPK;

                        var phyGroups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
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
            
            if (db.LocationsPOS.Find(toStore.Facitity_DBs_IDPK) == null)
            {
                ViewBag.Doc = "This POS is not stored in DB. Please try again!";
            }

            //ViewBag.GroupCount = db.PHYGroups.Count();

            ViewBag.Facitity_DBs_IDPK = toStore.Facitity_DBs_IDPK;

            //var groups = db.PHYGroups.OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
            //ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");

            /*Dame solo aquellos grupos de doctores que no esten asociados a ningun POS; de esta manera se garantiza que un grupo solo puede
             pertenecer a un POS*/
            var groups = db.PHYGroups.Where(x => !x.LocationsPOS.Any()).OrderBy(x => x.PHYGroupName).Select(x => new { x.PHYGrpID, x.PHYGroupName });
            ViewBag.PHYGrpID = new SelectList(groups, "PHYGrpID", "PHYGroupName");

            

            ViewData["Providers"] = db.Providers.OrderBy(x => x.ProviderName).Select(x => new VMProvider
            {
                ProvID = x.ProvID,
                ProviderName = x.ProviderName,
                NPI_Num = x.NPI_Num
            });

            return View(toStore);
        }

        // GET: PlaceOfServices/PHYGroups/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHYGroup pHYGroup = await db.PHYGroups.FindAsync(id);
            if (pHYGroup == null)
            {
                return HttpNotFound();
            }
            return View(pHYGroup);
        }

        // POST: PlaceOfServices/PHYGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PHYGrpID,PHYGroupName,PHYGrpNPI_Num")] PHYGroup pHYGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pHYGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pHYGroup);
        }

        // GET: PlaceOfServices/PHYGroups/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHYGroup pHYGroup = await db.PHYGroups.FindAsync(id);
            if (pHYGroup == null)
            {
                return HttpNotFound();
            }
            return View(pHYGroup);
        }

        // POST: PlaceOfServices/PHYGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PHYGroup pHYGroup = await db.PHYGroups.FindAsync(id);
            db.PHYGroups.Remove(pHYGroup);
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
