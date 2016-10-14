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
    public class Forms_SentController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

       
        public ActionResult Read_FormsOfThisPOS([DataSourceRequest] DataSourceRequest request, int? masterPOSID)
        {
            var result = db.Forms_sent.Include(p => p.MasterPOS).Include(f => f.FormsDict);
            if (masterPOSID != null && masterPOSID > 0)
            {
                result = result.Where(x => x.MasterPOS_MasterPOSID == masterPOSID);
            }
            return Json(result.ToDataSourceResult(request, x => new VMForms_Sent
            {
                MasterPOS_MasterPOSID = x.MasterPOS_MasterPOSID,
                FormsDict_FormsID = x.FormsDict_FormsID,
                FormSentID = x.FormSentID
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_FormsSent([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "FormSentID,FormsDict_FormsID,MasterPOS_MasterPOSID")]VMForms_Sent formsSent, int posName)
        {
            if (ModelState.IsValid)
            {
                if (posName == 0)
                {
                    ModelState.AddModelError("","Invalid POS Name. Please try again!");
                    return Json(new[] {formsSent}.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.Forms_sent.AnyAsync(x => x.FormsDict_FormsID == formsSent.FormsDict_FormsID && x.MasterPOS_MasterPOSID == posName))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { formsSent }.ToDataSourceResult(request, ModelState));
                        }
                        var toStore = new Forms_sent
                        {
                            FormsDict_FormsID = formsSent.FormsDict_FormsID,
                            MasterPOS_MasterPOSID = posName
                        };
                        db.Forms_sent.Add(toStore);
                        await db.SaveChangesAsync();
                        formsSent.FormSentID = toStore.FormSentID;
                        formsSent.MasterPOS_MasterPOSID = posName;

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "Forms_sent",
                            ModelPKey = toStore.FormSentID,
                            AuditAction = "I",
                            tableInfos = new List<TableInfo>
                            {
                                new TableInfo{Field_ColumName = "FormsDict_FormsID",NewValue = formsSent.FormsDict_FormsID.ToString()},
                                new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID",NewValue = posName.ToString()},
                            }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { formsSent }.ToDataSourceResult(request, ModelState));
                    } 
                }
            }
            return Json(new[] { formsSent }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_FormsSent([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "FormSentID,FormsDict_FormsID,MasterPOS_MasterPOSID")]VMForms_Sent formsSent)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (await db.Forms_sent.AnyAsync(x => x.FormsDict_FormsID == formsSent.FormsDict_FormsID && x.MasterPOS_MasterPOSID == formsSent.MasterPOS_MasterPOSID && x.FormSentID != formsSent.FormSentID))
                        {
                            ModelState.AddModelError("", "Duplicate Data. Please try again!");
                            return Json(new[] { formsSent }.ToDataSourceResult(request, ModelState));
                        }
                        var storedInDb = await db.Forms_sent.FindAsync(formsSent.FormSentID);
                        storedInDb.FormsDict_FormsID = formsSent.FormsDict_FormsID;
                        storedInDb.MasterPOS_MasterPOSID = formsSent.MasterPOS_MasterPOSID;

                        db.Forms_sent.Attach(storedInDb);
                        db.Entry(storedInDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        AuditToStore auditLog = new AuditToStore
                        {
                            UserLogons = User.Identity.GetUserName(),
                            AuditDateTime = DateTime.Now,
                            TableName = "Forms_sent",
                            ModelPKey = storedInDb.FormSentID,
                            AuditAction = "U",
                            tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "FormsDict_FormsID", NewValue = formsSent.FormsDict_FormsID.ToString(),OldValue = storedInDb.FormsDict_FormsID.ToString()},
                            new TableInfo{Field_ColumName = "MasterPOS_MasterPOSID", NewValue = formsSent.MasterPOS_MasterPOSID.ToString(), OldValue = storedInDb.MasterPOS_MasterPOSID.ToString()},
                        }
                        };
                        new AuditLogRepository().AddAuditLogs(auditLog);

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("", "Something failed. Please try again!");
                        return Json(new[] { formsSent }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] { formsSent }.ToDataSourceResult(request, ModelState));
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
