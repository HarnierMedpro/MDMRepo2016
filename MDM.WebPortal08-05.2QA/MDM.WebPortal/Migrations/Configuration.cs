using MDM.WebPortal.Models;
using MDM.WebPortal.Models.Identity;

namespace MDM.WebPortal.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MDM.WebPortal.Models.Identity.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MDM.WebPortal.Models.Identity.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Areas.AddOrUpdate(a => a.AreaName,
                new AreaSystem { AreaID = 1, AreaName = "ActionCode"},
                new AreaSystem { AreaID = 2, AreaName = "ADP"},
                new AreaSystem { AreaID = 3, AreaName = "ManagerDBA"}
                );

            context.Controllers.AddOrUpdate(c => c.Cont_Name,
                new ControllerSystem { ControllerID = 1, Cont_Name = "Account", AreaID = null },
                new ControllerSystem { ControllerID = 2, Cont_Name = "ActionSystems", AreaID = null },
                new ControllerSystem { ControllerID = 3, Cont_Name = "AreaSystems", AreaID = null },
                new ControllerSystem { ControllerID = 4, Cont_Name = "ControllerSystems", AreaID = null },
                new ControllerSystem { ControllerID = 5, Cont_Name = "Home", AreaID = null },
                new ControllerSystem { ControllerID = 6, Cont_Name = "Manage", AreaID = null },
                new ControllerSystem { ControllerID = 7, Cont_Name = "Permissions", AreaID = null },
                new ControllerSystem { ControllerID = 8, Cont_Name = "RolesAdmin", AreaID = null },
                new ControllerSystem { ControllerID = 9, Cont_Name = "UsersAdmin", AreaID = null },
                new ControllerSystem { ControllerID = 10, Cont_Name = "Corp_DBs", AreaID = null },
                new ControllerSystem { ControllerID = 11, Cont_Name = "Corp_Owner", AreaID = null },
                new ControllerSystem { ControllerID = 12, Cont_Name = "CorporateMasterList", AreaID = null },
                new ControllerSystem { ControllerID = 13, Cont_Name = "CPTDatas", AreaID = null },
                new ControllerSystem { ControllerID = 14, Cont_Name = "DBLists", AreaID = null },
                new ControllerSystem { ControllerID = 15, Cont_Name = "MDM_POS_ListName", AreaID = null },
                new ControllerSystem { ControllerID = 16, Cont_Name = "MDM_POS_Name_DBPOS_grp", AreaID = null },
                new ControllerSystem { ControllerID = 17, Cont_Name = "OwnerLists", AreaID = null },
                new ControllerSystem { ControllerID = 18, Cont_Name = "ACCategory", AreaID = 1 },
                new ControllerSystem { ControllerID = 19, Cont_Name = "ActionCodes", AreaID = 1 },
                new ControllerSystem { ControllerID = 20, Cont_Name = "ACTypes", AreaID = 1 },
                new ControllerSystem { ControllerID = 21, Cont_Name = "CodeMasterLists", AreaID = 1 },
                new ControllerSystem { ControllerID = 22, Cont_Name = "ADPMasters", AreaID = 2 },
                new ControllerSystem { ControllerID = 23, Cont_Name = "Edgemed_Logons", AreaID = 2 },
                new ControllerSystem { ControllerID = 24, Cont_Name = "BI_DB_FvP_Access", AreaID = 3 },
                new ControllerSystem { ControllerID = 25, Cont_Name = "Manager_Master", AreaID = 3 },
                new ControllerSystem { ControllerID = 26, Cont_Name = "Manager_Type", AreaID = 3 }
           );

            context.Actions.AddOrUpdate(ac => ac.Act_Name,
                new ActionSystem { ActionID = 1, Act_Name = "Index", ControllerID = 10},
                new ActionSystem { ActionID = 2, Act_Name = "Read", ControllerID = 10},
                new ActionSystem { ActionID = 3, Act_Name = "Update", ControllerID = 10},
                new ActionSystem { ActionID = 4, Act_Name = "Corp_DBs_Release", ControllerID = 10},
                new ActionSystem { ActionID = 5, Act_Name = "Create", ControllerID = 10},

                new ActionSystem { ActionID = 6, Act_Name = "Index", ControllerID = 12},
                new ActionSystem { ActionID = 7, Act_Name = "CorporateMasterLists_Read", ControllerID = 12},
                new ActionSystem { ActionID = 8, Act_Name = "CorporateMasterLists_Update", ControllerID = 12},
                new ActionSystem { ActionID = 9, Act_Name = "CorporateMasterLists_Create", ControllerID = 12},

                new ActionSystem { ActionID = 10, Act_Name = "Index", ControllerID = 13},
                new ActionSystem { ActionID = 11, Act_Name = "Read", ControllerID = 13},
                new ActionSystem { ActionID = 12, Act_Name = "Create_CPT", ControllerID = 13},
                new ActionSystem { ActionID = 13, Act_Name = "Update", ControllerID = 13},

                new ActionSystem { ActionID = 14, Act_Name = "Index", ControllerID = 15},
                new ActionSystem { ActionID = 15, Act_Name = "Read", ControllerID = 15},
                new ActionSystem { ActionID = 16, Act_Name = "Update", ControllerID = 15},
                new ActionSystem { ActionID = 17, Act_Name = "Create_POS", ControllerID = 15},

                new ActionSystem { ActionID = 18, Act_Name = "Index", ControllerID = 16},
                new ActionSystem { ActionID = 19, Act_Name = "Read", ControllerID = 16},
                new ActionSystem { ActionID = 20, Act_Name = "Update_Entity", ControllerID = 16},
                new ActionSystem { ActionID = 21, Act_Name = "Create_Entity", ControllerID = 16},

                new ActionSystem { ActionID = 22, Act_Name = "Index", ControllerID = 18},
                new ActionSystem { ActionID = 23, Act_Name = "Read_Category", ControllerID = 18},
                new ActionSystem { ActionID = 24, Act_Name = "Create_Category", ControllerID = 18},
                new ActionSystem { ActionID = 25, Act_Name = "Update_Category", ControllerID = 18},

                new ActionSystem { ActionID = 26, Act_Name = "Index", ControllerID = 19},
                new ActionSystem { ActionID = 27, Act_Name = "Read_ActionCode", ControllerID = 19},
                new ActionSystem { ActionID = 28, Act_Name = "Create_ActionCode", ControllerID = 19},
                new ActionSystem { ActionID = 29, Act_Name = "Update_ActionCode", ControllerID = 19},

                new ActionSystem { ActionID = 30, Act_Name = "Index", ControllerID = 22},
                new ActionSystem { ActionID = 31, Act_Name = "Read_Adp", ControllerID = 22},
                new ActionSystem { ActionID = 32, Act_Name = "EdgeMedLogonsforAdp", ControllerID = 22},
                new ActionSystem { ActionID = 33, Act_Name = "Update_Adp", ControllerID = 22},

                new ActionSystem { ActionID = 34, Act_Name = "Index", ControllerID = 24},
                new ActionSystem { ActionID = 35, Act_Name = "Read_GroupByManager", ControllerID = 24},
                new ActionSystem { ActionID = 36, Act_Name = "Read_BI_DB_FvPByManager", ControllerID = 24},
                new ActionSystem { ActionID = 37, Act_Name = "Update_BI_DB_FvP", ControllerID = 24},
                new ActionSystem { ActionID = 38, Act_Name = "Create_BI_DB_FvP", ControllerID = 24 }
           );

            context.Menus.AddOrUpdate(m => m.Title,
                new Menu { MenuID = 5, Title = "CORPORATIONS"},
                new Menu { MenuID = 6, ActionID = 6, ParentId = 5, Title = "CorporateMasterList"},
                new Menu { MenuID = 7, ActionID = 1, ParentId = 5, Title = "Corp_DBs" }
            );
        }
    }
}
