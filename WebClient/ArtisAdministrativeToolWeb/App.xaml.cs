using System;
using System.Windows;
using System.Windows.Threading;

namespace Artis.ArtisDataFiller
{ 
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShowLoginScreen();
        }
        private void ShowLoginScreen()
        {
            if (MainWindow == null) MainWindow = new Window();
            MainWindow.Content = new MainWindow();
            MainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //Handling the exception within the UnhandledExcpeiton handler.
            MessageBox.Show(e.Exception.Message+Environment.NewLine+e.Exception.StackTrace, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            //}
            //else
            //{
            //    //If you do not set e.Handled to true, the application will close due to crash.
            //    MessageBox.Show("Application is going to close! ", "Uncaught Exception");
            //    e.Handled = false;
            //}
        }
    }

}
