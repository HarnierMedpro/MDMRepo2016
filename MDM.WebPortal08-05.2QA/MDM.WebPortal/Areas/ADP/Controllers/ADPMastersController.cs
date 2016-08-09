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
using MDM.WebPortal.Data_Annotations;

namespace MDM.WebPortal.Areas.ADP.Controllers
{
    //[SetPermissions]
    public class ADPMastersController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Adp([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.ADPMasters.ToDataSourceResult(request, x => new VMAdpMaster
            {
                ADPMaster_ID = x.ADPMasterID,
                ADP = x.ADP_ID,
                Title = x.Title,
                FName = x.FName,
                LName = x.LName,
                Manager = x.Manager,
                Active = x.Active
            }), JsonRequestBehavior.AllowGet);
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
            [Bind(Include = "id,ADP_ID,FName,LName,Title,Manager,Active")] VMAdpMaster aDPMaster)
        {
            if (aDPMaster != null && ModelState.IsValid)
            {
                try
                {
                    //var storedInDb = await db.ADPMasters.FindAsync(aDPMaster.id);
                    var storedInDb = new ADPMaster {ADPMasterID = aDPMaster.ADPMaster_ID};//Avoid call to DB to get the object stored.
                    storedInDb.ADP_ID = aDPMaster.ADP;
                    storedInDb.Title = aDPMaster.Title;
                    storedInDb.Active = aDPMaster.Active;
                    storedInDb.FName = aDPMaster.FName;
                    storedInDb.LName = aDPMaster.LName;
                    storedInDb.Manager = aDPMaster.Manager;
                    db.ADPMasters.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
