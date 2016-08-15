using System.Web.Mvc;

namespace MDM.WebPortal.Areas.AudiTrails
{
    public class AudiTrailsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AudiTrails";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AudiTrails_default",
                "AudiTrails/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}