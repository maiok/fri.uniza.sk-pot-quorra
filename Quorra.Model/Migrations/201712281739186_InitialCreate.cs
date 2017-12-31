namespace Quorra.Model.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QProjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProductOwnerId = c.Int(nullable: false),
                        EstimatedEnd = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QUsers", t => t.ProductOwnerId, cascadeDelete: true)
                .Index(t => t.ProductOwnerId);
            
            CreateTable(
                "dbo.QUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        ResponsibleUserId = c.Int(nullable: false),
                        AssignedUserId = c.Int(),
                        ProjectId = c.Int(),
                        IsPrivate = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        EstimatedEnd = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QUsers", t => t.AssignedUserId)
                .ForeignKey("dbo.QProjects", t => t.ProjectId)
                .ForeignKey("dbo.QUsers", t => t.ResponsibleUserId, cascadeDelete: true)
                .Index(t => t.ResponsibleUserId)
                .Index(t => t.AssignedUserId)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QTasks", "ResponsibleUserId", "dbo.QUsers");
            DropForeignKey("dbo.QTasks", "ProjectId", "dbo.QProjects");
            DropForeignKey("dbo.QTasks", "AssignedUserId", "dbo.QUsers");
            DropForeignKey("dbo.QProjects", "ProductOwnerId", "dbo.QUsers");
            DropIndex("dbo.QTasks", new[] { "ProjectId" });
            DropIndex("dbo.QTasks", new[] { "AssignedUserId" });
            DropIndex("dbo.QTasks", new[] { "ResponsibleUserId" });
            DropIndex("dbo.QProjects", new[] { "ProductOwnerId" });
            DropTable("dbo.QTasks");
            DropTable("dbo.QUsers");
            DropTable("dbo.QProjects");
        }
    }
}
