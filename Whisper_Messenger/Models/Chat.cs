using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whisper_Messenger.Models
{
    [DataContract]
    public class Chat: INotifyPropertyChanged
    {
        [IgnoreDataMember]
        public Visibility visibleText { get; set; }
        [IgnoreDataMember]
        public Visibility visibleMedia { get; set; }

        [IgnoreDataMember]
        public Visibility VisibleText
        { 
            get
            {
                return visibleText;
            }
            set
            {
                visibleText = value;
                RaisePropertyChanged(nameof(VisibleText));
            }
        }

        [IgnoreDataMember]
        public Visibility VisibleMedia
        {
            get
            {
                return visibleMedia;
            }
            set
            {
                visibleMedia = value;
                RaisePropertyChanged(nameof(VisibleMedia));
            }
        }
        [DataMember]
        public string? date { get; set; }
        [DataMember]
        public string? chatContact { get; set; }
        [DataMember]
        public string? message { get; set; }
        [DataMember]
        public byte[]? media { get; set; }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
