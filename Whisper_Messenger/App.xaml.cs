using System.Configuration;
using System.Data;
using System.Windows;
using Whisper_Messenger.ViewModels;
using Whisper_Messenger.Views;

namespace Whisper_Messenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            MainViewModel viewModel = new MainViewModel(resetEvent);
            Authentification auth = new Authentification();
            auth.man_event = resetEvent;
            auth.DataContext = viewModel;
            auth.sock = viewModel.sender.socket;
            auth.Show();
        }
    }

}
