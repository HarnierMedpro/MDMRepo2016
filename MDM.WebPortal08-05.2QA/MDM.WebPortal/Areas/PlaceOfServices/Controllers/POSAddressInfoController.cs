using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class POSAddressInfoController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: PlaceOfServices/POSAddressInfo
        public ActionResult Index()
        {
            return View();
        }

        //// GET: PlaceOfServices/POSAddressInfo/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: PlaceOfServices/POSAddressInfo/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: PlaceOfServices/POSAddressInfo/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: PlaceOfServices/POSAddressInfo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Error", new {area ="Error"});
            }
            var locationPOS = await db.LocationsPOS.FindAsync(id);

            if (locationPOS == null)
            {
                return RedirectToAction("Index", "Error", new { area = "Error" });
            }

            var toView = new VMLocationPOS_AddressInfo
            {
                Facitity_DBs_IDPK = id.Value,
                Payment_Addr1 = locationPOS.Payment_Addr1,
                Payment_Addr2 = locationPOS.Payment_Addr2,
                Payment_City = locationPOS.Payment_City,
                Payment_state = locationPOS.Payment_state,
                Payment_Zip = locationPOS.Payment_Zip,
                
                Physical_Addr1 = locationPOS.Physical_Addr1,
                Physical_Addr2 = locationPOS.Physical_Addr2,
                Physical_City = locationPOS.Physical_City,
                Physical_state = locationPOS.Physical_state,
                Physical_Zip = locationPOS.Physical_City,
               

            };
            return View(toView);
        }

        // POST: PlaceOfServices/POSAddressInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //// GET: PlaceOfServices/POSAddressInfo/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: PlaceOfServices/POSAddressInfo/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
