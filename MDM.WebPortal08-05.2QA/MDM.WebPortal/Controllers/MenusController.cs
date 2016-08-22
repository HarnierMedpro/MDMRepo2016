using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MDM.WebPortal.Controllers
{
    public class MenusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Menus
        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated && !User.IsInRole("ADMIN"))
            {
                var userID = User.Identity.GetUserId();

                var user = db.Users.Find(userID);

                var roleId = user.Roles.First().RoleId;

                ApplicationRole Role = new ApplicationRole();

                using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db)))
                {
                    Role = appRoleManager.FindById(roleId);
                }

                var Permissions = Role.Permissions.ToList();
                List<ActionSystem> actionSystems = Permissions.Select(permission => permission.Action).ToList();
                List<Menu> menuSystem = actionSystems.Select(action => db.Menus.FirstOrDefault(x => x.ActionID == action.ActionID)).Where(menu => menu != null).ToList();

                List<Menu> toReturn = new List<Menu>();

                List<Menu> allRoots = menuSystem.Select(menu => FindingTheRoot(menu)).ToList();
                /*foreach (var menu in menuSystem)
                {
                    var findItsRoot = FindingTheRoot(menu);
                    allRoots.Add(findItsRoot);
                }*/
                toReturn = allRoots.Distinct().ToList();

                return View(toReturn);
            }
            var menus = await db.Menus.Include(m => m.actionSystem).Include(p => p.Parent).Include(c => c.ChildMenus).Where(x => x.Parent == null).ToListAsync();
            return View(menus);
        }


        public async Task<ActionResult> Testing()
        {
            if (User.Identity.IsAuthenticated && !User.IsInRole("ADMIN"))
            {
                var userID = User.Identity.GetUserId();

                var user = db.Users.Find(userID);

                var roleId = user.Roles.First().RoleId;

                ApplicationRole Role = new ApplicationRole();

                using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db)))
                {
                    Role = appRoleManager.FindById(roleId);
                }

                var Permissions = Role.Permissions.ToList();
                List<ActionSystem> actionSystems = Permissions.Select(permission => permission.Action).ToList();
                List<Menu> menuSystem = actionSystems.Select(action => db.Menus.FirstOrDefault(x => x.ActionID == action.ActionID)).Where(menu => menu != null).ToList();

                List<Menu> toReturn = new List<Menu>();

                List<Menu> allRoots = menuSystem.Select(menu => FindingTheRoot(menu)).ToList();
                /*foreach (var menu in menuSystem)
                {
                    var findItsRoot = FindingTheRoot(menu);
                    allRoots.Add(findItsRoot);
                }*/
                toReturn = allRoots.Distinct().ToList();

                return PartialView("Testing",toReturn);
            }
            var menus = await db.Menus.Include(m => m.actionSystem).Include(p => p.Parent).Include(c => c.ChildMenus).Where(x => x.Parent == null).ToListAsync();
            return PartialView("Testing",menus);
        }



        public Menu FindingTheRoot(Menu men)
        {
            if (men != null && men.ActionID == null && men.Parent == null)
            {
                return men;
            }
            else
            {
                return FindingTheRoot(men.Parent);
            }
        }

        public static Menu FindingTheRaiz(Menu men)
        {
            if (men != null && men.ActionID == null && men.Parent == null)
            {
                return men;
            }
            else
            {
                return FindingTheRaiz(men.Parent);
            }
        }

        public static List<Menu> GetMenus(bool isAuthenticated, bool isAdmin, string userI)
        {
            using (var context = new ApplicationDbContext())
            {
                if (isAuthenticated && !isAdmin)
                {
                    var userID = userI;

                    var user = context.Users.Find(userID);

                    var roleId = user.Roles.First().RoleId;

                    ApplicationRole Role = new ApplicationRole();

                    using (var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context)))
                    {
                        Role = appRoleManager.FindById(roleId);
                    }

                    var Permissions = Role.Permissions.ToList();
                    List<ActionSystem> actionSystems = Permissions.Select(permission => permission.Action).ToList();
                    List<Menu> menuSystem = actionSystems.Select(action => context.Menus.FirstOrDefault(x => x.ActionID == action.ActionID)).Where(menu => menu != null).ToList();

                    List<Menu> toReturn = new List<Menu>();

                    List<Menu> allRoots = menuSystem.Select(menu => FindingTheRaiz(menu)).ToList();
                    /*foreach (var menu in menuSystem)
                    {
                        var findItsRoot = FindingTheRoot(menu);
                        allRoots.Add(findItsRoot);
                    }*/
                    toReturn = allRoots.Distinct().ToList();

                    return toReturn;
                }
                return context.Menus.Include(m => m.actionSystem).Include(p => p.Parent).Include(c => c.ChildMenus).Where(x => x.Parent == null).ToList();
            }
            
        }

     

        // GET: Menus/Create
        public ActionResult Create()
        {
            ViewBag.ActionID = new SelectList(db.Actions, "ActionID", "Act_Name");
            ViewBag.Parents = new SelectList(db.Menus, "MenuID", "Title");
            return View();
            //return Json(ViewBag.Parents, JsonRequestBehavior.AllowGet);
        }

        // POST: Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MenuID,ActionID, ParentId ,Title")] VMMenu menu)
        {
            if (ModelState.IsValid)
            {
                var toStore = new Menu
                {
                    ActionID = menu.ActionID,
                    Title = menu.Title,
                    Parent = db.Menus.Find(1)
                };
                db.Menus.Add(toStore);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ActionID = new SelectList(db.Actions, "ActionID", "Act_Name", menu.ActionID);
            return View(menu);
        }

       

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
