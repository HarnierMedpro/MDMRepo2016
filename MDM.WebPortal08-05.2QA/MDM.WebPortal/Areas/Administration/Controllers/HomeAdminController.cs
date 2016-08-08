using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDM.WebPortal.Areas.Administration.Controllers
{
    [AllowAnonymous]
    public class HomeAdminController : Controller
    {
        // GET: Administration/HomeAdmin
        public ActionResult Index()
        {
            return View();
        }
    }
}