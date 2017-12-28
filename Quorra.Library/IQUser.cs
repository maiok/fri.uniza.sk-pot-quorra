namespace Quorra.Library
{
    /// <summary>
    /// Rozhranie uzivatela.
    /// </summary>
    public interface IQUser
    {
        /// <summary>
        /// Id uzivatela - generovane v DB.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Meno uzivatela. V pripade, ze sa jedna o spolocnost, tak jej nazov.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Pracovna rola.
        /// </summary>
        QUserRole UserRole { get; set; }
    }
}