using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class FACInfoDatasController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/FACInfoDatas
        public async Task<ActionResult> Index()
        {
            var fACInfoDatas = db.FACInfoDatas.Include(f => f.POSLOCExtraData);
            return View(await fACInfoDatas.ToListAsync());
        }

        // GET: PlaceOfServices/FACInfoDatas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            var currentLocationPos = await db.LocationsPOS.FindAsync(id);
            //FACInfoData fACInfoData = await db.FACInfoDatas.FindAsync(id);
            if (currentLocationPos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }
            FACInfoData fACInfoData = currentLocationPos.FACInfoData;
            if (fACInfoData == null)
            {
                return View(new VMFACInfoData{Facitity_DBs_IDPK = id.Value});
            }
            VMFACInfoData toView = new VMFACInfoData
            {
                Facitity_DBs_IDPK = id.Value,
                FACInfoDataID = fACInfoData.FACInfoDataID,
                LicExpireDate = fACInfoData.LicExpireDate,
                LicEffectiveDate = fACInfoData.LicEffectiveDate,
                Taxonomy = fACInfoData.Taxonomy,
                LicNumCLIA_waiver = fACInfoData.Taxonomy,
                FAC_NPI_Num = fACInfoData.FAC_NPI_Num,
                LicType = fACInfoData.LicType,
                StateLic = fACInfoData.StateLic,
                DocProviderName = fACInfoData.DocProviderName
            };
            return View(toView);
        }

        // GET: PlaceOfServices/FACInfoDatas/Create
        /*Se va a crear un objeto FACInfoData si y solo si se esta modificando un objeto LocationsPOS; por ende se necesita conocer el ID de dicho LocationsPOS
         y ese es el valor que va a tomar la variable locPOS*/
        public ActionResult Create(int? locPOS)
        {
            if (locPOS != null)
            {
                ViewBag.Facitity_DBs_IDPK = locPOS;
                return View();
            }
            /*Si locPOS es nulo quiere decir que no se definio a que LocationsPOS object se le va a crear un objeto FACInfoData, por eso se redirecciona al index de LocationsPOS
             y se le notifica al usuario que ocurrio un error*/
            TempData["Error"] = "Something failed. Please try again!";
            return RedirectToAction("Index","LocationsPOS", new {area ="PlaceOfServices"});
        }

        // POST: PlaceOfServices/FACInfoDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FACInfoDataID,DocProviderName,LicType,StateLic,LicNumCLIA_waiver,LicEffectiveDate,LicExpireDate,Taxonomy,FAC_NPI_Num,Facitity_DBs_IDPK")] VMFACInfoData fACInfoData)
        {
            if (ModelState.IsValid && fACInfoData.Facitity_DBs_IDPK > 0)
            {
                try
                {
                    var currentLocPOS = await db.LocationsPOS.FindAsync(fACInfoData.Facitity_DBs_IDPK);
                    ICollection<LocationsPOS> Locations = new List<LocationsPOS> { currentLocPOS };

                    if (currentLocPOS != null)
                    {
                        var toStore = new FACInfoData
                        {
                            DocProviderName = fACInfoData.DocProviderName,
                            LicType = fACInfoData.LicType,
                            StateLic = fACInfoData.StateLic,
                            LicNumCLIA_waiver = fACInfoData.LicNumCLIA_waiver,
                            LicEffectiveDate = fACInfoData.LicEffectiveDate,
                            LicExpireDate = fACInfoData.LicExpireDate,
                            Taxonomy = fACInfoData.Taxonomy,
                            FAC_NPI_Num = fACInfoData.FAC_NPI_Num,
                            LocationsPOS = Locations
                        };
                        db.FACInfoDatas.Add(toStore);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = "Something failed. Please try again!";
                    return View(fACInfoData);
                }
                
            }
            //ViewBag.FACInfoDataID = new SelectList(db.POSLOCExtraDatas, "FACInfoData_FACInfoDataID", "Phone_Number", fACInfoData.FACInfoDataID);
            return View(fACInfoData);
        }

        // GET: PlaceOfServices/FACInfoDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FACInfoData fACInfoData = await db.FACInfoDatas.FindAsync(id);
            if (fACInfoData == null)
            {
                return HttpNotFound();
            }
            ViewBag.FACInfoDataID = new SelectList(db.POSLOCExtraDatas, "FACInfoData_FACInfoDataID", "Phone_Number", fACInfoData.FACInfoDataID);
            return View(fACInfoData);
        }

        // POST: PlaceOfServices/FACInfoDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FACInfoDataID,DocProviderName,LicType,StateLic,LicNumCLIA_waiver,LicEffectiveDate,LicExpireDate,Taxonomy,FAC_NPI_Num")] FACInfoData fACInfoData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fACInfoData).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FACInfoDataID = new SelectList(db.POSLOCExtraDatas, "FACInfoData_FACInfoDataID", "Phone_Number", fACInfoData.FACInfoDataID);
            return View(fACInfoData);
        }

        // GET: PlaceOfServices/FACInfoDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FACInfoData fACInfoData = await db.FACInfoDatas.FindAsync(id);
            if (fACInfoData == null)
            {
                return HttpNotFound();
            }
            return View(fACInfoData);
        }

        // POST: PlaceOfServices/FACInfoDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FACInfoData fACInfoData = await db.FACInfoDatas.FindAsync(id);
            db.FACInfoDatas.Remove(fACInfoData);
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
