using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Hubs;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class CorporateMasterListsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       
        public ActionResult Index()
        {
            ViewData["DBs"] = db.DBLists.Select(x => new { x.DB_ID, x.DB }).ToList();
            return View();
        }

        public ActionResult Read_Corporation([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.CorporateMasterLists.Include(x => x.Corp_DBs).Include(x => x.Corp_Owner).OrderBy(x => x.CorporateName).ToList();
            var ownersContacts = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                                   .Where(ty => ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                                   .Select(tv => tv.Contact);
            return Json(result.ToDataSourceResult(request, x => new VMCorporateMasterList
            {
                corpID = x.corpID,
                CorporateName = x.CorporateName,
                TaxID = x.TaxID,
               
                active = x.active,
                DBs = x.Corp_DBs.Select(y => new VMDBList{DB_ID = y.DB_ID, databaseName = y.DBList.databaseName, DB = y.DBList.DB, active = y.DBList.active}).ToList(),
                Owners = x.Corp_Owner.Select(y => y.Contact).Intersect(ownersContacts).Select(z => new VMContact{ContactID = z.ContactID, FName = z.FName, LName = z.LName, active = z.active, Email = z.Email, PhoneNumber = z.PhoneNumber})
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_CorpContacts([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            /*Obtener todos los contactos a nivel de corporacion excepto los de tipo Owner*/
            var allCorpContacts =
                db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                                      .Where(ty => ty.ContactType.ContactLevel.Equals("corporation", StringComparison.InvariantCultureIgnoreCase) &&
                                             ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase) == false)
                                             .Select(tv => tv.Contact).ToList();
            var allCorpOwners = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                                      .Where(ty => ty.ContactType.ContactLevel.Equals("corporation", StringComparison.InvariantCultureIgnoreCase) &&
                                             ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                                             .Select(tv => tv.Contact).ToList();

            var allCorpCntExceptOwners = allCorpContacts.Except(allCorpOwners).ToList();

            var contactsOfThisCorp = db.Corp_Owner.Include(corp => corp.CorporateMasterList).Include(cnt => cnt.Contact).Where(x => x.corpID == corpID).Select(cnt => cnt.Contact).ToList();

            var result = allCorpContacts.Intersect(contactsOfThisCorp).ToList();
            
            return Json(result.ToDataSourceResult(request, x => new VMCorpContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active,
                ContactTypes = x.ContactType_Contact.Select(y =>
                           new VMContactType
                           {
                               ContactTypeID = y.ContactType_ContactTypeID,
                               ContactType_Name = y.ContactType.ContactType_Name,
                               ContactLevel = y.ContactType.ContactLevel
                           })
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_CorpOwners([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            var corpContacts = db.Corp_Owner.Include(corp => corp.CorporateMasterList).Include(cnt => cnt.Contact).Where(x => x.corpID == corpID).Select(cnt => cnt.Contact);
            var ownersContacts = db.ContactType_Contact.Include(cnt => cnt.Contact).Include(ty => ty.ContactType)
                                   .Where(ty => ty.ContactType.ContactType_Name.Equals("owner", StringComparison.InvariantCultureIgnoreCase))
                                   .Select(tv => tv.Contact);
           var result1 = ownersContacts.Intersect(corpContacts).ToList();

            var result = from owner in ownersContacts
                join corp in corpContacts on owner.ContactID equals corp.ContactID
                select new VMContact
                {
                    ContactID = corp.ContactID,
                    FName = corp.FName,
                    LName = corp.LName,
                    Email = corp.Email,
                    PhoneNumber = corp.PhoneNumber,
                    active = corp.active
                };

            var result2 = result1.ToList();

            if (result1.Any() || result2.Any())
            {
                
            }

            return Json(result.ToDataSourceResult(request, x => new VMContact
            {
                ContactID = x.ContactID,
                FName = x.FName,
                LName = x.LName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                active = x.active
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_MasterPOSOfThisCorp([DataSourceRequest] DataSourceRequest request, int? corpID)
        {
            List<MasterPOS> posOfThisCorp = new List<MasterPOS>();
            if (corpID != null)
            {
                var dbsOfThisCorp = db.Corp_DBs.Include(d => d.DBList).Include(c => c.CorporateMasterList)
                                               .Where(c => c.corpID == corpID).Select(d => d.DBList).ToList();
                foreach (var item in dbsOfThisCorp)
                {
                    posOfThisCorp.AddRange(item.MasterPOS);
                }
            }
            return Json(posOfThisCorp.OrderBy(p => p.PosMasterName).ToDataSourceResult(request, x => new VMMasterPOS
            {
                MasterPOSID = x.MasterPOSID,
                PosMasterName = x.PosMasterName,
                active = x.active
            }));
        }


        public async Task<ActionResult> Create_Corporation([DataSourceRequest]DataSourceRequest request,
            [Bind(Include = "corpID,CorporateName,TaxID,active,Owners, DBs")] VMCorporateMasterList corpMasterList)
        {
            if (ModelState.IsValid)
            {
                if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.Equals(corpMasterList.CorporateName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                }

                var toStore = new CorporateMasterList
                {
                    CorporateName = corpMasterList.CorporateName,
                    active = corpMasterList.active,
                    TaxID = corpMasterList.TaxID
                };

                if (corpMasterList.Owners.Any() || corpMasterList.DBs.Any())
                {
                    using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.CorporateMasterLists.Add(toStore);
                            await db.SaveChangesAsync();
                            corpMasterList.corpID = toStore.corpID;

                            List<AuditToStore> auditLogs = new List<AuditToStore>
                            {
                                new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    AuditAction = "I",
                                    TableName = "CorporateMasterLists",
                                    ModelPKey = toStore.corpID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "CorporateName", NewValue = toStore.CorporateName},
                                        new TableInfo{Field_ColumName = "TaxID", NewValue = toStore.TaxID},
                                        new TableInfo{Field_ColumName = "active", NewValue = toStore.active.ToString()}
                                    }
                                }
                            };

                            //if (corpMasterList.Owners.Any())
                            var corpOwner = corpMasterList.Owners.Select(x => new Corp_Owner{corpID = toStore.corpID, Contact_ContactID = x.ContactID}).ToList();
                            db.Corp_Owner.AddRange(corpOwner);

                            /*Aqui tengo que verificar que ninguna de las bases de datos seleccionadas ya esten asociadas a una corporacion, si sucede este escenario,
                             hay que notificarle al usuario cual(es) ya estan asociadas y almacenar las restantes si existen*/
                            var corpDb = new List<Corp_DBs>();
                            string error = "";

                            foreach (var database in corpMasterList.DBs)
                            {
                                if (!await db.Corp_DBs.AnyAsync(x => x.DB_ID == database.DB_ID))
                                {
                                    corpDb.Add(new Corp_DBs { corpID = toStore.corpID, DB_ID = database.DB_ID });
                                }
                                else
                                {
                                    error = error + " " + database.DB;
                                }
                            }
                            db.Corp_DBs.AddRange(corpDb);

                            if (error != "")
                            {
                                CorporateMasterListHub.DoIfDBDuplicated(error + " " + "is/are already asociated with a Corporation.");
                            }

                            await db.SaveChangesAsync();

                            if (corpOwner.Any())
                            {
                                var corpOwnerLog = corpOwner.Select(x => new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "Corp_Owner",
                                    AuditAction = "I",
                                    ModelPKey = x.corpOwnerID,
                                    tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()},
                                    new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = x.Contact_ContactID.ToString()},
                                }
                                }).ToList();

                                auditLogs.AddRange(corpOwnerLog);
                            }

                            if (corpDb.Any())
                            {
                                var corpDbLog = corpDb.Select(x => new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "Corp_DBs",
                                    AuditAction = "I",
                                    ModelPKey = x.ID_PK,
                                    tableInfos = new List<TableInfo>
                                {
                                    new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()},
                                    new TableInfo{Field_ColumName = "DB_ID", NewValue = x.DB_ID.ToString()},
                                }
                                }).ToList();

                                auditLogs.AddRange(corpDbLog);
                            }

                            new AuditLogRepository().SaveLogs(auditLogs);

                            dbTransaction.Commit();

                        }
                        catch (Exception)
                        {
                            dbTransaction.Rollback();
                            ModelState.AddModelError("", "Something failed. Please try again!");
                            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                else
                {
                    try
                    {
                        db.CorporateMasterLists.Add(toStore);
                        await db.SaveChangesAsync();
                        corpMasterList.corpID = toStore.corpID;

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "CorporateMasterLists",
                            ModelPKey = toStore.corpID,
                            AuditAction = "I",
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "CorporateName", NewValue = toStore.CorporateName},
                                new TableInfo{Field_ColumName = "TaxID", NewValue = toStore.TaxID},
                                new TableInfo{Field_ColumName = "active", NewValue = toStore.active.ToString()},
                            }
                        };

                        new AuditLogRepository().AddAuditLogs(auditLog);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
        }

        /*This method is riched only when one or more columns of the entity is modified.*/
        public async Task<ActionResult> Update_Corporation([DataSourceRequest]DataSourceRequest request,
           [Bind(Include = "corpID,CorporateName,TaxID,active,Owners, DBs")] VMCorporateMasterList corpMasterList)
        {
            if (ModelState.IsValid)
            {
                if (await db.CorporateMasterLists.AnyAsync(x => x.CorporateName.Equals(corpMasterList.CorporateName, StringComparison.CurrentCultureIgnoreCase) && x.corpID != corpMasterList.corpID))
                {
                    ModelState.AddModelError("", "Duplicate Data. Please try again!");
                    return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var storedInDB = await db.CorporateMasterLists.FindAsync(corpMasterList.corpID);
                        List<AuditToStore> auditLogs = new List<AuditToStore>();

                        var currentOwners = storedInDB.Corp_Owner.Select(x => x.Contact_ContactID).ToList();
                        var ownersByParam = corpMasterList.Owners.Select(x => x.ContactID).ToList();

                        var currentDbs = storedInDB.Corp_DBs.Select(x => x.DB_ID).ToList();
                        var dbByParam = corpMasterList.DBs.Select(x => x.DB_ID).ToList();


                        var newOwnerIDs = ownersByParam.Except(currentOwners).ToList();
                        var newCorpOwner = newOwnerIDs.Select(x => new Corp_Owner
                        {
                            Contact_ContactID = x,
                            corpID = storedInDB.corpID
                        }).ToList();
                        if (newCorpOwner.Any())
                        {
                            db.Corp_Owner.AddRange(newCorpOwner);
                        }
                        
                        var deleteOwners = currentOwners.Except(ownersByParam).ToList();
                        foreach (var ow in deleteOwners)
                        {
                            var corpOwner = await db.Corp_Owner.FirstOrDefaultAsync(x => x.Contact_ContactID == ow && x.corpID == storedInDB.corpID);
                            if (corpOwner != null)
                            {
                                db.Corp_Owner.Remove(corpOwner);

                                auditLogs.Add(new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "Corp_Owner",
                                    AuditAction = "D",
                                    ModelPKey = corpOwner.corpOwnerID,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "Contact_ContactID", OldValue = corpOwner.Contact_ContactID.ToString()},
                                        new TableInfo{Field_ColumName = "corpID", OldValue = corpOwner.corpID.ToString()},
                                    }
                                });
                            }
                        }

                        var newDbs = dbByParam.Except(currentDbs).ToList();
                        var newCorpDbs = new List<Corp_DBs>();
                        string error = "";
                        foreach (var datab in newDbs)
                        {
                            if (!await db.Corp_DBs.AnyAsync(x => x.DB_ID == datab))
                            {
                                newCorpDbs.Add(new Corp_DBs { corpID = storedInDB.corpID, DB_ID = datab });
                            }
                            else
                            {
                                error = error + " " + datab;
                            }
                        }
                        if (newCorpDbs.Any())
                        {
                            db.Corp_DBs.AddRange(newCorpDbs);
                        }
                       
                        if (error != "")
                        {
                            CorporateMasterListHub.DoIfDBDuplicated(error + " " + "is/are asociated with other Corporation.");
                        }

                        var deleteDbs = currentDbs.Except(dbByParam).ToList();
                        foreach (var datb in deleteDbs)
                        {
                            var corpDb = await db.Corp_DBs.FirstOrDefaultAsync(x => x.DB_ID == datb && x.corpID == storedInDB.corpID);
                            if (corpDb != null)
                            {
                                db.Corp_DBs.Remove(corpDb);

                                auditLogs.Add(new AuditToStore
                                {
                                    UserLogons = User.Identity.GetUserName(),
                                    AuditDateTime = DateTime.Now,
                                    TableName = "Corp_DBs",
                                    AuditAction = "D",
                                    ModelPKey = corpDb.ID_PK,
                                    tableInfos = new List<TableInfo>
                                    {
                                        new TableInfo{Field_ColumName = "DB_ID", OldValue = corpDb.DB_ID.ToString()},
                                        new TableInfo{Field_ColumName = "corpID", OldValue = corpDb.corpID.ToString()},
                                    }
                                });
                            }
                        }


                        var tableColumnInfo = new List<TableInfo>();
                        if (storedInDB.CorporateName != corpMasterList.CorporateName)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "CorporateName", OldValue = storedInDB.CorporateName, NewValue = corpMasterList.CorporateName });
                            storedInDB.CorporateName = corpMasterList.CorporateName;
                        }
                        if (storedInDB.active != corpMasterList.active)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "active", OldValue = storedInDB.active.ToString(), NewValue = corpMasterList.active.ToString() });
                            storedInDB.active = corpMasterList.active;
                        }
                        if (storedInDB.TaxID != corpMasterList.TaxID)
                        {
                            tableColumnInfo.Add(new TableInfo { Field_ColumName = "TaxID", OldValue = storedInDB.TaxID, NewValue = corpMasterList.TaxID });
                            storedInDB.TaxID = corpMasterList.TaxID;
                        }
                       
                        db.CorporateMasterLists.Attach(storedInDB);
                        db.Entry(storedInDB).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        if (newCorpOwner.Any())
                        {
                            var corpOwnerLog = newCorpOwner.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "Corp_Owner",
                                AuditAction = "I",
                                ModelPKey = x.corpOwnerID,
                                tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "Contact_ContactID", NewValue = x.Contact_ContactID.ToString()},
                                new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()},
                            }
                            }).ToList();
                            auditLogs.AddRange(corpOwnerLog);
                        }

                        if (newCorpDbs.Any())
                        {
                            var corpDbLog = newCorpDbs.Select(x => new AuditToStore
                            {
                                UserLogons = User.Identity.GetUserName(),
                                AuditDateTime = DateTime.Now,
                                TableName = "Corp_DBs",
                                AuditAction = "I",
                                ModelPKey = x.ID_PK,
                                tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "DB_ID", NewValue = x.DB_ID.ToString()},
                                new TableInfo{Field_ColumName = "corpID", NewValue = x.corpID.ToString()}
                            }
                            }).ToList();
                            auditLogs.AddRange(corpDbLog);
                        }

                        if (tableColumnInfo.Any())
                        {
                            AuditToStore auditLog = new AuditToStore
                            {
                                AuditAction = "U",
                                AuditDateTime = DateTime.Now,
                                ModelPKey = storedInDB.corpID,
                                TableName = "CorporateMasterList",
                                UserLogons = User.Identity.GetUserName(),
                                tableInfos = tableColumnInfo
                            };
                            auditLogs.Add(auditLog);
                        }

                        new AuditLogRepository().SaveLogs(auditLogs);

                        dbTransaction.Commit();
                       
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { corpMasterList }.ToDataSourceResult(request, ModelState));
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
