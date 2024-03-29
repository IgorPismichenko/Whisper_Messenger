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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        
        public SynchronizationContext uiContext;
        public ManualResetEvent man_event;
        public Register()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
            man_event = new ManualResetEvent(false);
            OpenForm();
        }
        private void PhoneInFocus(object sender, RoutedEventArgs e)
        {
            phone_textBox.Text = string.Empty;
        }
        private void LogInFocus(object sender, RoutedEventArgs e)
        {
            nickname_textBox.Text = string.Empty;
        }
        private void CloseClick(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void InputLogCheck(object sender, TextCompositionEventArgs e)
        {
            string str = "qwertyuioasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789._@";
            if (!str.Contains(e.Text))
            {
                e.Handled = true;
            }
        }
        private void InputPhoneCheck(object sender, TextCompositionEventArgs e)
        {
            string str = "0123456789";
            if (!str.Contains(e.Text) || phone_textBox.Text.Length == 10)
            {
                e.Handled = true;
            }
        }
        private void FormClosingFunc()
        {
            try
            {
                man_event.Reset();
                MainWindow mainWindow = new MainWindow();
                mainWindow.man_event = man_event;
                mainWindow.DataContext = DataContext;
                
                Thread.Sleep(1500);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент-formR: " + ex.Message);
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
                    MessageBox.Show("Клиент-соединение: " + ex.Message);
                }
            });
        }
        private void LogButtonClick(object sender, RoutedEventArgs e)
        {
            Authentification logIn = new Authentification();
            logIn.man_event = man_event;
            logIn.DataContext = DataContext;
            
            logIn.Show();
            this.Close();
        }
    }
}
