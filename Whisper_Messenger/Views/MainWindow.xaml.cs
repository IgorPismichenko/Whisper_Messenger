﻿using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Whisper_Messenger.ViewModels;

namespace Whisper_Messenger.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket? sock;
        public ManualResetEvent man_event;
        public SynchronizationContext uiContext;
        SoundPlayer soundPlayer;
        public static bool isDarkTheme;
        public string theme = "Light";
        public static readonly string ThemeKey = "AppTheme";
        public static readonly string SettingsFilePath = "appsettings.xml";

        public MainWindow()
        {

            LoadSettings();
            InitializeComponent();
            
            man_event = new ManualResetEvent(false);
            uiContext = SynchronizationContext.Current;
            PlayOnStart();
            OpenForm();
        }
        private void SmsInFocus(object sender, RoutedEventArgs e)
        {
            sms.Text = string.Empty;
        }
        private void Border_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                PathTextBox.Text = openFileDialog.FileName;
                PathTextBox.Focus();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
            this.Close();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Maximize_Click(Object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;

            }
        }
        private void SearchInFocus(object sender, RoutedEventArgs e)
        {
            Search_TextBox.Text = string.Empty;
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
                    MessageBox.Show("Клиент-formM-Function: " + ex.Message);
                }
            });
        }
        private void FormClosingFunc()
        {
            try
            {
                if (this.IsVisible)
                {
                    man_event.Reset();
                    Register register = new Register();
                    register.man_event = man_event;
                    register.DataContext = DataContext;
                    register.sock = sock;
                    register.Show();
                   
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент-formM: " + ex.Message);
            }
        }
        private void PlayOnStart()
        {
            soundPlayer = new SoundPlayer("whisper.wav");
            soundPlayer.Play();
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            //theme = (theme == "Light") ? "Dark" : "Light";
            ApplyTheme();
            SaveSettings();
            //Application.Current.Resources[ThemeKey] = isDarkTheme ? "Dark" : "Light";
            Application.Current.Properties[ThemeKey] = isDarkTheme ? "Dark" : "Light";
           
        }
        private void SaveSettings()
        {

            AppSettings settings = new AppSettings();
            settings.Theme = isDarkTheme ? "Dark" : "Light";

            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            using (FileStream stream = new FileStream(SettingsFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, settings);
            }
        }
        private void LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                using (FileStream stream = new FileStream(SettingsFilePath, FileMode.Open))
                {
                    AppSettings settings = (AppSettings)serializer.Deserialize(stream);
                    isDarkTheme = settings.Theme == "Dark";
                    ApplyTheme();
                }
            }
        }
        public void ApplyTheme()
        {
            if (isDarkTheme)
            {
                ChangeTheme.ThemeChange(new Uri("Theme/Light.xaml", UriKind.Relative));
            }
            else
            {
                ChangeTheme.ThemeChange(new Uri("Theme/Dark.xaml", UriKind.Relative));

            }

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
            if (!str.Contains(e.Text) || PhoneTextBox.Text.Length == 10)
            {
                e.Handled = true;
            }
        }
    }
    public class AppSettings
    {
        public string Theme { get; set; }
    }
}