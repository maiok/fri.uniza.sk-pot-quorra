using System.ComponentModel.DataAnnotations;
using Quorra.Library;

namespace Quorra.Model
{
    public class QUser : IQUser
    {
        /// <summary>
        /// Konstruktor pre vytvorenie objektu pouzivatela.
        /// </summary>
        public QUser()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public QUserRole UserRole { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, UserRole: {UserRole}";
        }
    }
}