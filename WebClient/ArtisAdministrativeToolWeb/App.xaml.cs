using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

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
    }

}
