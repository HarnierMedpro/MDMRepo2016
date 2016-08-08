using System.Web.Mvc;

namespace MDM.WebPortal.Areas.ADP
{
    public class ADPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ADP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute("GetAdps",
            //"ADP/Edgemed_Logons/GetAdps",
            //new { controller = "Edgemed_Logons", action = "GetAdps" },
            //new[] { "MDM.WebPortal.Areas.ADP.Controllers" }
            //);

            context.MapRoute(
                "ADP_default",
                "ADP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}