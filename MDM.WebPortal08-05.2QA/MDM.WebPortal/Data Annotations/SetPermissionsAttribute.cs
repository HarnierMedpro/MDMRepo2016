using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using MDM.WebPortal.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MDM.WebPortal.Data_Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SetPermissionsAttribute : AuthorizeAttribute
    {
        public string Permissions { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isUserAuthorized = base.AuthorizeCore(httpContext);
            if (httpContext != null && isUserAuthorized)
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));
                //UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));


                /*Here I get the controller and the action that has been requested by the user
                 You can get that information from AppRelativeCurrentExecutionFilePath,
                 FilePath, Path Request's properties too.*/
                var request = httpContext.Request.CurrentExecutionFilePath;

                /*Here I converted the request var above into a list*/
                var url = request.Split('/').ToList();

                /*Make sure that the list does not have blanck space as item*/
                var aux = url.Where(item => item != "").ToList();

                string controller;
                string action;
                string areaController = "";
                string areaAction = "";
                var currentUserId = httpContext.User.Identity.GetUserId();

                switch (aux.Count())
                {
                    case 3:
                        controller = aux[1];
                        action = aux[2];
                        break;
                    /*In Case 2 may occurr one of two scenarios: The first one is "/Controller/Action/" and the second one is "/Area/Controller/" and by default the action is Index*/
                    case 2:
                        controller = aux[0];
                        action = aux[1];
                        areaController = aux[1];
                        areaAction = "Index";
                        break;

                    case 1:
                        controller = aux[0];
                        action = "Index";
                        break;

                    default:
                        return false;
                }
                if (httpContext.User.IsInRole("ADMIN"))
                {
                    return true;
                }

                if (dbContext.Permissions.Include(x => x.Roles).Include(x => x.Action).Any(x => x.Action.ControllerSystem.Cont_Name.Equals(controller, StringComparison.CurrentCultureIgnoreCase) && x.Action.Act_Name.Equals(action, StringComparison.CurrentCultureIgnoreCase) && x.Active))
                {
                    var roles = dbContext.Permissions.Include(x => x.Roles).Include(x => x.Action)
                                          .Where(x => x.Action.ControllerSystem.Cont_Name == controller && x.Action.Act_Name == action && x.Active)
                                          .Select(x => x.Roles).ToList();
                    var response = roles.SelectMany(item => item).Any(rol => userManager.IsInRole(currentUserId, rol.Name));
                    return response;
                }
                if (dbContext.Permissions.Include(x => x.Roles).Include(x => x.Action).Any(x => x.Action.ControllerSystem.Cont_Name.Equals(areaController, StringComparison.CurrentCultureIgnoreCase) && x.Action.Act_Name.Equals(areaAction, StringComparison.CurrentCultureIgnoreCase) && x.Active))
                {
                    var roles = dbContext.Permissions.Include(x => x.Roles).Include(x => x.Action)
                                          .Where(x => x.Action.ControllerSystem.Cont_Name == areaController && x.Action.Act_Name == areaAction && x.Active)
                                          .Select(x => x.Roles).ToList();
                    var response = roles.SelectMany(item => item).Any(rol => userManager.IsInRole(currentUserId, rol.Name));
                    return response;
                }
                return false;

            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                //filterContext.Result = new HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                filterContext.Result = new RedirectResult("/BadRequest/Error/Forbbiden");
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
      
    }
}