using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Whisper_Messenger.Models
{
    [Serializable]
    [DataContract]
    public class User : INotifyPropertyChanged
    {
        public User() { }

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
        public string? data { get; set; }
        [DataMember]
        public string? path { get; set; }
        [DataMember]
        public string? isOnline { get; set; }
        [DataMember]
        public string? contact;
        [DataMember]
        public string? Contact
        {
            get
            {
                return contact;
            }
            set
            {
                contact = value;
                RaisePropertyChanged(nameof(Contact));
            }
        }
        [DataMember]
        public List<Chat>? chat { get; set; }
        [DataMember]
        public byte[] avatar { get; set; }
        [DataMember]
        public byte[] media { get; set; }
        [DataMember]
        public List<Profile>? profile { get; set; }
        [DataMember]
        public List<byte[]>? mediaList { get; set; }
        [XmlIgnore]
        public BitmapImage image;
        [XmlIgnore]
        public BitmapImage Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                RaisePropertyChanged(nameof(Image));
            }
        }
        [DataMember]
        public Chat? c { get; set; }
        [DataMember]
        public string? blocked { get; set; }
        public override string ToString()
        {
            return contact;
        }
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
