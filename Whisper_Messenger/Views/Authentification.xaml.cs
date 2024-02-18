using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Whisper_Messenger.Views
{
    /// <summary>
    /// Interaction logic for Authentification.xaml
    /// </summary>
    public partial class Authentification : Window
    {
        public Socket? sock;
        public SynchronizationContext uiContext;
        public ManualResetEvent man_event;
        public Authentification()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
            man_event = new ManualResetEvent(false);
            OpenForm();
        }
        private void RegButtonClick(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.man_event = man_event;
            register.DataContext = DataContext;
            register.sock = sock;
            register.Show();
            this.Close();
        }
        //private void PassInFocus(object sender, RoutedEventArgs e)
        //{
        //    Password_textBox.Text = string.Empty;
        //}
        private void LogInFocus(object sender, RoutedEventArgs e)
        {
            nickname_textBox.Text = string.Empty;
        }
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
            this.Close();
        }
        private void FormClosingFunc()
        {
            try
            {
                if (this.IsVisible)
                {
                    man_event.Reset();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.man_event = man_event;
                    mainWindow.DataContext = DataContext;
                    mainWindow.sock = sock;
                    Thread.Sleep(1500);
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент-formA: " + ex.Message);
            }
        }
        private async void OpenForm()
        {
            await Task.Run(() =>
            {
                try
                {
                    man_event.WaitOne();
                    uiContext.Send(d => FormClosingFunc(), null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Клиент-formA-Function: " + ex.Message);
                }
            });
        }
    }
}
