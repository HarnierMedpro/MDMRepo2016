using System.Web.Mvc;
using System.Web.Routing;

namespace IdentitySample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "AlternateManagerBIAccess",
                url: "ManagerBIAccessold/{action}/{id}",
                defaults: new { controller = "ManagerDBAccessBIs", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "AlternateActionCode",
                url: "ActionCodeold/{action}/{id}",
                defaults: new { controller = "CollNoteTypeCatPriorities", action = "Index", id = UrlParameter.Optional }
                );

            routes.MapRoute("CheckUserName",
              "UsersAdmin/CheckUserName",
              new { controller = "UsersAdmin", action = "CheckUserName" },
              new[] { "MDM.WebPortal.Controllers" }
              );

            routes.MapRoute("CheckRelationship",
             "Corp_Owner/CheckRelationship",
             new { controller = "Corp_Owner", action = "CheckRelationship" },
             new[] { "MDM.WebPortal.Controllers" }
             );

            routes.MapRoute("CheckIfExist",
             "MDM_POS_ListName/CheckIfExist",
             new { controller = "MDM_POS_ListName", action = "CheckIfExist" },
             new[] { "MDM.WebPortal.Controllers" }
             );

            routes.MapRoute("CheckDB",
             "DBLists/CheckDB",
             new { controller = "DBLists", action = "CheckDB" },
             new[] { "MDM.WebPortal.Controllers" }
             );

            routes.MapRoute("GetFacilities",
             "MDM_POS_Name_DBPOS_grp/GetFacilities",
             new { controller = "MDM_POS_Name_DBPOS_grp", action = "GetFacilities" },
             new[] { "MDM.WebPortal.Controllers" }
             );

            routes.MapRoute("FindCpt",
             "CPTDatas/FindCpt",
             new { controller = "CPTDatas", action = "FindCpt" },
             new[] { "MDM.WebPortal.Controllers" }
             );

            routes.MapRoute("GetAdps",
             "ADP/Edgemed_Logons/GetAdps",
             new { controller = "Edgemed_Logons", action = "GetAdps" },
             new[] { "MDM.WebPortal.Areas.ADP.Controllers" }
             );

            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}