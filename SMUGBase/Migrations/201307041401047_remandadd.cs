namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remandadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mugusers", "ONounce", c => c.String());
            DropColumn("dbo.Mugusers", "OAuthSecret");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mugusers", "OAuthSecret", c => c.String());
            DropColumn("dbo.Mugusers", "ONounce");
        }
    }
}
