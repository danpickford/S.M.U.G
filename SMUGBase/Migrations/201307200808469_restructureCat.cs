namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class restructureCat : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Gallerys", "UserId", "dbo.Mugusers");
            DropIndex("dbo.Gallerys", new[] { "UserId" });
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RowId)
                .ForeignKey("dbo.Mugusers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            DropTable("dbo.Gallerys");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Gallerys",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        GalleryName = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RowId);
            
            DropIndex("dbo.Categories", new[] { "UserId" });
            DropForeignKey("dbo.Categories", "UserId", "dbo.Mugusers");
            DropTable("dbo.Categories");
            CreateIndex("dbo.Gallerys", "UserId");
            AddForeignKey("dbo.Gallerys", "UserId", "dbo.Mugusers", "UserId", cascadeDelete: true);
        }
    }
}
