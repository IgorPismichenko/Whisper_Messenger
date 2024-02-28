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
        Socket socket;
        public event Action MyEvent;
        public event Action MyEvent2;
        string temp;
        byte[] defImg;
        private PopupNotifier notifier = null;
        public MainViewModel(ManualResetEvent ev)
        {
            mRevent = ev;
            MyEvent += MyEventHandler;
            MyEvent2 += MyEventHandler2;
            CurrentLogin = "login";
            //CurrentContact = "";
            //CurrentPass = "password";
            CurrentPhone = "phone";
            CurrentSearch = "search";
            Sms = "sms";
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
            if (CurrentContact != "")
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
                Messages.Add(c);
                Sms = "";
                sender.SendCommand(user, mRevent, MyEvent2);

                //string messageToParse = user.mess;

                //Regex regex = new Regex(@"\((\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}:\d{2})\) (\w+):(.*)");

                //Match match = regex.Match(messageToParse);

                //if (match.Success)
                //{
                //    string timestamp = match.Groups[1].Value;
                //    string nickname = match.Groups[2].Value;
                //    string messageText = match.Groups[3].Value;


                //    string formattedMessage = $"{nickname}: {messageText} \n{timestamp}";

                //    Chat c = new Chat();
                //    c.message = formattedMessage;
                //    Messages.Add(c);
                //}
                //else
                //{

                //    Console.WriteLine("Message format is invalid: " + messageToParse);
                //}
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
            if(CurrentMediaPath != null)
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
                    Messages.Add(c);
                    sender.SendCommand(user, mRevent, MyEvent2);
                    mRevent.Set();
                    CurrentMedia = null;
                }
            }
        }
        private bool CanSendFile(object o)
        {
            //if (CurrentMediaPath == null /*|| CurrentContact == null*/)
            //    return false;
            return true;
        }

        public ICommand DeleteUserFromContactClick
        {
            get
            {
                if (_DeleteUserFromContact == null)
                {
                    _DeleteUserFromContact = new DelegateCommand(DeleteUser, CanDeleteUser);
                }
                return _DeleteUserFromContact;
            }
        }
        private void DeleteUser(object o)
        {
           
            User user1 = new User() { command = "DeleteUser" , contact = CurrentContact };
            sender.SendCommand(user1, mRevent, MyEvent2);


        
            foreach (var user in Contacts)
            {
                if (user.contact == CurrentContact)
                {
                    Contacts.Remove(user);
                    break; 
                }
            }
            messages.Clear();
            CurrentContact = "";
        

        }
        private bool CanDeleteUser(object o)
        {
            return true;

        }
        public void MyEventHandler()
        {
            if (sender.us.command == "Chat")
            {
                Messages.Clear();
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
                    Messages.Add(m);
                    //Regex regex = new Regex(@"\((\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}:\d{2})\) (\w+):(.*)");

                    //Match match = regex.Match(m.message);

                    //if (match.Success)
                    //{
                    //    string timestamp = match.Groups[1].Value;
                    //    string nickname = match.Groups[2].Value;
                    //    string messageText = match.Groups[3].Value;


                    //    string formattedMessage = $"{nickname}: {messageText} \n{timestamp}";
                    //    m.message = formattedMessage;
                    //    Messages.Add(m);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Ошибка парсинга сообщения: " + m);
                    //}
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
        }
        public void MyEventHandler2()
        {
            if (sender.us.command == "SendingMessage")
            {
                if (CurrentContact == sender.us.login)
                {
                    Messages.Clear();
                    foreach (var m in sender.us.chat)
                    {
                        if (m.media == null)
                        {
                            m.VisibleMedia = Visibility.Collapsed;
                        }
                        if (m.message == null)
                        {
                            m.VisibleText = Visibility.Collapsed;
                        }
                        Messages.Add(m);
                        //Regex regex = new Regex(@"\((\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}:\d{2})\) (\w+):(.*)");

                        //Match match = regex.Match(m.message);

                        //if (match.Success)
                        //{
                        //    string timestamp = match.Groups[1].Value;
                        //    string nickname = match.Groups[2].Value;
                        //    string messageText = match.Groups[3].Value;


                        //    string formattedMessage = $"{nickname}: {messageText} \n{timestamp}";
                        //    m.message = formattedMessage;
                            //Messages.Add(m);

                            notifier = new PopupNotifier();
                            notifier.BodyColor = Color.Yellow;
                            notifier.TitleText = m.chatContact;
                            notifier.ContentText = m.message;

                            notifier.TitleFont = new Font("Arial", 20);


                            notifier.ContentFont = new Font("Arial", 18);


                            notifier.Popup();

                        //}
                        //else
                        //{
                        //    Console.WriteLine("Ошибка парсинга сообщения: " + m);
                        //}
                    }
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
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
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
