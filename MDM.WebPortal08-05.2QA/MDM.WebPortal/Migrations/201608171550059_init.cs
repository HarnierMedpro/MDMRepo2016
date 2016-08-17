namespace MDM.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActionSystems",
                c => new
                    {
                        ActionID = c.Int(nullable: false, identity: true),
                        Act_Name = c.String(nullable: false, maxLength: 50),
                        ControllerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActionID)
                .ForeignKey("dbo.ControllerSystems", t => t.ControllerID, cascadeDelete: true)
                .Index(t => t.ControllerID);
            
            CreateTable(
                "dbo.ControllerSystems",
                c => new
                    {
                        ControllerID = c.Int(nullable: false, identity: true),
                        Cont_Name = c.String(nullable: false, maxLength: 50),
                        AreaID = c.Int(),
                    })
                .PrimaryKey(t => t.ControllerID)
                .ForeignKey("dbo.AreaSystems", t => t.AreaID)
                .Index(t => t.AreaID);
            
            CreateTable(
                "dbo.AreaSystems",
                c => new
                    {
                        AreaID = c.Int(nullable: false, identity: true),
                        AreaName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.AreaID);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        PermissionID = c.Int(nullable: false, identity: true),
                        ActionID = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PermissionID)
                .ForeignKey("dbo.ActionSystems", t => t.ActionID, cascadeDelete: true)
                .Index(t => t.ActionID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Active = c.Boolean(),
                        Priority = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        MenuID = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        ActionID = c.Int(),
                        Title = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.MenuID)
                .ForeignKey("dbo.ActionSystems", t => t.ActionID)
                .Index(t => t.ActionID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Active = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ApplicationRolePermissions",
                c => new
                    {
                        ApplicationRole_Id = c.String(nullable: false, maxLength: 128),
                        Permission_PermissionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationRole_Id, t.Permission_PermissionID })
                .ForeignKey("dbo.AspNetRoles", t => t.ApplicationRole_Id, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.Permission_PermissionID, cascadeDelete: true)
                .Index(t => t.ApplicationRole_Id)
                .Index(t => t.Permission_PermissionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Menus", "ActionID", "dbo.ActionSystems");
            DropForeignKey("dbo.ApplicationRolePermissions", "Permission_PermissionID", "dbo.Permissions");
            DropForeignKey("dbo.ApplicationRolePermissions", "ApplicationRole_Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.Permissions", "ActionID", "dbo.ActionSystems");
            DropForeignKey("dbo.ControllerSystems", "AreaID", "dbo.AreaSystems");
            DropForeignKey("dbo.ActionSystems", "ControllerID", "dbo.ControllerSystems");
            DropIndex("dbo.ApplicationRolePermissions", new[] { "Permission_PermissionID" });
            DropIndex("dbo.ApplicationRolePermissions", new[] { "ApplicationRole_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Menus", new[] { "ActionID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Permissions", new[] { "ActionID" });
            DropIndex("dbo.ControllerSystems", new[] { "AreaID" });
            DropIndex("dbo.ActionSystems", new[] { "ControllerID" });
            DropTable("dbo.ApplicationRolePermissions");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Menus");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Permissions");
            DropTable("dbo.AreaSystems");
            DropTable("dbo.ControllerSystems");
            DropTable("dbo.ActionSystems");
        }
    }
}
