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
               new AreaSystem { AreaID = 1, AreaName = "ActionCode" },
               new AreaSystem { AreaID = 2, AreaName = "ADP" },
               new AreaSystem { AreaID = 3, AreaName = "ManagerDBA" }
               );

            context.Controllers.AddOrUpdate(c => c.Cont_Name,
                new ControllerSystem { ControllerID = 2, Cont_Name = "Account", AreaID = null },
                new ControllerSystem { ControllerID = 3, Cont_Name = "ActionSystems", AreaID = null },
                new ControllerSystem { ControllerID = 4, Cont_Name = "AreaSystems", AreaID = null },
                new ControllerSystem { ControllerID = 5, Cont_Name = "ControllerSystems", AreaID = null },
                new ControllerSystem { ControllerID = 6, Cont_Name = "Home", AreaID = null },
                new ControllerSystem { ControllerID = 7, Cont_Name = "Manage", AreaID = null },
                new ControllerSystem { ControllerID = 8, Cont_Name = "Permissions", AreaID = null },
                new ControllerSystem { ControllerID = 9, Cont_Name = "RolesAdmin", AreaID = null },
                new ControllerSystem { ControllerID = 10, Cont_Name = "UsersAdmin", AreaID = null },
                new ControllerSystem { ControllerID = 11, Cont_Name = "Corp_DBs", AreaID = null },
                new ControllerSystem { ControllerID = 12, Cont_Name = "Corp_Owner", AreaID = null },
                new ControllerSystem { ControllerID = 13, Cont_Name = "CorporateMasterList", AreaID = null },
                new ControllerSystem { ControllerID = 14, Cont_Name = "CPTDatas", AreaID = null },
                new ControllerSystem { ControllerID = 15, Cont_Name = "DBLists", AreaID = null },
                new ControllerSystem { ControllerID = 16, Cont_Name = "MDM_POS_ListName", AreaID = null },
                new ControllerSystem { ControllerID = 17, Cont_Name = "MDM_POS_Name_DBPOS_grp", AreaID = null },
                new ControllerSystem { ControllerID = 18, Cont_Name = "OwnerLists", AreaID = null },
                new ControllerSystem { ControllerID = 19, Cont_Name = "ACCategory", AreaID = 1 },
                new ControllerSystem { ControllerID = 20, Cont_Name = "ActionCodes", AreaID = 1 },
                new ControllerSystem { ControllerID = 21, Cont_Name = "ACTypes", AreaID = 1 },
                new ControllerSystem { ControllerID = 22, Cont_Name = "CodeMasterLists", AreaID = 1 },
                new ControllerSystem { ControllerID = 23, Cont_Name = "ADPMasters", AreaID = 2 },
                new ControllerSystem { ControllerID = 24, Cont_Name = "Edgemed_Logons", AreaID = 2 },
                new ControllerSystem { ControllerID = 25, Cont_Name = "BI_DB_FvP_Access", AreaID = 3 },
                new ControllerSystem { ControllerID = 26, Cont_Name = "Manager_Master", AreaID = 3 },
                new ControllerSystem { ControllerID = 27, Cont_Name = "Manager_Type", AreaID = 3 },
                new ControllerSystem { ControllerID = 28, Cont_Name = "Menus", AreaID = null }
           );

            context.Actions.AddOrUpdate(ac => ac.Act_Name,
                /*Corp_DBs Controller*/
                new ActionSystem { ActionID = 2, Act_Name = "Index", ControllerID = 11 },
                new ActionSystem { ActionID = 3, Act_Name = "Read", ControllerID = 11 },
                new ActionSystem { ActionID = 4, Act_Name = "Update", ControllerID = 11 },
                new ActionSystem { ActionID = 5, Act_Name = "Corp_DBs_Release", ControllerID = 11 },
                new ActionSystem { ActionID = 6, Act_Name = "Create", ControllerID = 11 },

                /*CorporateMasterList Controller*/
                new ActionSystem { ActionID = 7, Act_Name = "Index", ControllerID = 13 },
                new ActionSystem { ActionID = 8, Act_Name = "CorporateMasterLists_Read", ControllerID = 13 },
                new ActionSystem { ActionID = 9, Act_Name = "CorporateMasterLists_Update", ControllerID = 13 },
                new ActionSystem { ActionID = 10, Act_Name = "CorporateMasterLists_Create", ControllerID = 13 },

                /*CPTData Controller*/
                new ActionSystem { ActionID = 11, Act_Name = "Index", ControllerID = 14 },
                new ActionSystem { ActionID = 12, Act_Name = "Read", ControllerID = 14 },
                new ActionSystem { ActionID = 13, Act_Name = "Create_CPT", ControllerID = 14 },
                new ActionSystem { ActionID = 14, Act_Name = "Update", ControllerID = 14 },

                /*MDM_POS_ListName Controller*/
                new ActionSystem { ActionID = 15, Act_Name = "Index", ControllerID = 16 },
                new ActionSystem { ActionID = 16, Act_Name = "Read", ControllerID = 16 },
                new ActionSystem { ActionID = 17, Act_Name = "Update", ControllerID = 16 },
                new ActionSystem { ActionID = 18, Act_Name = "Create_POS", ControllerID = 16 },

                /*MDM_POS_NAME_DBPOS_grp Controller*/
                new ActionSystem { ActionID = 19, Act_Name = "Index", ControllerID = 17 },
                new ActionSystem { ActionID = 20, Act_Name = "Read", ControllerID = 17 },
                new ActionSystem { ActionID = 21, Act_Name = "Update_Entity", ControllerID = 17 },
                new ActionSystem { ActionID = 22, Act_Name = "Create_Entity", ControllerID = 17 },

                /*ACCategory Controller*/
                new ActionSystem { ActionID = 23, Act_Name = "Index", ControllerID = 19 },
                new ActionSystem { ActionID = 24, Act_Name = "Read_Category", ControllerID = 19 },
                new ActionSystem { ActionID = 25, Act_Name = "Create_Category", ControllerID = 19 },
                new ActionSystem { ActionID = 26, Act_Name = "Update_Category", ControllerID = 19 },

                /*ActionCode Controller*/
                new ActionSystem { ActionID = 27, Act_Name = "Index", ControllerID = 20 },
                new ActionSystem { ActionID = 28, Act_Name = "Read_ActionCode", ControllerID = 20 },
                new ActionSystem { ActionID = 29, Act_Name = "Create_ActionCode", ControllerID = 20 },
                new ActionSystem { ActionID = 30, Act_Name = "Update_ActionCode", ControllerID = 20 },

                /*ADPMaster Controller*/
                new ActionSystem { ActionID = 31, Act_Name = "Index", ControllerID = 23 },
                new ActionSystem { ActionID = 32, Act_Name = "Read_Adp", ControllerID = 23 },
                new ActionSystem { ActionID = 33, Act_Name = "EdgeMedLogonsforAdp", ControllerID = 23 },
                new ActionSystem { ActionID = 34, Act_Name = "Update_Adp", ControllerID = 23 },

                /*BI_DB_Fvp_Access Controller*/
                new ActionSystem { ActionID = 35, Act_Name = "Index", ControllerID = 25 },
                new ActionSystem { ActionID = 36, Act_Name = "Read_GroupByManager", ControllerID = 25 },
                new ActionSystem { ActionID = 37, Act_Name = "Read_BI_DB_FvPByManager", ControllerID = 25 },
                new ActionSystem { ActionID = 38, Act_Name = "Update_BI_DB_FvP", ControllerID = 25 },
                new ActionSystem { ActionID = 39, Act_Name = "Create_BI_DB_FvP", ControllerID = 25 },

                /*Edgemed_Logons Controller*/
                new ActionSystem { ActionID = 40, Act_Name = "EdgeMedLogonsforAdp", ControllerID = 24 },
                new ActionSystem { ActionID = 41, Act_Name = "UpdateEdgemed", ControllerID = 24 },
                new ActionSystem { ActionID = 42, Act_Name = "CreateEdgemed", ControllerID = 24 },

                /*Corp_Owner Controller*/
                new ActionSystem { ActionID = 43, Act_Name = "Owners", ControllerID = 12 },
                new ActionSystem { ActionID = 44, Act_Name = "Corporate", ControllerID = 12 },
                new ActionSystem { ActionID = 45, Act_Name = "Update", ControllerID = 12 },
                new ActionSystem { ActionID = 46, Act_Name = "Add_Corp", ControllerID = 12 },
                new ActionSystem { ActionID = 51, Act_Name = "Index", ControllerID = 12 },

                /*OwnerList Controller*/
                new ActionSystem { ActionID = 47, Act_Name = "Read", ControllerID = 18 },
                new ActionSystem { ActionID = 48, Act_Name = "Update", ControllerID = 18 },
                new ActionSystem { ActionID = 49, Act_Name = "Create_Owner", ControllerID = 18 },
                new ActionSystem { ActionID = 50, Act_Name = "Index", ControllerID = 18 }
           );

            context.Permissions.AddOrUpdate(p => p.Active,
                new Permission { PermissionID = 1, ActionID = 7, Active = true },
                new Permission { PermissionID = 2, ActionID = 8, Active = true },
                new Permission { PermissionID = 3, ActionID = 9, Active = true },
                new Permission { PermissionID = 4, ActionID = 10, Active = true },
                new Permission { PermissionID = 5, ActionID = 2, Active = true },
                new Permission { PermissionID = 6, ActionID = 3, Active = true },
                new Permission { PermissionID = 7, ActionID = 4, Active = true },
                new Permission { PermissionID = 8, ActionID = 5, Active = true },
                new Permission { PermissionID = 9, ActionID = 6, Active = true },
                new Permission { PermissionID = 10, ActionID = 11, Active = true },
                new Permission { PermissionID = 11, ActionID = 12, Active = true },
                new Permission { PermissionID = 12, ActionID = 13, Active = true },
                new Permission { PermissionID = 13, ActionID = 14, Active = true },
                new Permission { PermissionID = 14, ActionID = 31, Active = true },
                new Permission { PermissionID = 15, ActionID = 32, Active = true },
                new Permission { PermissionID = 16, ActionID = 33, Active = true },
                new Permission { PermissionID = 17, ActionID = 34, Active = true },
                new Permission { PermissionID = 18, ActionID = 40, Active = true },
                new Permission { PermissionID = 19, ActionID = 41, Active = true },
                new Permission { PermissionID = 20, ActionID = 42, Active = true },
                new Permission { PermissionID = 21, ActionID = 27, Active = true },
                new Permission { PermissionID = 22, ActionID = 28, Active = true },
                new Permission { PermissionID = 23, ActionID = 29, Active = true },
                new Permission { PermissionID = 24, ActionID = 30, Active = true },
                new Permission { PermissionID = 25, ActionID = 35, Active = true },
                new Permission { PermissionID = 26, ActionID = 36, Active = true },
                new Permission { PermissionID = 27, ActionID = 37, Active = true },
                new Permission { PermissionID = 28, ActionID = 38, Active = true },
                new Permission { PermissionID = 29, ActionID = 39, Active = true },
                new Permission { PermissionID = 30, ActionID = 15, Active = true },
                new Permission { PermissionID = 31, ActionID = 16, Active = true },
                new Permission { PermissionID = 32, ActionID = 17, Active = true },
                new Permission { PermissionID = 33, ActionID = 18, Active = true },
                new Permission { PermissionID = 34, ActionID = 19, Active = true },
                new Permission { PermissionID = 35, ActionID = 20, Active = true },
                new Permission { PermissionID = 36, ActionID = 21, Active = true },
                new Permission { PermissionID = 37, ActionID = 22, Active = true },
                new Permission { PermissionID = 38, ActionID = 46, Active = true },
                new Permission { PermissionID = 39, ActionID = 44, Active = true },
                new Permission { PermissionID = 40, ActionID = 43, Active = true },
                new Permission { PermissionID = 41, ActionID = 45, Active = true },
                new Permission { PermissionID = 42, ActionID = 49, Active = true },
                new Permission { PermissionID = 43, ActionID = 47, Active = true },
                new Permission { PermissionID = 44, ActionID = 48, Active = true },
                new Permission { PermissionID = 45, ActionID = 50, Active = true },
                new Permission { PermissionID = 46, ActionID = 51, Active = true }
            );
        }
    }
}
