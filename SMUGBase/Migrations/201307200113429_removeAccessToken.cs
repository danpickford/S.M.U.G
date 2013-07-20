namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeAccessToken : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Mugusers", "AccessToken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mugusers", "AccessToken", c => c.String());
        }
    }
}
