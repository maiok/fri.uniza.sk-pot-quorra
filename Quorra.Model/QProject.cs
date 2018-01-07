using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quorra.Library;

namespace Quorra.Model
{
    /// <summary>
    /// Projekt ako objekt v DB
    /// </summary>
    public class QProject : IQProject
    {
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
            return $"Project: {Name}, ProductOwner: {ProductOwner}, EstimatedEnd: {EstimatedEnd}";
        }
    }
}
