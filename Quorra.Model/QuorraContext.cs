using System.Data.Entity;

namespace Quorra.Model
{
    public class QuorraContext : DbContext
    {
        public QuorraContext() : base("name=QuorraDatabase")
        {
        }

        public DbSet<QUser> QUsers { get; set; }
        public DbSet<QProject> QProjects { get; set; }
        public DbSet<QTask> QTasks { get; set; }
    }
}