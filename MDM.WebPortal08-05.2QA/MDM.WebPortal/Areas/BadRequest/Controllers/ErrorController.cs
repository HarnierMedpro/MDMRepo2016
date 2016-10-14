using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDM.WebPortal.Areas.BadRequest.Controllers
{
    public class ErrorController : Controller
    {
        // GET: BadRequest/Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Forbbiden()
        {
            return View();
        }

        public ActionResult UnothorizedRequest()
        {
            return View();
        }
    }
}