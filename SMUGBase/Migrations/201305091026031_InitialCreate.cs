namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mugusers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        OAuthToken = c.String(),
                        OAuthSecret = c.String(),
                        SyncFolder = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Gallerys",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        GalleryName = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RowId)
                .ForeignKey("dbo.Mugusers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Gallerys", new[] { "UserId" });
            DropForeignKey("dbo.Gallerys", "UserId", "dbo.Mugusers");
            DropTable("dbo.Gallerys");
            DropTable("dbo.Mugusers");
        }
    }
}
