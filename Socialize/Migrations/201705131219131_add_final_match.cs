namespace Socialize.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_final_match : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinalMatchLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsersId = c.String(),
                        Created = c.DateTime(nullable: false),
                        MatchStrength = c.String(),
                        Locations = c.String(),
                        IsAccepted = c.Boolean(nullable: false),
                        Factors = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FinalMatchLogs");
        }
    }
}
