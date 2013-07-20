namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAlbums : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        AlbumName = c.String(),
                        Category_RowId = c.Int(),
                    })
                .PrimaryKey(t => t.RowId)
                .ForeignKey("dbo.Categories", t => t.Category_RowId)
                .Index(t => t.Category_RowId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Albums", new[] { "Category_RowId" });
            DropForeignKey("dbo.Albums", "Category_RowId", "dbo.Categories");
            DropTable("dbo.Albums");
        }
    }
}
