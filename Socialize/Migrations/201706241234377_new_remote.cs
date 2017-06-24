namespace Socialize.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class new_remote : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AvatarImgs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ImgUrl = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Factors",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(maxLength: 128),
                    Class = c.String(),
                    FinalMatch_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.FinalMatches", t => t.FinalMatch_Id)
                .Index(t => t.UserId)
                .Index(t => t.FinalMatch_Id);

            CreateTable(
                "dbo.SubClasses",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    ImgUrl = c.String(),
                    Factor_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Factors", t => t.Factor_Id)
                .Index(t => t.Factor_Id);

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

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    FirstName = c.String(),
                    LastName = c.String(),
                    Description = c.String(),
                    ImgUrl = c.String(),
                    Age = c.Int(nullable: false),
                    Premium = c.Boolean(nullable: false),
                    DeclineNum = c.Int(nullable: false),
                    AcceptNum = c.Int(nullable: false),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.FinalMatches",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Created = c.DateTime(nullable: false),
                    Locations = c.String(),
                    IsAccepted = c.Boolean(nullable: false),
                    ApplicationUser_Id = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.FinalMatches", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Factors", "FinalMatch_Id", "dbo.FinalMatches");
            DropForeignKey("dbo.Factors", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.SubClasses", "Factor_Id", "dbo.Factors");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.FinalMatches", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SubClasses", new[] { "Factor_Id" });
            DropIndex("dbo.Factors", new[] { "FinalMatch_Id" });
            DropIndex("dbo.Factors", new[] { "UserId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.FinalMatches");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.OptionalMatchLogs");
            DropTable("dbo.MatchRequestLogs");
            DropTable("dbo.FinalMatchLogs");
            DropTable("dbo.SubClasses");
            DropTable("dbo.Factors");
            DropTable("dbo.AvatarImgs");
        }
    }
}
