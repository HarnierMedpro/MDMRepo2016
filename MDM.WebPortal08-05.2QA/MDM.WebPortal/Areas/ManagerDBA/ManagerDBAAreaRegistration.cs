using System.Web.Mvc;

namespace MDM.WebPortal.Areas.ManagerDBA
{
    public class ManagerDBAAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ManagerDBA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ManagerDBA_default",
                "ManagerDBA/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}