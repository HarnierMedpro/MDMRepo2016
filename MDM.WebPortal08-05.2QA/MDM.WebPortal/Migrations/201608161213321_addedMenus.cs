namespace MDM.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedMenus : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Menus", "ActionID", "dbo.ActionSystems");
            DropIndex("dbo.Menus", new[] { "ActionID" });
            DropTable("dbo.Menus");
        }
    }
}
