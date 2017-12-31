using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quorra.Library;

namespace Quorra.Model
{
    public class QTask : IQTask
    {
        public QTask()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ResponsibleUserId { get; set; }
        public int? AssignedUserId { get; set; }
        public int? ProjectId { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime Created { get; set; }
        public DateTime? EstimatedEnd { get; set; }

        // Automaticke naplnenie atributov objektami s ID
        [ForeignKey("ResponsibleUserId")]
        public virtual QUser ResponsibleUser { get; set; }
        [ForeignKey("AssignedUserId")]
        public virtual QUser AssignedUser { get; set; }
        [ForeignKey("ProjectId")]
        public virtual QProject Project { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}, Description: {Description}, ResponsibleUser: {ResponsibleUser.Name}, " +
                   $"AssignedUser: {AssignedUser.Name}, Project: {Project.Name}";
        }
    }
}
