namespace SMUGBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlogged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mugusers", "LoggedUser", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mugusers", "LoggedUser");
        }
    }
}
