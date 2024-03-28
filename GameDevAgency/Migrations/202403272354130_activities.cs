namespace GameDevAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        ActivityName = c.String(),
                        ActivityDescription = c.String(),
                        ActivityPriority = c.String(),
                        ActivityStatus = c.String(),
                        ActivityDueDate = c.DateTime(nullable: false),
                        ActivityEstimates = c.String(),
                        GameId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .Index(t => t.GameId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activities", "GameId", "dbo.Games");
            DropForeignKey("dbo.Activities", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropIndex("dbo.Activities", new[] { "GameId" });
            DropTable("dbo.Activities");
        }
    }
}
