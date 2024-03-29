using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Whisper_Messenger.Commands;
using Whisper_Messenger.Models;
using Tulpep.NotificationWindow;
using System.Windows.Documents;

namespace Whisper_Messenger.ViewModels
{
    [DataContract]
    public class MainViewModel : INotifyPropertyChanged
    {
        [DataMember]
        public TcpSender sender;
        ManualResetEvent mRevent;
        ManualResetEvent mReventClose;
        Socket socket;
        public event Action MyEvent;
        public event Action MyEvent2;
        string temp;
        byte[] defImg;
        private PopupNotifier notifier = null;
        public MainViewModel(ManualResetEvent ev, ManualResetEvent evC)
        {
            mRevent = ev;
            mReventClose = evC;
            MyEvent += MyEventHandler;
            MyEvent2 += MyEventHandler2;
            CurrentLogin = "login";
            //CurrentContact = "";
            //CurrentPass = "password";
            CurrentPhone = "phone";
            CurrentSearch = "search";
            //Sms = "";
            //CurrentContact = "";
            IsButtonEnabled = true;
            sender = new TcpSender();
            sender.ReceiveMessage(socket, MyEvent2);
        }
        #region Fields&Properties
        string currentLogin;
        public string CurrentLogin
        {
            get
            {
                return currentLogin;
            }
            set
            {
                currentLogin = value;
                RaisePropertyChanged(nameof(CurrentLogin));
            }
        }

        string currentPassword;
        public string CurrentPass
        {
            get
            {
                return currentPassword;
            }
            set
            {
                currentPassword = value;
                RaisePropertyChanged(nameof(CurrentPass));
            }
        }

        string currentPhone;
        public string CurrentPhone
        {
            get
            {
                return currentPhone;
            }
            set
            {
                currentPhone = value;
                RaisePropertyChanged(nameof(CurrentPhone));
            }
        }
        string sms;
        public string Sms
        {
            get
            {
                return sms;
            }
            set
            {
                sms = value;
                RaisePropertyChanged(nameof(Sms));
            }
        }
        string currentSms;
        public string CurrentSms
        {
            get
            {
                return currentSms;
            }
            set
            {
                currentSms = value;
                RaisePropertyChanged(nameof(CurrentSms));
            }
        }

        string selectedContactNickname;

        public string SelectedContactNickname
        {
            get { return selectedContactNickname; }
            set
            {
                selectedContactNickname = value;
                RaisePropertyChanged(nameof(SelectedContactNickname));
            }
        }


        string currentContact;
        public string CurrentContact
        {
            get
            {
                return currentContact;
            }
            set
            {
                if (value == null)
                {
                    currentContact = temp;
                }
                else
                {
                    currentContact = value;
                }
                RaisePropertyChanged(nameof(CurrentContact));
            }
        }



        string currentPath;
        public string CurrentPath
        {
            get
            {
                return currentPath;
            }
            set
            {
                currentPath = value;
                RaisePropertyChanged(nameof(CurrentPath));
            }
        }

        string currentBlock;
        public string CurrentBlock
        {
            get
            {
                return currentBlock;
            }
            set
            {
                currentBlock = value;
                RaisePropertyChanged(nameof(CurrentBlock));
            }
        }
        string currentStatus;
        public string CurrentStatus
        {
            get
            {
                return currentStatus;
            }
            set
            {
                currentStatus = value;
                RaisePropertyChanged(nameof(CurrentStatus));
            }
        }

        string currentMediaPath;
        public string CurrentMediaPath
        {
            get
            {
                return currentMediaPath;
            }
            set
            {
                currentMediaPath = value;
                RaisePropertyChanged(nameof(CurrentMediaPath));
                byte[] img = GetImageBytes(currentMediaPath);
                CurrentMedia = ConvertBitmapFunc(img);
            }
        }

        string currentSearch;
        public string CurrentSearch
        {
            get
            {
                return currentSearch;
            }
            set
            {
                currentSearch = value;
                RaisePropertyChanged(nameof(CurrentSearch));
            }
        }

