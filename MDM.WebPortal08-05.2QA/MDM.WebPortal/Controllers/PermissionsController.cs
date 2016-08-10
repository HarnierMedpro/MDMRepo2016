using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Controllers;
using IdentitySample.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MDM.WebPortal.Controllers
{
    //[SetPermissions]
    [AllowAnonymous]
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            /*See: C:\Users\hsuarez\Desktop\Documentation\Kendo UI for ASP.NET MVC5\ui-for-aspnet-mvc-examples-master\grid\multiselect-in-grid-popup*/
            ViewData["Roles"] = db.Roles.Select(x => new VMPermissionRole { Id = x.Id, Name = x.Name });

            ViewData["Controllers"] = db.Controllers.Select(x => new VMControllerSystem { ControllerID = x.ControllerID, Cont_Name = x.Cont_Name });

            ViewData["Actions"] = db.Actions.Select(x => new VMActionSystem { ActionID = x.ActionID, Act_Name = x.Act_Name });

            return View();
        }

        public ActionResult Read_Premissions([DataSourceRequest] DataSourceRequest request)
        {
            /*See: C:\Users\hsuarez\Desktop\Documentation\Kendo UI for ASP.NET MVC5\ui-for-aspnet-mvc-examples-master\grid\multiselect-in-grid-popup*/
            var temp = db.Permissions.Include(x => x.Action).ToList();
            return Json(db.Permissions.Include(x => x.Action).ToDataSourceResult(request, x => new VMPermission
            {
                PermissionID = x.PermissionID,
                ControllerID = x.Action.ControllerID,
                ActionID = x.ActionID,
                Active = x.Active,
                Roles = x.Roles.Select(s => new VMPermissionRole { Id = s.Id, Name = s.Name }).ToList()
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeActions(int ControllerID)
        {
            return Json(db.Actions.Where(x => x.ControllerID == ControllerID).Select(x => new VMActionSystem { ActionID = x.ActionID, Act_Name = x.Act_Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Controllers()
        {
            return Json(db.Controllers.Select(x => new VMControllerSystem { ControllerID = x.ControllerID, Cont_Name = x.Cont_Name }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_Permission([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "PermissionID,ControllerID,Active,ActionID,Roles")] VMPermission permission)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Permissions.AnyAsync(x => x.ActionID == permission.ActionID && x.PermissionID != permission.PermissionID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { permission }.ToDataSourceResult(request, ModelState));
                    }
                    var storedInDb = await db.Permissions.FindAsync(permission.PermissionID);
                    storedInDb.ActionID = permission.ActionID;
                    storedInDb.Active = permission.Active;

                    var rolByParam = new List<ApplicationRole>();
                    var rolToStore = new List<ApplicationRole>();
                    var rolToDelete = new List<ApplicationRole>();

                    foreach (var item in permission.Roles)
                    {
                        using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db)))
                        {
                            rolByParam.Add(await appRoleManager.FindByIdAsync(item.Id));
                        }
                    }

                    rolToStore = rolByParam.Except(storedInDb.Roles.ToList()).ToList();

                    foreach (var item in rolToStore)
                    {
                        storedInDb.Roles.Add(item);
                    }

                    rolToDelete = storedInDb.Roles.ToList().Except(rolByParam).ToList();

                    foreach (var item in rolToDelete)
                    {
                        storedInDb.Roles.Remove(item);
                    }
                    db.Permissions.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();                   
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return Json(new[] { permission }.ToDataSourceResult(request, ModelState));
                }
            }            
            return Json(new[] { permission }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Create_Permission([DataSourceRequest] DataSourceRequest request,
           [Bind(Include = "PermissionID,ControllerID,ActionID,Active,Roles")] VMPermission permission)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (await db.Permissions.AnyAsync(x => x.ActionID == permission.ActionID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { permission }.ToDataSourceResult(request, ModelState));                       
                    }

                    var permissionObj = new Permission { PermissionID = permission.PermissionID, ActionID = permission.ActionID, Active = permission.Active};
                    permissionObj.Roles = new List<ApplicationRole>();
                    var Rol = new List<ApplicationRole>();

                    foreach (var item in permission.Roles)
                    {
                        using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db)))
                        {
                            Rol.Add(await appRoleManager.FindByIdAsync(item.Id));
                        }
                    }
                    permissionObj.Roles = Rol;
                    db.Permissions.Add(permissionObj);
                    await db.SaveChangesAsync();
                    permission.PermissionID = permissionObj.PermissionID;
                    permission.ControllerID = db.Actions.Find(permissionObj.ActionID).ControllerID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { permission }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { permission }.ToDataSourceResult(request, ModelState));
        }

