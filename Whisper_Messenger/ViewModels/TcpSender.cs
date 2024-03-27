using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Whisper_Messenger.Models;

namespace Whisper_Messenger.ViewModels
{
    [DataContract]
    public class TcpSender
    {
        public Socket socket;
        [DataMember]
        public User us;
        public event Action MyEvent;
        public TcpSender()
        {
            if (socket == null)
                Connect();
            us = new User();
        }

        public async void Connect()
        {
            await Task.Run(() =>
            {
                try
                {
                    byte[] buf = new byte[1000000];
                    IPAddress ipAddr = IPAddress.Parse("26.208.70.215");
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 49152);
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, buf);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, buf);
                    socket.Connect(ipEndPoint);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент-соединение: " + ex.Message);
                }
            });
        }
        #region Log/Reg
        public async void SendCommand(User user, ManualResetEvent ev, Action a)
        {
            await Task.Run(() =>
            {
                try
                {
                    DataContractJsonSerializer jsonFormatter = null;
                    jsonFormatter = new DataContractJsonSerializer(typeof(User));
                    MemoryStream stream = new MemoryStream();
                    byte[] msg = null;
                    jsonFormatter.WriteObject(stream, user);
                    msg = stream.ToArray();
                    socket.Send(msg);
                    stream.Close();
                    ReceiveData(user, a, ev);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент-отправка: " + ex.Message);
                }
            });
        }
        public async void ReceiveData(User user, Action a, ManualResetEvent ev)
        {
            await Task.Run(() =>
            {
                try
                {
                    byte[] bytes = new byte[10000000];
                    int bytesRec = 0;
                    while (true)
                    {
                        bytesRec = socket.Receive(bytes);
                        if (bytesRec == 0)
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            return;
                        }
                        MemoryStream stream = new MemoryStream(bytes, 0, bytesRec);
                        DataContractJsonSerializer jsonFormatter = null;
                        jsonFormatter = new DataContractJsonSerializer(typeof(User));
                        user = (User)jsonFormatter.ReadObject(stream);
                        stream.Close();
                        if (user.command == "Accept" || user.command == "AcceptLog")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                            ev.Set();
                        }
                        else if (user.command == "Exist")
                        {
                            MessageBox.Show("Such login or phone already exists or you already have an account!");
                        }
                        else if (user.command == "Denied")
                        {
                            MessageBox.Show("Incorrect login or password, try again!");
                        }
                        else if (user.command == "Match")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                        }
                        else if (user.command == "No match")
                        {
                            MessageBox.Show("Such number is not registered, try again!");
                        }

                        else if (user.command == "Chat")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                        }
                        else if (user.command == "CurrentProfile")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                        }
                        else if (user.command == "ProfileSaved")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                        }
                        else if (user.command == "ProfileNotSaved")
                        {
                            MessageBox.Show("Saving new profile failed! Try again");
                        }
                        else if (user.command == "ProfileDeleted")
                        {
                            ev.Set();
                        }
                        else if (user.command == "ProfileNotDeleted")
                        {
                            MessageBox.Show("Deleting profile failed! Try again");
                        }
                        else if(user.command == "successfulDeleted")
                        {
                            MessageBox.Show("Успешно удаленно");
                        }
                        else if (user.command == "ContactIsBlocked")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                        }
                        else if (user.command == "ContactIsUnblocked")
                        {
                            us = user;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                a?.Invoke();
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент: " + ex.Message);
                }
            });
        }
        #endregion
        #region Chat
        public async void ReceiveMessage(Socket sListener, Action ev)
        {
            await Task.Run(() =>
            {
                try
                {
                    byte[] buf = new byte[1000000];
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 49153);
                    sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sListener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, buf);
                    sListener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, buf);
                    sListener.Bind(ipEndPoint);
                    sListener.Listen();
                    while (true)
                    {
                        Socket handler = sListener.Accept();
                        Receive(handler, ev);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент-sms.connect: " + ex.Message);
                }
            });
        }
        public async void Receive(Socket socket, Action ev)
        {
            await Task.Run(() =>
            {
                try
                {
                    User user = new User();
                    DataContractJsonSerializer jsonFormatter = null;
                    jsonFormatter = new DataContractJsonSerializer(typeof(User));
                    byte[] bytes = new byte[10000000];
                    int bytesRec = 0;
                    while (true)
                    { 
                        bytesRec = socket.Receive(bytes);
                        if (bytesRec == 0)
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            return;
                        }
                        MemoryStream stream = new MemoryStream(bytes, 0, bytesRec);
                        user = (User)jsonFormatter.ReadObject(stream);
                        stream.Close();
                        us = user;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ev?.Invoke();
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент-sms.receive: " + ex.Message);
                }
            });
        }
        #endregion
    }
}
