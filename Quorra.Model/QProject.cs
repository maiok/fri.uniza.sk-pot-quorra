using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quorra.Library;

namespace Quorra.Model
{
    public class QProject : IQProject
    {
        public QProject()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ProductOwnerId { get; set; }
        public DateTime? EstimatedEnd { get; set; }

        //  Vytvorenie vztah 1:1 k pouzivatelovi (vlastnikovi projektu)
        [ForeignKey("ProductOwnerId")]
        public virtual QUser ProductOwner { get; set; }

        public override string ToString()
        {
            return $"Project: {Name}, ProductOwner: {ProductOwner.Name}, EstimatedEnd: {EstimatedEnd}";
        }
    }
}
