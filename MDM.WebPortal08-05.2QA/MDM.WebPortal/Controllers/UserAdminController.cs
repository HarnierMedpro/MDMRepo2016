using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.DAL;
using MDM.WebPortal.Models.ViewModel;
using System.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MDM.WebPortal.Tools;
using System.Globalization;
using MDM.WebPortal.Models.Identity;

namespace IdentitySample.Controllers
{
    //[Authorize(Roles = "ADMIN")]
    [AllowAnonymous]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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
            private set
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
        // GET: /Users ORIGINAL CODE/
        //public async Task<ActionResult> Index()
        //{
        //    return View(await UserManager.Users.ToListAsync());            
        //}

        /*Using Kendo UI for ASP.NET MVC*/
        public ActionResult Index()
        {
           // var roles = RoleManager.Roles.ToList().Select(x => new { roleId = x.Id, roleName = x.Name }).ToList();
            ViewData["Roles"] = RoleManager.Roles.ToList().Select(x => new {roleId = x.Id, roleName = x.Name});
            //var x = await RoleManager.Roles.ToListAsync();
            return View();
        }
        
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(UserManager.Users.ToList().ToDataSourceResult(request, x => new VMUser
            {
                Active = x.Active,
                Id = x.Id,
                Email = x.Email,
                roleId = x.Roles.ElementAt(0).RoleId,//Here we are assuming that the user just have assigned one role.
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update([DataSourceRequest] DataSourceRequest request, [Bind(Include = "Email,Id, Active, roleId")] VMUser editUser)
        {
            if (editUser != null && ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.Active = editUser.Active;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                /*Partiendo de la idea de que un usuario solo va a tener un rol en el systema podemos decir*/
                var myCurrentRole = await RoleManager.FindByNameAsync(userRoles.ElementAt(0));
                if (myCurrentRole.Active == false && editUser.Active == true)
                {
                    ModelState.AddModelError("","You can not activate the user because it's currently asociated to an incative role.");
                    return Json(new[] { editUser }.ToDataSourceResult(request, ModelState));
                }

                var newRole = RoleManager.FindById(editUser.roleId);
                string[] selectedRole = new string[] { newRole.Name };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return Json(new[]{user}.ToDataSourceResult(request,ModelState));
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return Json(new[] { user }.ToDataSourceResult(request, ModelState));
                }
                return Json(new[] { editUser }.ToDataSourceResult(request, ModelState));
            }
            ModelState.AddModelError("", "Something failed. Please try again!");
            return Json(new[] {editUser}.ToDataSourceResult(request,ModelState));
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.Where(x => x.Active == true).OrderBy(x => x.Priority).ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]       
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, Active = true};
                //var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                /*The user has to have roles assigned to himselft*/
                if (selectedRoles != null)
                {
                    var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
                    if (adminresult.Succeeded)
                    {
                      var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                      if (!result.Succeeded)
                      {
                          ModelState.AddModelError("", result.Errors.First());
                          ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                          return View();
                      }
                      else
                      {
                          /*Send Email confirmation to the new User*/
                          var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                          var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                          //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                          
                          //List<string> emailsTo = new List<string>();
                          //emailsTo.Add(user.Email);

                          string body = string.Format(CultureInfo.InvariantCulture,
                                    @"<center>
                                    <h3> Hi {0} !</h3>
                                    <div> Congratulations on your new MDM account! Getting set up with MDM is quick and easy.You can get integrated in minutes. </div>
                                    <div> Your User is: {0} </div>
                                    <div> Your Password is: {2} </div>
                                    <div> Let's confirm your account by clicking on the following link. </div>
                                    <div> <a href='{1}' target='_top'>Confirm</a></div>                                    
                                    </center>",
                                        user.Email, callbackUrl, userViewModel.Password);
                          string Subject = "Confirm your account.";                       


                          //NotificationHelper notification = new NotificationHelper();

                          //notification.SendEmailsAdvanced(emailsTo, Subject, body);

                          //string body = "";

                          Mail email = new Mail();
                          email.To.Add(user.Email);
                          email.Subject = Subject;
                          email.Body = body;
                          await email.SendAsync();
                      }
                    }
                    else
                    {
                        ModelState.AddModelError("", adminresult.Errors.First());
                        ViewBag.RoleId = new SelectList(await RoleManager.Roles.Where(x => x.Active == true).OrderBy(x => x.Priority).ToListAsync(), "Name", "Name");
                        return View();
                    }
                }
                else
                {
                    //ModelState.AddModelError("", "You have to select the Role.");
                    ViewBag.Role = "You have to select the Role.";
                    ViewBag.RoleId = new SelectList(await RoleManager.Roles.Where(x => x.Active == true).OrderBy(x => x.Priority).ToListAsync(), "Name", "Name");
                    return View(userViewModel);
                }
                return RedirectToAction("Index");              
               
            }
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.Where(x => x.Active == true).OrderBy(x => x.Priority).ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckUserName(string term)
        {
            if (!String.IsNullOrEmpty(term))
            {
                _DALUsers users = new _DALUsers();
                var resultSet = users.BuscarUser(term.ToLower());
                List<VMUser> result = new List<VMUser>();
                foreach (DataRow fila in resultSet.Rows)
                {
                    result.Add(new VMUser
                    {
                        Email = fila.ItemArray[0].ToString(),
                        Id = fila.ItemArray[1].ToString()
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
                return RedirectToAction("Index", "Error", new {area = "Error" });
        }
    }
}
