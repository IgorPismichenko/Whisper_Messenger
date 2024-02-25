using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whisper_Messenger.Models
{
    [DataContract]
    public class Chat
    {
        [IgnoreDataMember]
        public Visibility visibleText { get; set; }
        [IgnoreDataMember]
        public Visibility visibleMedia { get; set; }
        [DataMember]
        public string? date { get; set; }
        [DataMember]
        public string? chatContact { get; set; }
        [DataMember]
        public string? message { get; set; }
        [DataMember]
        public byte[]? media { get; set; }
    }
}
