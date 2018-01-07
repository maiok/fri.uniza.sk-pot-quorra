using System;
using System.Runtime.Serialization;

namespace Quorra.Library
{
    [DataContract]
    public class Message
    {
        public Message()
        {
            Time = DateTime.Now;
        }

        [DataMember]
        public string FromUser { get; set; }

        [DataMember]
        public string ToUser { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public string Text { get; set; }

        public override string ToString()
        {
            var toUser = (ToUser != null && ToUser.Trim() != "") ? ToUser : "Všetci";
            return $"{Time:yyyy-MM-dd HH:mm:ss}: {FromUser}->{toUser}: {Text}"; 
        }
    }
}