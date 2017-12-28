namespace Quorra.Library
{
    /// <summary>
    /// Pouzivatelska rola v systeme.
    /// </summary>
    public enum QUserRole
    {
        /// <summary>
        /// Vlastnik produktu (projektu).
        /// </summary>
        ProductOwner = 0,
        /// <summary>
        /// Projektovy manazer. Riadi a organizuje cas, zdroje a financie na projekte.
        /// </summary>
        ProjectManager = 1,
        /// <summary>
        /// Vyvojar specializujuci sa na aplikacnu a biznis logiku.
        /// </summary>
        BackendDeveloper = 2,
        /// <summary>
        /// Vyvojar specializujuci sa na dizajn a UX.
        /// </summary>
        FrontendDeveloper = 3,
        /// <summary>
        /// Systemovy administrator dohliada na SW a HW pouzivany pri vyvoji.
        /// </summary>
        SystemAdministrator = 4,
        /// <summary>
        /// Akykolvek uzivatel, ktory testuje produkt.
        /// </summary>
        Tester = 5
    }
}