using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MediaPreview.xaml
    /// </summary>
    public partial class MediaPreview : Window
    {
        public SynchronizationContext uiContext;
        public ManualResetEvent man_event;
        public MediaPreview()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
            man_event = new ManualResetEvent(false);
            OpenForm();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            man_event = null;
            DataContext = null;
            this.Close();
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
                    man_event = null;
                    DataContext = null;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Клиент-formM: " + ex.Message);
            }
        }
    }
}
