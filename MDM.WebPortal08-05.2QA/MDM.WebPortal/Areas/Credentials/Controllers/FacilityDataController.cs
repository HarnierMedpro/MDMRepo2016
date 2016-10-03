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
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class FacilityDataController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public async Task<ActionResult> Facility_Info(int? MasterPOSID)
        {
            if (MasterPOSID == null)
            {
                return RedirectToAction("Error", "Error", new { area = "Error" });
            }
            var pos = await db.MasterPOS.FindAsync(MasterPOSID);
            if (pos == null)
            {
                return RedirectToAction("Error", "Error", new { area = "Error" });
            }
            var facInfo = pos.FACInfo;
            var toView = new List<VMFacilityData>();
            if (facInfo != null)
            {
                toView.Add(new VMFacilityData
                {
                    FACInfoID = facInfo.FACInfoID,
                    NPI_Number = facInfo.NPI_Number,
                    MasterPOSID = MasterPOSID.Value
                });
            }
            
            ViewBag.MasterPOS = MasterPOSID;
            return View(toView);
        }

        public async Task<ActionResult> Create_Facility([DataSourceRequest] DataSourceRequest request, [Bind(Include = "FACInfoID,MasterPOSID,NPI_Number")] VMFacilityData facility, int? ParentID)
        {
            if (ModelState.IsValid)
            {
                if (ParentID == null || ParentID == 0)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
                }
                using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var toStore = new FACInfo
                        {
                            NPI_Number = facility.NPI_Number
                        };

                        db.FACInfoes.Add(toStore);
                        await db.SaveChangesAsync();
                        facility.FACInfoID = toStore.FACInfoID;

                        var pos = await db.MasterPOS.FindAsync(ParentID);
                        pos.FACInfo_FACInfoID = toStore.FACInfoID;
                        db.MasterPOS.Attach(pos);
                        db.Entry(pos).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        facility.MasterPOSID = ParentID.Value;

                        dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        ModelState.AddModelError("","Something failed. Please try again!");
                        return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            return Json(new[] {facility}.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Facility([DataSourceRequest] DataSourceRequest request, [Bind(Include = "FACInfoID,MasterPOSID,NPI_Number")] VMFacilityData facility, int? ParentID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.FACInfoes.FindAsync(facility.FACInfoID);

                    storedInDb.NPI_Number = facility.NPI_Number;

                    db.FACInfoes.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { facility }.ToDataSourceResult(request, ModelState));
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
