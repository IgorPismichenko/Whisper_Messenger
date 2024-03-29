using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    
    public partial class MediaPreview : Window
    {
        public SynchronizationContext uiContext;
        public ManualResetEvent man_eventMedia;
        public MediaPreview()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
            
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            
            DataContext = null;
            this.Close();
        }

        
        private void CloseClick(object sender, RoutedEventArgs e)
        {
           
            DataContext = null;
            this.Close();
        }
        private void ChooseFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                MediaPathTextBox.Text = openFileDialog.FileName;
                MediaPathTextBox.Focus();
            }
        }
    }
}
