using System;
using System.Runtime.Serialization;

namespace Quorra.Library
{
    /// <summary>
    /// Trieda ako kontajner spravy v chate.
    /// </summary>
    [DataContract]
    public class Message
    {
        public Message()
        {
            Time = DateTime.Now;
        }

        /// <summary>
        /// Odosielatel
        /// </summary>
        [DataMember]
        public string FromUser { get; set; }

        /// <summary>
        /// Adresat
        /// </summary>
        [DataMember]
        public string ToUser { get; set; }

        /// <summary>
        /// Cas odoslania
        /// </summary>
        [DataMember]
        public DateTime Time { get; set; }

        /// <summary>
        /// Sprava.
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        public override string ToString()
        {
            var toUser = (ToUser != null && ToUser.Trim() != "") ? ToUser : "Všetci";
            return $"{Time:yyyy-MM-dd HH:mm:ss}: {FromUser}->{toUser}: {Text}"; 
        }
    }
}