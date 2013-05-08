namespace S.M.U.G.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncFolderFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mugusers", "SyncFolder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mugusers", "SyncFolder");
        }
    }
}