        BitmapImage currentUserAvatar;
        public BitmapImage CurrentUserAvatar
        {
            get
            {
                return currentUserAvatar;
            }
            set
            {
                currentUserAvatar = value;
                RaisePropertyChanged(nameof(CurrentUserAvatar));
            }
        }

        BitmapImage currentMedia;
        public BitmapImage CurrentMedia
        {
            get
            {
                return currentMedia;
            }
            set
            {
                currentMedia = value;
                RaisePropertyChanged(nameof(CurrentMedia));
            }
        }
        [DataMember]
        private ObservableCollection<User>? contacts = new ObservableCollection<User>();
        [DataMember]
        public ObservableCollection<User> Contacts
        {
            get
            {
                return contacts;
            }
            set
            {
                contacts = value;
                RaisePropertyChanged(nameof(Contacts));
            }
        }
        private ObservableCollection<Chat>? messages = new ObservableCollection<Chat>();

        public ObservableCollection<Chat> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                messages = value;
                RaisePropertyChanged(nameof(Messages));
            }
        }
        private ObservableCollection<BitmapImage>? images = new ObservableCollection<BitmapImage>();

        public ObservableCollection<BitmapImage> Images
        {
            get
            {
                return images;
            }
            set
            {
                images = value;
                RaisePropertyChanged(nameof(Images));
            }
        }

        private bool _isButtonEnabled;

        public bool IsButtonEnabled
        {
            get
            {
                return _isButtonEnabled;
            }
            set
            {
                _isButtonEnabled = value;
                RaisePropertyChanged(nameof(IsButtonEnabled));
            }
        }
        string selectedContactStatus;

        public string SelectedContactStatus
        {
            get { return selectedContactStatus; }
            set
            {
                selectedContactStatus = value;
                RaisePropertyChanged(nameof(SelectedContactStatus));
            }
        }
        string currentContactPhone;
        public string CurrentContactPhone
        {
            get
            {
                return currentContactPhone;
            }
            set
            {
                currentContactPhone = value;
                RaisePropertyChanged(nameof(CurrentContactPhone));
            }
        }
        BitmapImage currentAvatar;
        public BitmapImage CurrentAvatar
        {
            get
            {
                return currentAvatar;
            }
            set
            {
                currentAvatar = value;
                RaisePropertyChanged(nameof(CurrentAvatar));
            }
        }
        #endregion
        #region ICommands
        private DelegateCommand _RegCommand;
        private DelegateCommand _LogCommand;
        private DelegateCommand _SendCommand;
        private DelegateCommand _UpdProfileCommand;
        private DelegateCommand _ContactCommand;
        private DelegateCommand _ChangeCommand;
        private DelegateCommand _ReadProfileCommand;
        private DelegateCommand _DeleteProfileCommand;
        private DelegateCommand _SendFileCommand;
        private DelegateCommand _DeleteUserFromContact;
        private DelegateCommand _BlockContact;
        private DelegateCommand _UnblockContact;
        private DelegateCommand _CloseCommand;
        private DelegateCommand _DeleteSmsCommand;
        public ICommand RegButtonClick
        {
            get
            {
                if (_RegCommand == null)
                {
                    _RegCommand = new DelegateCommand(Reg, CanReg);
                }
                return _RegCommand;
            }
        }
        private void Reg(object o)
        {
            if (CurrentPhone.Length == 10)
            {
                defImg = GetImageBytes("photo_2023-12-18_16-19-13.jpg");
                User user = new User() { login = CurrentLogin, password = CurrentPass, phone = CurrentPhone, avatar = defImg, command = "Register" };
                sender.SendCommand(user, mRevent, MyEvent);
            }
            else
            {
                MessageBox.Show("Phone number must contain 10 digits");
            }
        }
        private bool CanReg(object o)
        {
            if (CurrentLogin == "" || CurrentPass == "" || CurrentPhone == "")
                return false;
            return true;
        }
        public ICommand LogButtonClick
        {
            get
            {
                if (_LogCommand == null)
                {
                    _LogCommand = new DelegateCommand(Log, CanLog);
                }
                return _LogCommand;
            }
        }
        private void Log(object o)
        {
            User user = new User() { login = CurrentLogin, password = CurrentPass, command = "Login" };
            sender.SendCommand(user, mRevent, MyEvent);
        }
        private bool CanLog(object o)
        {
            if (CurrentLogin == "" || CurrentPass == "")
                return false;
            return true;
        }

        public ICommand SendButtonClick
        {
            get
            {
                if (_SendCommand == null)
                {
                    _SendCommand = new DelegateCommand(Send, CanSend);
                }
                return _SendCommand;
            }
        }
        
        private void Send(object o)
        {
            if (CurrentBlock == "block")
            {
                MessageBox.Show("This contact was blocked. Unblock it to send messages!");
            }
            else if (CurrentContact == null || CurrentContact == "")
            {
                MessageBox.Show("Choose contact to send!");
            }
            else 
            {

                User user = new User();
                user.contact = CurrentContact;
                user.command = "Send";
                user.mess = Sms;
                user.data = DateTime.Now.Date.ToString();
                Chat c = new Chat();
                c.message = Sms;
                c.date = user.data;
                c.chatContact = CurrentLogin;
                c.VisibleText = Visibility.Visible;
                c.VisibleMedia = Visibility.Collapsed;
                Messages.Insert(0, c);
                Sms = "";
                sender.SendCommand(user, mRevent, MyEvent2);
            }
        }
        private bool CanSend(object o)
        {
            if (Sms == "")
                return false;
            return true;
        }

        public ICommand ContactButtonClick
        {
            get
            {
                if (_ContactCommand == null)
                {
                    _ContactCommand = new DelegateCommand(Search, CanSearch);
                }
                return _ContactCommand;
            }
        }
        private void Search(object o)
        {
            bool match = false;
            foreach (var c in Contacts)
            {
                if (c.phone == CurrentSearch)
                    match = true;
            }
            if (CurrentSearch == CurrentPhone)
            {
                MessageBox.Show("Can`t use your number!");
            }
            else if (match)
                MessageBox.Show("Contact already in the list");
            else
            {
                User user = new User() { phone = CurrentSearch, command = "Search" };
                sender.SendCommand(user, mRevent, MyEvent);
            }
        }
        private bool CanSearch(object o)
        {
            if (CurrentSearch == "")
                return false;
            return true;
        }

        public ICommand OnChangeCommand
        {
            get
            {
                if (_ChangeCommand == null)
                {
                    _ChangeCommand = new DelegateCommand(Change, CanChange);
                }
                return _ChangeCommand;
            }
        }
        private void Change(object o)
        {
            Messages.Clear();
            temp = CurrentContact;
            User user = new User() { contact = CurrentContact, command = "Update" };
            sender.SendCommand(user, mRevent, MyEvent);
            SelectedContactNickname = CurrentContact;
        }
        private bool CanChange(object o)
        {
            return true;
        }

        public ICommand ProfileButtonClick
        {
            get
            {
                if (_UpdProfileCommand == null)
                {
                    _UpdProfileCommand = new DelegateCommand(Update, CanUpdate);
                }
                return _UpdProfileCommand;
            }
        }
        private void Update(object o)
        {
            if (CurrentPhone.Length == 10)
            {
                if (currentPath != null)
                {
                    byte[] ava = GetImageBytes(CurrentPath);
                    if (ava != null)
                    {
                        User user = new User() { login = CurrentLogin, password = CurrentPass, phone = CurrentPhone, avatar = ava, command = "Profile" };
                        sender.SendCommand(user, mRevent, MyEvent2);
                    }
                }
                else if (currentPath == null)
                {
                    User user = new User() { login = CurrentLogin, password = CurrentPass, phone = CurrentPhone, command = "Profile" };
                    sender.SendCommand(user, mRevent, MyEvent2);
                }
                else
                {
                    MessageBox.Show("Image file not found!");
                }
            }
            else
            {
                MessageBox.Show("Phone number must contain 10 digits");
            }
        }
        private bool CanUpdate(object o)
        {
            if (CurrentLogin == "" || CurrentPass == "" || CurrentPhone == "" || CurrentPath == "")
                return false;
            return true;
        }

        public ICommand ChangeProfileButtonClick
        {
            get
            {
                if (_ReadProfileCommand == null)
                {
                    _ReadProfileCommand = new DelegateCommand(ReadPro, CanReadPro);
                }
                return _ReadProfileCommand;
            }
        }
        private void ReadPro(object o)
        {
            User user = new User() { command = "ChangeProfile" };
            sender.SendCommand(user, mRevent, MyEvent2);
        }
        private bool CanReadPro(object o)
        {
            return true;
        }

        public ICommand DeleteProfileButtonClick
        {
            get
            {
                if (_DeleteProfileCommand == null)
                {
                    _DeleteProfileCommand = new DelegateCommand(DelPro, CanDelPro);
                }
                return _DeleteProfileCommand;
            }
        }
        private void DelPro(object o)
        {
            User user = new User() { command = "DeleteProfile" };
            sender.SendCommand(user, mRevent, MyEvent2);
            CurrentLogin = "login";
            CurrentPass = "password";
            CurrentPhone = "phone";
            if (File.Exists("contact_list.json"))
            {
                File.Delete("contact_list.json");
            }
        }
        private bool CanDelPro(object o)
        {
            return true;
        }

        public ICommand SendFileClick
        {
            get
            {
                if (_SendFileCommand == null)
                {
                    _SendFileCommand = new DelegateCommand(SendFile, CanSendFile);
                }
                return _SendFileCommand;
            }
        }

        private void SendFile(object o)
        {
            if (CurrentBlock == "block")
            {
                MessageBox.Show("This contact was blocked. Unblock it to send messages!");
            }
            else if (CurrentContact == null || CurrentContact == "")
            {
                MessageBox.Show("Choose contact to send!");
            }
            else if (CurrentMediaPath != null)
            {
                byte[] img = GetImageBytes(CurrentMediaPath);
                if(img != null)
                {
                    User user = new User() { contact = CurrentContact, data = DateTime.Now.Date.ToString(), media = img, path = Path.GetFileName(CurrentMediaPath), command = "Send" };
                    Chat c = new Chat();
                    c.media = img;
                    c.date = user.data;
                    c.chatContact = CurrentLogin;
                    c.VisibleText = Visibility.Collapsed;
                    c.VisibleMedia = Visibility.Visible;
                    Messages.Insert(0, c);
                    sender.SendCommand(user, mRevent, MyEvent2);
                    mRevent.Set();
                    CurrentMedia = null;
                }
            }
        }
        private bool CanSendFile(object o)
        {
            return true;
        }

        //public ICommand DeleteUserFromContactClick
        //{
        //    get
        //    {
        //        if (_DeleteUserFromContact == null)
        //        {
        //            _DeleteUserFromContact = new DelegateCommand(DeleteUser, CanDeleteUser);
        //        }
        //        return _DeleteUserFromContact;
        //    }
        //}
        //private void DeleteUser(object o)
        //{
           
        //    User user1 = new User() { command = "DeleteUser" , contact = CurrentContact, login = CurrentLogin };
        //    sender.SendCommand(user1, mRevent, MyEvent2);


        
        //    foreach (var user in Contacts)
        //    {
        //        if (user.contact == CurrentContact)
        //        {
        //            Contacts.Remove(user);
        //            break; 
        //        }
        //    }
        //    messages.Clear();
        //    CurrentContact = "";
        

        //}
        //private bool CanDeleteUser(object o)
        //{
        //    return true;

        //}

        public ICommand BlockContact
        {
            get
            {
                if (_BlockContact == null)
                {
                    _BlockContact = new DelegateCommand(Block, CanBlock);
                }
                return _BlockContact;
            }
        }

        private void Block(object o)
        {
            User user = new User() { command = "BlockContact", contact = CurrentContact, login = CurrentLogin };
            sender.SendCommand(user, mRevent, MyEvent2);
        }
        private bool CanBlock(object o)
        {

            return true;
        }

        public ICommand UnblockContact
        {
            get
            {
                if (_UnblockContact == null)
                {
                    _UnblockContact = new DelegateCommand(Unblock, CanUnblock);
                }
                return _UnblockContact;
            }
        }

        private void Unblock(object o)
        {
            User user = new User() { command = "UnblockContact", contact = CurrentContact, login = CurrentLogin };
            sender.SendCommand(user, mRevent, MyEvent2);
        }
        private bool CanUnblock(object o)
        {
            return true;
        }
        public ICommand CloseClick
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new DelegateCommand(Close, CanClose);
                }
                return _CloseCommand;

            }
        }
        private void Close(object o)
        {
            User user = new User() { login = CurrentLogin, command = "CloseCommand", isOnline = "red" };
            sender.SendCommand(user, mRevent, MyEvent2);
            mReventClose.Set();
            //CurrentStatus = "red";

        }
        private bool CanClose(object o)
        {
            return true;
        }
        public ICommand DeleteSmsClick
        {
            get
            {
                if (_DeleteSmsCommand == null)
                {
                    _DeleteSmsCommand = new DelegateCommand(DeleteSms, CanDeleteSms);
                }
                return _DeleteSmsCommand;
            }
        }

        private void DeleteSms(object o)
        {
            //User user = new User() { command = "DeleteSms", mess = CurrentSms, login = CurrentLogin, contact = CurrentContact };

            //foreach (var message in Messages.ToList())
            //{
            //    if (user.mess == CurrentSms)
            //    {
            //        Messages.Remove(message);
            //        break;
            //    }
            //}
            //sender.SendCommand(user, mRevent, MyEvent2);
            if (CurrentSms != null)
            {
                User user = new User() { command = "DeleteSms", mess = CurrentSms, login = CurrentLogin, contact = CurrentContact };

                foreach (var message in Messages.ToList())
                {
                    if (user.mess == CurrentSms)
                    {
                        Messages.Remove(message);
                        break;
                    }
                }
                sender.SendCommand(user, mRevent, MyEvent2);
            }
        }

        private bool CanDeleteSms(object o)
        {
            return true;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region EventHandlers
        public void MyEventHandler()
        {
            if (sender.us.command == "Chat")
            {
                Messages.Clear();
                if (sender.us.blocked == "block")
                {
                    CurrentBlock = "🙁";
                }
                else
                {
                    CurrentBlock = "";
                }
                if(sender.us.isOnline == "green")
                {
                    SelectedContactStatus = "⚫ online";
                    CurrentStatus = sender.us.isOnline;
                }
                else if(sender.us.isOnline == "red")
                {
                    SelectedContactStatus = "⚫ offline";
                    CurrentStatus = sender.us.isOnline;
                }
                else if( sender.us.isOnline == "black")
                {
                    SelectedContactStatus = "⚫ offline";
                    CurrentStatus = sender.us.isOnline;
                }
                CurrentContactPhone = sender.us.phone;
                foreach (var m in sender.us.chat)
                {
                    if(m.media == null)
                    {
                        m.VisibleMedia = Visibility.Collapsed;
                    }
                    if(m.message == null)
                    {
                        m.VisibleText = Visibility.Collapsed;
                    }
                    Messages.Insert(0, m);
                }
                Images.Clear();
                if (sender.us.mediaList != null)
                {
                    foreach (var im in sender.us.mediaList)
                    {
                        BitmapImage tmp = ConvertBitmapFunc(im);
                        Images.Insert(0, tmp);
                    }
                }
                if (sender.us.avatar != null)
                {
                    CurrentAvatar = ConvertBitmapFunc(sender.us.avatar);
                }


            }
            else if (sender.us.command == "Accept")
            {
                if (sender.us.avatar != null)
                {
                    CurrentUserAvatar = ConvertBitmapFunc(sender.us.avatar);
                }
            }
            else if (sender.us.command == "AcceptLog")
            {
                CurrentPhone = sender.us.phone;
                if (sender.us.avatar != null)
                {
                    CurrentUserAvatar = ConvertBitmapFunc(sender.us.avatar);
                }
                foreach (var e in sender.us.profile)
                {
                    User u = new User();
                    if (e.avatar != null)
                    {
                        u.Image = ConvertBitmapFunc(e.avatar);
                    }
                    u.Contact = e.login;
                    u.phone = e.phone;
                    u.avatar = e.avatar;
                    Contacts.Add(u);
                }
            }
            else if (sender.us.command == "Match")
            {
                if (sender.us.avatar != null)
                {
                    sender.us.Image = ConvertBitmapFunc(sender.us.avatar);
                }
                Contacts.Add(sender.us);
                CurrentContact = sender.us.Contact;
            }
            else if (sender.us.command == "CurrentProfile")
            {
                CurrentLogin = sender.us.login;
                CurrentPass = sender.us.password;
                CurrentPhone = sender.us.phone;
            }
            else if (sender.us.command == "ProfileSaved")
            {
                if (sender.us.avatar != null)
                {
                    CurrentUserAvatar = ConvertBitmapFunc(sender.us.avatar);
                }
                CurrentLogin = sender.us.login;
            }
            else if (sender.us.command == "ContactIsBlocked")
            {
                if(sender.us.blocked == "block")
                {
                   foreach(var c in Contacts)
                   {
                        if(c.login == sender.us.contact)
                        {
                            c.blocked = "block";
                            //CurrentBlock = "🙁";
                        }
                   }
                    CurrentBlock = "🙁";
                }
                MessageBox.Show("This contact is in your black list!");
            }
            else if (sender.us.command == "ContactIsUnblocked")
            {
                if (sender.us.blocked == "unblock")
                {
                    foreach (var c in Contacts)
                    {
                        if (c.login == sender.us.contact)
                        {
                            c.blocked = "unblock";
                            
                        }
                    }
                }
                CurrentBlock = "";
                MessageBox.Show("This contact was succesfully unblocked!");
            }
            
        }
        public void MyEventHandler2()
        {
            if (sender.us.command == "SendingMessage")
            {
                if (CurrentContact == sender.us.c.chatContact)
                {
                    
                        if (sender.us.c.media == null)
                        {
                        sender.us.c.VisibleMedia = Visibility.Collapsed;
                        }
                        if (sender.us.c.message == null)
                        {
                        sender.us.c.VisibleText = Visibility.Collapsed;
                        }
                    Messages.Insert(0, sender.us.c);
                    notifier = new PopupNotifier();
                        notifier.BodyColor = Color.Yellow;
                        notifier.TitleText = sender.us.c.chatContact;
                        notifier.ContentText = sender.us.c.message;
                        notifier.TitleFont = new Font("Arial", 20);
                        notifier.ContentFont = new Font("Arial", 18);
                        notifier.Popup();
                }
            }
            else if (sender.us.command == "ContactProfileChanged")
            {
                foreach (var c in Contacts)
                {
                    if (c.Contact == sender.us.mess)
                    {
                        c.Contact = sender.us.login;
                        if (sender.us.avatar != null)
                        {
                            c.Image = ConvertBitmapFunc(sender.us.avatar);
                        }
                    }
                }
            }
            else if(sender.us.command == "ContactOnline")
            {
                foreach (var c in Contacts)
                {
                    if (c.login == sender.us.login && CurrentContact == sender.us.login)
                    {
                        c.isOnline = sender.us.isOnline;
                        CurrentStatus = sender.us.isOnline;
                    }
                    else if (c.login == sender.us.login)
                    {
                        c.isOnline = sender.us.isOnline;
                    }
                }
            }
        }
        #endregion
        #region MyFunctions
        private byte[] GetImageBytes(string p)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Bitmap bitmap = new Bitmap(p);
                bitmap.Save(ms, bitmap.RawFormat);
                return ms.ToArray();
            }
        }

        private BitmapImage ConvertBitmapFunc(byte[] b)
        {
            using (MemoryStream ms = new MemoryStream(b))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
        }
        #endregion
    }
}
