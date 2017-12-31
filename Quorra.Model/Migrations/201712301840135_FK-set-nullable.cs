namespace Quorra.Model.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FKsetnullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QProjects", "ProductOwnerId", "dbo.QUsers");
            DropForeignKey("dbo.QTasks", "ResponsibleUserId", "dbo.QUsers");
            DropIndex("dbo.QProjects", new[] { "ProductOwnerId" });
            DropIndex("dbo.QTasks", new[] { "ResponsibleUserId" });
            AlterColumn("dbo.QProjects", "ProductOwnerId", c => c.Int());
            AlterColumn("dbo.QTasks", "ResponsibleUserId", c => c.Int());
            CreateIndex("dbo.QProjects", "ProductOwnerId");
            CreateIndex("dbo.QTasks", "ResponsibleUserId");
            AddForeignKey("dbo.QProjects", "ProductOwnerId", "dbo.QUsers", "Id");
            AddForeignKey("dbo.QTasks", "ResponsibleUserId", "dbo.QUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QTasks", "ResponsibleUserId", "dbo.QUsers");
            DropForeignKey("dbo.QProjects", "ProductOwnerId", "dbo.QUsers");
            DropIndex("dbo.QTasks", new[] { "ResponsibleUserId" });
            DropIndex("dbo.QProjects", new[] { "ProductOwnerId" });
            AlterColumn("dbo.QTasks", "ResponsibleUserId", c => c.Int(nullable: false));
            AlterColumn("dbo.QProjects", "ProductOwnerId", c => c.Int(nullable: false));
            CreateIndex("dbo.QTasks", "ResponsibleUserId");
            CreateIndex("dbo.QProjects", "ProductOwnerId");
            AddForeignKey("dbo.QTasks", "ResponsibleUserId", "dbo.QUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.QProjects", "ProductOwnerId", "dbo.QUsers", "Id", cascadeDelete: true);
        }
    }
}
