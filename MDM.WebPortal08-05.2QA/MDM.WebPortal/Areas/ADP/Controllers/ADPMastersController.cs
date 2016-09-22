using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Models.FromDB;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MDM.WebPortal.Areas.ADP.Models;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Data_Annotations;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.ADP.Controllers
{
    [SetPermissions]
    public class ADPMastersController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Adp([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.ADPMasters.Select(x => new VMAdpMaster
            {
                ADPMaster_ID = x.ADPMasterID,
                ADP = x.ADP_ID,
                Title = x.Title,
                FName = x.FName,
                LName = x.LName,
                Manager = x.Manager,
                Active = x.Active
            }).OrderBy(x => x.ADP);
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /*Get al EdgeMedLogons of specific ADP*/
        public ActionResult EdgeMedLogonsforAdp([DataSourceRequest] DataSourceRequest request, int? ADPMaster_ID)
        {
            IQueryable<Edgemed_Logons> edgemed = db.Edgemed_Logons;

            if (ADPMaster_ID != null)
            {
                edgemed = edgemed.Where(x => x.ADPMasterID == ADPMaster_ID);
            }

            return Json(edgemed.ToDataSourceResult(request, x => new VMEdgemed_Logons
            {
                Edgemed_LogID = x.Edgemed_LogID, //PK from Edgemed table
                Edgemed_UserName = x.Edgemed_UserName,
                Zno = x.Zno,
                EdgeMed_ID = x.EdgeMed_ID,
                Active = x.Active,
                ADPMasterID = x.ADPMasterID //FK from dbo.ADP_Master table
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_Adp([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "ADPMaster_ID,ADP,FName,LName,Title,Manager,Active")] VMAdpMaster aDPMaster)
        {
            if (aDPMaster != null && ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.ADPMasters.FindAsync(aDPMaster.ADPMaster_ID);
                    //var storedInDb = new ADPMaster {ADPMasterID = aDPMaster.ADPMaster_ID};//Avoid call to DB to get the object stored.
                    //storedInDb.ADP_ID = aDPMaster.ADP;
                    //storedInDb.Title = aDPMaster.Title;
                    //storedInDb.Active = aDPMaster.Active;
                    //storedInDb.FName = aDPMaster.FName;
                    //storedInDb.LName = aDPMaster.LName;
                    //storedInDb.Manager = aDPMaster.Manager;

                    List<TableInfo> tableColumnInfos = new List<TableInfo>();

                    if (storedInDb.ADP_ID != aDPMaster.ADP)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "ADP_ID", 
                            NewValue = aDPMaster.ADP, 
                            OldValue = storedInDb.ADP_ID
                        });
                        storedInDb.ADP_ID = aDPMaster.ADP;
                    }
                    if (storedInDb.Title != aDPMaster.Title)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "Title",
                            NewValue = aDPMaster.Title, 
                            OldValue = storedInDb.Title
                        });
                        storedInDb.Title = aDPMaster.Title;
                    }
                    if (storedInDb.Active != aDPMaster.Active)
                    {
                        
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "Active", 
                            NewValue = aDPMaster.Active.ToString(), 
                            OldValue = storedInDb.Active.ToString()
                        });
                        storedInDb.Active = aDPMaster.Active;
                    }
                    if (storedInDb.FName != aDPMaster.FName)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "FName", 
                            NewValue = aDPMaster.FName, 
                            OldValue = storedInDb.FName
                        });
                        storedInDb.FName = aDPMaster.FName;
                    }
                    if (storedInDb.LName != aDPMaster.LName)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "LName", 
                            NewValue = aDPMaster.LName, 
                            OldValue = storedInDb.LName
                        });
                        storedInDb.LName = aDPMaster.LName;
                    }
                    if (storedInDb.Manager != aDPMaster.Manager)
                    {
                        tableColumnInfos.Add(new TableInfo
                        {
                            Field_ColumName = "Manager", 
                            NewValue = aDPMaster.Manager, 
                            OldValue = storedInDb.Manager
                        });
                        storedInDb.Manager = aDPMaster.Manager;
                    }

                    db.ADPMasters.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditAction = "U",
                        tableInfos = tableColumnInfos,
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        ModelPKey = storedInDb.ADPMasterID,
                        TableName = "ADPMaster"
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something Failed. Please try again!");
                    return Json(new[] { aDPMaster }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { aDPMaster }.ToDataSourceResult(request, ModelState));
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
