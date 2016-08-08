using System.Web.Mvc;

namespace IdentitySample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {            
            filters.Add(new HandleErrorAttribute()); //Original Code
            //filters.Add(new HandleErrorAttribute());
            //filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
