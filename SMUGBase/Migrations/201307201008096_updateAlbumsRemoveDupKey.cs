namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAlbumsRemoveDupKey : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Albums", "CategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Albums", "CategoryId", c => c.Int(nullable: false));
        }
    }
}
