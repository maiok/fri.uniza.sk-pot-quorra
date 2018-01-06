namespace Quorra.Model.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class projectesimatedtimenullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QProjects", "EstimatedEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QProjects", "EstimatedEnd", c => c.DateTime(nullable: false));
        }
    }
}
