namespace Quorra.Library
{
    /// <summary>
    /// Trieda predstavuje objekt prihlaseneho pouzivatela v chate.
    /// </summary>
    public class ConnectedClient
    {
        /// <summary>
        /// Toto je instancia klienta, ktori sa pripoji do chatu,
        /// na zaklade toho ho viem spatne kontaktovat.
        /// </summary>
        public IClient Connection;
        
        /// <summary>
        /// Nickname
        /// </summary>
        public string UserName { get; set; }
    }
}