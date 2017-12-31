using System;

namespace Quorra.Library
{
    /// <summary>
    /// Rozhranie projektu.
    /// </summary>
    public interface IQProject
    {
        /// <summary>
        /// Id projektu.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Nazov projektu.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Vlastnik projektu, ktory to financuje.
        /// </summary>
        int? ProductOwnerId { get; set; }
        /// <summary>
        /// Odhadovany datum ukoncenia projektu.
        /// </summary>
        DateTime? EstimatedEnd { get; set; }
    }
}