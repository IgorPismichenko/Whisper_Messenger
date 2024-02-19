using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Whisper_Messenger.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string? login { get; set; }
        [DataMember]
        public string? password { get; set; }
        [DataMember]
        public string? phone { get; set; }
        [DataMember]
        public string? command { get; set; }
        [DataMember]
        public string? mess { get; set; }
        [DataMember]
        public string? contact { get; set; }
        [DataMember]
        public List<string>? chat { get; set; }
        [DataMember]
        public byte[] avatar { get; set; }
        [DataMember]
        public List<Profile>? profile { get; set; }
        [IgnoreDataMember]
        public BitmapImage image { get; set; }
        public override string ToString()
        {
            return contact;
        }
    }
    [DataContract]
    public class Profile
    {
        [DataMember]
        public string? login { get; set; }
        [DataMember]
        public byte[]? avatar { get; set; }
        [DataMember]
        public string? phone { get; set; }
    }
}
