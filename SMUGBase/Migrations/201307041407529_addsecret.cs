namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsecret : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mugusers", "OAuthTokenSecret", c => c.String());
            DropColumn("dbo.Mugusers", "ONounce");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mugusers", "ONounce", c => c.String());
            DropColumn("dbo.Mugusers", "OAuthTokenSecret");
        }
    }
}
