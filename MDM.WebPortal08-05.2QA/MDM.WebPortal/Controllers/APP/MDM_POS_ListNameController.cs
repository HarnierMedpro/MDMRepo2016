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
using MDM.WebPortal.Models.ViewModel;

namespace MDM.WebPortal.Controllers.APP
{
    public class MDM_POS_ListNameController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();
      
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = await db.MDM_POS_ListName.Select(x => new VMMDM_POS_ListName { active = x.active, MDMPOS_ListNameID = x.MDMPOS_ListNameID, PosName = x.PosName }).ToListAsync();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "MDMPOS_ListNameID,PosName,active")] VMMDM_POS_ListName mDM_POS_ListName)
        {
            if (ModelState.IsValid)
            {               
                try
                {
                    if (await db.MDM_POS_ListName.AnyAsync(x => x.PosName.Equals(mDM_POS_ListName.PosName, StringComparison.CurrentCultureIgnoreCase) && x.MDMPOS_ListNameID != mDM_POS_ListName.MDMPOS_ListNameID))
                    {                       
                        ModelState.AddModelError("", "Duplicate POS. Please try again!");
                        return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
                    }
                    var StoredInDB = new MDM_POS_ListName
                    {
                        MDMPOS_ListNameID = mDM_POS_ListName.MDMPOS_ListNameID, 
                        PosName = mDM_POS_ListName.PosName, 
                        active = mDM_POS_ListName.active
                    };
                    db.MDM_POS_ListName.Attach(StoredInDB);
                    db.Entry(StoredInDB).State = EntityState.Modified;
                    await db.SaveChangesAsync();                  
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
                } 
            }           
            return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));         
        }

        public async Task<ActionResult> Create_POS([DataSourceRequest] DataSourceRequest request, [Bind(Include = "MDMPOS_ListNameID,PosName,active")] VMMDM_POS_ListName mDM_POS_ListName)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.MDM_POS_ListName.AnyAsync(x => x.PosName.Equals(mDM_POS_ListName.PosName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "Duplicate POS. Please try again!");
                        return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
                    }
                    var StoredInDB = new MDM_POS_ListName { PosName = mDM_POS_ListName.PosName, active = mDM_POS_ListName.active };
                    db.MDM_POS_ListName.Add(StoredInDB);
                    await db.SaveChangesAsync();
                    mDM_POS_ListName.MDMPOS_ListNameID = StoredInDB.MDMPOS_ListNameID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { mDM_POS_ListName }.ToDataSourceResult(request, ModelState));
        }

       

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckIfExist(string term)
        {
            return Json(db.MDM_POS_ListName.Where(x => x.PosName == term).Select(x => x.PosName).ToList(), JsonRequestBehavior.AllowGet);
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
