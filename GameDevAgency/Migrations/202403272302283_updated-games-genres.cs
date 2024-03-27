namespace GameDevAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedgamesgenres : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        GameId = c.Int(nullable: false, identity: true),
                        GameVersion = c.String(),
                        GameName = c.String(),
                        GameDescription = c.String(),
                        GameReleaseDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GameId);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreId = c.Int(nullable: false, identity: true),
                        GenreName = c.String(),
                    })
                .PrimaryKey(t => t.GenreId);
            
            CreateTable(
                "dbo.GenreGames",
                c => new
                    {
                        Genre_GenreId = c.Int(nullable: false),
                        Game_GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Genre_GenreId, t.Game_GameId })
                .ForeignKey("dbo.Genres", t => t.Genre_GenreId, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.Game_GameId, cascadeDelete: true)
                .Index(t => t.Genre_GenreId)
                .Index(t => t.Game_GameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GenreGames", "Game_GameId", "dbo.Games");
            DropForeignKey("dbo.GenreGames", "Genre_GenreId", "dbo.Genres");
            DropIndex("dbo.GenreGames", new[] { "Game_GameId" });
            DropIndex("dbo.GenreGames", new[] { "Genre_GenreId" });
            DropTable("dbo.GenreGames");
            DropTable("dbo.Genres");
            DropTable("dbo.Games");
        }
    }
}
