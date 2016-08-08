using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eMedServiceCorp.Areas.Error.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}