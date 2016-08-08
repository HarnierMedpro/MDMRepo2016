using System;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data.Entity;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;

namespace IdentitySample.Controllers
{
    //[Authorize(Roles = "ADMIN")]
    [AllowAnonymous]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Roles/
        public ActionResult Index1()
        {
            return View(RoleManager.Roles.OrderBy(x => x.Priority).ToList());
            using (var context = new ApplicationDbContext())
            {
                context.Roles.OrderBy(x => x.Name);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read_Roles([DataSourceRequest] DataSourceRequest request)
        {
            var roles = RoleManager.Roles.OrderBy(x => x.Priority).ToList();
            return Json(roles.ToDataSourceResult(request, x => new RoleViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Active = x.Active,
                Priority = x.Priority
            }), JsonRequestBehavior.AllowGet);
        }

        ///*Get al DB_Facilities of specific POS*/
        public ActionResult Read_UserOfrole([DataSourceRequest] DataSourceRequest request, string roleID)
        {
            List<ApplicationUser> usuarios = new List<ApplicationUser>();
            if (!String.IsNullOrEmpty(roleID))
            {
                using (var db = new ApplicationDbContext())
                {
                   usuarios = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleID)).ToList();
                }
            }
            return Json(usuarios.ToDataSourceResult(request, x => new VMUser
            {
                Id = x.Id,
                Email = x.Email,
                Active = x.Active
            }));
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "Name,Id,Active,Priority")] RoleViewModel roleModel)
        {
            if (roleModel != null && ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Active = roleModel.Active;
                var result = await RoleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] { roleModel }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    /*Si se inactiva el rol, hay que inactivar de igual modo todos los usuarios que 
                     han sido asignados a este rol*/
                    if (roleModel.Active == false)
                    {
                        List<ApplicationUser> usuarios = new List<ApplicationUser>();
                        using (var db = new ApplicationDbContext())
                        {
                            usuarios = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleModel.Id)).ToList();
                            foreach (var user in usuarios)
                            {
                                user.Active = false;
                                db.Entry(user).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        
                    }
                }
                
            }
            return Json(new[] { roleModel }.ToDataSourceResult(request, ModelState));
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Name, Priority")]RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name, true, roleViewModel.Priority);

                if (! await RoleManager.RoleExistsAsync(role.Name))
                {
                    var roleresult = await RoleManager.CreateAsync(role);
                    if (!roleresult.Succeeded)
                    {
                        ModelState.AddModelError("", roleresult.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.Error = "Duplicate Role.";
                return View();
            }
            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public List<ApplicationRole> GetRoles(string[] SelectedRoles)
        {
            List<ApplicationRole> result = new List<ApplicationRole>();
            foreach (var role in SelectedRoles)
            {
                var toStore = RoleManager.FindById(role);
                result.Add(toStore);
            }
            return result;
        }
    }
}