////        // GET: Permissions/Create
////        public async Task<ActionResult> Create()
////        {
////            using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db)))
////            {
////                ViewBag.RoleId = new SelectList(await appRoleManager.Roles.Where(x => x.Active).OrderBy(x => x.Priority).ToListAsync(), "Id", "Name");
////            }
////            //ViewBag.RoleId = new SelectList(await db.Roles.ToListAsync(), "Id", "Name");
////            return View();
////        }

////        // POST: Permissions/Create
////        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
////        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
////        [HttpPost]
////        [ValidateAntiForgeryToken]
////        public async Task<ActionResult> Create([Bind(Include = "PermissionID,ControllerName,ActionName")] Permission permission, params string[] SelectedRoles)
////        {
////            if (ModelState.IsValid)
////            {
////                if (SelectedRoles == null)
////                {
////                    ViewBag.Role = "You have to select at least one role.";
////                    return View(permission);
////                }
////                try
////                {
////                    if (await db.Permissions.AnyAsync(x => x.ControllerName == permission.ControllerName && x.ActionName == permission.ActionName))
////                    {
////                        ViewBag.Role = "Duplicate Data.";
////                        return View(permission);
////                    }
////                    permission.Roles = new List<ApplicationRole>();
////                    var aux = new List<ApplicationRole>();

////                    using (var roleCont = new RolesAdminController(new ApplicationUserManager(new UserStore<ApplicationUser>(db)), new ApplicationRoleManager(new RoleStore<ApplicationRole>(db))))
////                    {
////                        aux = roleCont.GetRoles(SelectedRoles);
////                    }
////                    permission.Roles = aux;
////                    db.Permissions.Add(permission);                   
////                    await db.SaveChangesAsync();
////                    return RedirectToAction("Index");
////                }
////                catch (Exception)
////                {
////                    ViewBag.Role = "Something failed. Please try again!";
////                    ViewBag.RoleId = new SelectList(db.Roles.ToList(), "Id", "Name");
////                    return View(permission);
////                }
////            }
////            ViewBag.RoleId = new SelectList(await db.Roles.ToListAsync(), "Id", "Name");
////            return View(permission);
////        }

        ////        public ActionResult Read_RolesByPermissions([DataSourceRequest] DataSourceRequest request, int PermissionID)
        ////        {
        ////            var firstOrDefault = db.Permissions.Include(x => x.Roles).FirstOrDefault(x => x.PermissionID == PermissionID);
        ////            if (firstOrDefault != null)
        ////            {
        ////                var result =  firstOrDefault.Roles.OrderBy(x => x.Priority);
        ////                return Json(result.ToDataSourceResult(request, x => new VMRole
        ////                {
        ////                    Id = x.Id,
        ////                    Name = x.Name,
        ////                    Active = x.Active,
        ////                    Priotity = x.Priority
        ////                }), JsonRequestBehavior.AllowGet);
        ////            }
        ////            return Json(new List<ApplicationRole>().ToDataSourceResult(request));
        ////        }

        ////        public async Task<ActionResult> GetAllRoles([DataSourceRequest] DataSourceRequest request)
        ////        {
        ////            var appRoles = new List<ApplicationRole>();
        ////            using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db)))
        ////            {
        ////              appRoles = await appRoleManager.Roles.Where(x => x.Active).OrderBy(x => x.Priority).ToListAsync();
        ////            }
        ////            return Json(appRoles.ToDataSourceResult(request, x => new VMRole
        ////            {
        ////                Id = x.Id,
        ////                Name = x.Name,
        ////                Active = x.Active,
        ////                Priotity = x.Priority
        ////            }), JsonRequestBehavior.AllowGet);
        ////        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
