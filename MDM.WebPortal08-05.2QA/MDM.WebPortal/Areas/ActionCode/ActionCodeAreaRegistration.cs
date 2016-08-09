using System.Web.Mvc;

namespace MDM.WebPortal.Areas.ActionCode
{
    public class ActionCodeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ActionCode";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ActionCode_default",
                "ActionCode/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}