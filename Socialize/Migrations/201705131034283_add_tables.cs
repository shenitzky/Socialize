namespace Socialize.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchRequestLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Created = c.DateTime(nullable: false),
                        MatchFactors = c.String(),
                        Location = c.String(),
                        maxDistance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OptionalMatchLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Created = c.DateTime(nullable: false),
                        MatchedFactors = c.String(),
                        MatchStrength = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OptionalMatchLogs");
            DropTable("dbo.MatchRequestLogs");
        }
    }
}
