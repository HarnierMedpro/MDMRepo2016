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
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.AudiTrails.Controllers
{
    [SetPermissions]
    public class AuditLogsController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: AudiTrails/AuditLogs
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.AuditLogs.ToListAsync());
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Audit([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.AuditLogs.OrderBy(x => x.AuditDateTime).ToDataSourceResult(request, x => new VMAuditLog
            {
                AuditLogID = x.AuditLogID,
                TableName = x.TableName,
                UserLogons = x.UserLogons,
                AuditDateTime = x.AuditDateTime,
                Field_ColumName = x.Field_ColumName,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                AuditAction = x.AuditAction,
                ModelPKey = x.ModelPKey
            }), JsonRequestBehavior.AllowGet);
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
