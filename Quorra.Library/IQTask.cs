using System;

namespace Quorra.Library
{
    /// <summary>
    /// Rozhranie pre ulohu v projekte.
    /// </summary>
    public interface IQTask
    {
        /// <summary>
        /// Unikatne ID ulohy.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Nazov ulohy.
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Blizsi popis k ulohe.
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// ID pouzivatela, ktora je zodpovedna za ulohu.
        /// </summary>
        int? ResponsibleUserId { get; set; }
        /// <summary>
        /// ID pouzivatela, ktora je zodpovedna za vypracovanie ulohy.
        /// </summary>
        int? AssignedUserId { get; set; }
        /// <summary>
        /// ID projektu, ku ktoremu je uloha priradena (volitelne).
        /// </summary>
        int? ProjectId { get; set; }
        /// <summary>
        /// Je to sukromna uloha?
        /// </summary>
        bool IsPrivate { get; set; }
        /// <summary>
        /// Datum vytvorenia.
        /// </summary>
        DateTime Created { get; set; }
        /// <summary>
        /// Predpokladany datum ukoncenia.
        /// </summary>
        DateTime? EstimatedEnd { get; set; }
    }
}