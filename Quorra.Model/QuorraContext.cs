using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Quorra.Library;

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

        /// <summary>
        /// Metoda na vlozenie pouzivatela do DB.
        /// </summary>
        /// <param name="user"></param>
        public void InsertUser(QUser user)
        {
            QUsers.Add(user);
            SaveChanges();
        }

        public void UpdateUser(QUser user)
        {
            if (user != null)
            {
                QUser userToUpdate = GetUserById(user.Id);
                QUsers.Attach(userToUpdate);
                var entry = Entry(userToUpdate);

                // Property na update
                entry.Property(u => u.Name).IsModified = true;
                entry.Property(u => u.UserRole).IsModified = true;

                SaveChanges();
            }
        }

        public void UpdateProject(QProject project)
        {
            if (project != null)
            {
                QProject projectToUpdate = GetProjectById(project.Id);
                QProjects.Attach(projectToUpdate);
                var entry = Entry(projectToUpdate);

                // Property na update
                entry.Property(p => p.Name).IsModified = true;
                entry.Property(p => p.ProductOwnerId).IsModified = true;
                entry.Property(p => p.EstimatedEnd).IsModified = true;

                SaveChanges();
            }
        }

        public void UpdateTask(QTask task)
        {
            if (task != null)
            {
                QTask taskToUpdate = GetTaskById(task.Id);
                QTasks.Attach(taskToUpdate);
                var entry = Entry(taskToUpdate);

                entry.Property(t => t.Title).IsModified = true;
                entry.Property(t => t.Description).IsModified = true;
                entry.Property(t => t.ResponsibleUserId).IsModified = true;
                entry.Property(t => t.AssignedUserId).IsModified = true;
                entry.Property(t => t.ProjectId).IsModified = true;
                entry.Property(t => t.IsPrivate).IsModified = true;
                entry.Property(t => t.EstimatedEnd).IsModified = true;

                SaveChanges();
            }
        }

        public void RemoveUser(QUser user)
        {
            // EF nepodporuje Cascade Nullable, takze to potrebujem osetrit predtym ako sa zavola Remove
            // Cize tabulky, ktore mi referencuju Quser, musim nastavit tuto referenciu na NULL
            QProjects.Where(p => p.ProductOwnerId == user.Id).Load();

            QUsers.Remove(user);
            SaveChanges();
        }

        public void RemoveProject(QProject project)
        {
            // Ulohy my referencuju projekt, ale ja po zmazani projektu chcem, aby mi vymazalo aj vsetky
            // ulohy, ktore ma projekt priradene, preto tu nenastavujem NULL
            QProjects.Remove(project);
            SaveChanges();
        }

        public void RemoveTask(QTask task)
        {
            QTasks.Remove(task);
            SaveChanges();
        }

        public QUser GetUserById(int id)
        {
            try
            {
                return QUsers.Find(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public QProject GetProjectById(int id)
        {
            try
            {
                return QProjects.Find(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private QTask GetTaskById(int id)
        {
            try
            {
                return QTasks.Find(id);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<QUser> ApplyFilterUsers(string userName, List<QUserRole> userRoles)
        {
            // Full select
            var query = from u in QUsers
                select u;
            // Where
            // Podla mena
            if (userName != null)
            {
                query = query.Where(u => u.Name.Contains(userName));
            }
            // Podla role
            if (userRoles.Count > 0)
            {
                query = query.Where(u => userRoles.Contains((QUserRole) u.UserRole));
            }
            // OrderBY
            query = query.OrderBy(u => u.Name);

            return query.ToList();
        }

        public List<QProject> ApplyFilterProjects(string projectName, string productOwnerName,
            DateTime? estimatedEndFrom, DateTime? estimatedEndTo)
        {
            var query = from p in QProjects
                select p;

            if (projectName != null)
            {
                query = query.Where(p => p.Name.Contains(projectName));
            }
            if (productOwnerName != null)
            {
                query = query.Where(p => p.ProductOwner.Name.Contains(productOwnerName));
            }
            if (estimatedEndFrom != null)
            {
                query = query.Where(p => p.EstimatedEnd >= estimatedEndFrom);
            }
            if (estimatedEndTo != null)
            {
                query = query.Where(p => p.EstimatedEnd <= estimatedEndTo);
            }

            query.OrderBy(p => p.Name);

            return query.ToList();
        }

        public void InsertProject(QProject project)
        {
            QProjects.Add(project);
            SaveChanges();
        }

        public void InsertTask(QTask task)
        {
            task.Created = DateTime.Now;
            QTasks.Add(task);
            SaveChanges();
        }

        public IQueryable<QUser> GetUsers()
        {
            return QUsers;
        }

        public IQueryable<QProject> GetProjects()
        {
            return QProjects;
        }

        public IQueryable<QTask> GetTasks()
        {
            return QTasks;
        }
    }
}