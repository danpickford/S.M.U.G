namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mugusers", "AccessToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mugusers", "AccessToken");
        }
    }
}
