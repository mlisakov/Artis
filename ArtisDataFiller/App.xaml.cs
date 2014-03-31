using System;
using System.Windows;
using System.Windows.Threading;
using Artis.ArtisDataFiller.Properties;
using Artis.Consts;

namespace ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
           // AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ServiceAddress.SetConnectionString(Settings.Default.ConnectionString);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.ConnectionString = ServiceAddress.ArtisConnectionString;
            Settings.Default.Save();
            base.OnExit(e);
        }

        //void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    Exception ex = e.ExceptionObject as Exception;
        //    MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //}

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
                //Handling the exception within the UnhandledExcpeiton handler.
                MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
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