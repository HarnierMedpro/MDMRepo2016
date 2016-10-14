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
    public class ProvidersController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/Providers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Provider([DataSourceRequest] DataSourceRequest request)
        {
            //SortDescriptor sort = new SortDescriptor { Member = "ProviderName", SortDirection = ListSortDirection.Ascending};
            //request.Sorts.Add(sort);
            //OR
            var result = db.Providers.OrderBy(x => x.ProviderName).ToList();
            return Json(result.ToDataSourceResult(request, provider => new VMProvider
            {
                ProvID = provider.ProvID,
                ProviderName = provider.ProviderName,
                NPI_Num = provider.NPI_Num
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Provider([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ProvID,ProviderName,NPI_Num")] VMProvider provider)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Providers.AnyAsync(x => x.NPI_Num.Equals(provider.NPI_Num, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("","Duplicate Data. Please try again!");
                        return Json(new[] {provider}.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new Provider
                    {
                        NPI_Num = provider.NPI_Num,
                        ProviderName = provider.ProviderName
                    };
                    db.Providers.Add(toStore);
                    await db.SaveChangesAsync();
                    provider.ProvID = toStore.ProvID;

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        TableName = "Providers",
                        AuditAction = "I",
                        ModelPKey = toStore.ProvID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "NPI_Num", NewValue = provider.NPI_Num},
                            new TableInfo{Field_ColumName = "ProviderName", NewValue = provider.ProviderName}
                        }
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { provider }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { provider }.ToDataSourceResult(request, ModelState));
        }


        public async Task<ActionResult> Update_Provider([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "ProvID,ProviderName,NPI_Num")] VMProvider provider)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Providers.AnyAsync(x => x.NPI_Num.Equals(provider.NPI_Num, StringComparison.InvariantCultureIgnoreCase) && x.ProvID != provider.ProvID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { provider }.ToDataSourceResult(request, ModelState));
                    }

                    var storedInDb = await db.Providers.FindAsync(provider.ProvID);

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDb.NPI_Num != provider.NPI_Num)
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "NPI_Num", OldValue = storedInDb.NPI_Num, NewValue = provider.NPI_Num});
                        storedInDb.NPI_Num = provider.NPI_Num;
                    }
                    if (!storedInDb.ProviderName.Equals(provider.ProviderName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumnInfos.Add(new TableInfo { Field_ColumName = "ProviderName", OldValue = storedInDb.ProviderName, NewValue = provider.ProviderName});
                        storedInDb.ProviderName = provider.ProviderName;
                    }

                    db.Providers.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        TableName = "Providers",
                        AuditAction = "U",
                        ModelPKey = storedInDb.ProvID,
                        tableInfos = tableColumnInfos
                    };
                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { provider }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { provider }.ToDataSourceResult(request, ModelState));
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
