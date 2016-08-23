using System.Web.Mvc;

namespace MDM.WebPortal.Areas.PlaceOfServices
{
    public class PlaceOfServicesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PlaceOfServices";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PlaceOfServices_default",
                "PlaceOfServices/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}