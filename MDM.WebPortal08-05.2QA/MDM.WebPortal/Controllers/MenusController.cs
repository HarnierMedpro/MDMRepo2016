using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Models.Identity;
using MDM.WebPortal.Models.ViewModel;
using Menu = MDM.WebPortal.Models.Identity.Menu;

namespace MDM.WebPortal.Controllers
{
    public class MenusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Menus
        public ActionResult Index()
        {
            ViewData["Actions"] = db.Actions.Include(controller => controller.ControllerSystem)
                                            .Where(action => action.Act_Name.Equals("Index",StringComparison.InvariantCultureIgnoreCase))
                                            .Select(action => new VMActionEditor { ActionID = action.ActionID, Info = action.Act_Name + " " + "FROM" + " " + action.ControllerSystem.Cont_Name });

            ViewData["Parent"] = db.Menus.Where(x => x.ActionID == null).Select(x => new { ParentId = x.MenuID, Title = x.Title });
            //return Json(ViewData["Actions"], JsonRequestBehavior.AllowGet);
            return View();
        }

        public JsonResult GetActions([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Actions.Include(controller => controller.ControllerSystem).
                Where(action => action.Act_Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase)).ToDataSourceResult(request, x => new VMActionEditor
            {
                ActionID = x.ActionID,
                Info = x.Act_Name + " " + "FROM" + " " + x.ControllerSystem.Cont_Name
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Read_Menu([DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.Menus.ToDataSourceResult(request, x => new VMMenu
            {
                MenuID = x.MenuID,
                ActionID = x.ActionID,
                ParentId = x.ParentId,
                Title = x.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create_Menu([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MenuID,ParentId,ActionID,Title")] VMMenu Menu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((Menu.ActionID != null && await db.Menus.AnyAsync(menu => menu.ActionID == Menu.ActionID)) || await db.Menus.AnyAsync(x => x.Title.Equals(Menu.Title, StringComparison.InvariantCultureIgnoreCase)) )
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { Menu }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new Menu
                    {
                        ActionID = Menu.ActionID,
                        ParentId = Menu.ParentId,
                        Title = Menu.Title
                    };

                    db.Menus.Add(toStore);
                    await db.SaveChangesAsync();
                    Menu.MenuID = toStore.MenuID;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something failed. Please try again!");
                    return Json(new[] { Menu }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { Menu }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Insert_Menu([DataSourceRequest] DataSourceRequest request, [Bind(Include = "MenuID,ParentId,ActionID,Title")] VMMenu Menues)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((Menues.ActionID != null && await db.Menus.AnyAsync(menu => menu.ActionID == Menues.ActionID)) || await db.Menus.AnyAsync(x => x.Title.Equals(Menues.Title, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { Menues }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new Menu
                    {
                        ActionID = Menues.ActionID,
                        ParentId = Menues.ParentId,
                        Title = Menues.Title
                    };

                    db.Menus.Add(toStore);
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something Failed. Please try again!");
                    return Json(new[] { Menues }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { Menues }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> Update_Menu([DataSourceRequest] DataSourceRequest request,
            [Bind(Include = "MenuID,ParentId,ActionID,Title")] VMMenu Menu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((Menu.ActionID != null && await db.Menus.AnyAsync(menu => menu.ActionID == Menu.ActionID && menu.MenuID != Menu.MenuID)) || await db.Menus.AnyAsync(x => x.Title.Equals(Menu.Title, StringComparison.InvariantCultureIgnoreCase) && x.MenuID != Menu.MenuID))
                    {
                        ModelState.AddModelError("", "Duplicate Data. Please try again!");
                        return Json(new[] { Menu }.ToDataSourceResult(request, ModelState));
                    }
                    var toStore = new Menu
                    {
                        MenuID = Menu.MenuID,
                        ActionID = Menu.ActionID,
                        ParentId = Menu.ParentId,
                        Title = Menu.Title
                    };

                    db.Menus.Attach(toStore);
                    db.Entry(toStore).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Something Failed. Please try again!");
                    return Json(new[] { Menu }.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { Menu }.ToDataSourceResult(request, ModelState));
        }

        public static List<Menu> GetMyMenus()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Menus.Include(x => x.actionSystem).ToList();
               
            }
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
