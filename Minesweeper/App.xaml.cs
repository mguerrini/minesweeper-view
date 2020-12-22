using Minesweeper.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = string.Empty;

            try
            {
                if (e.Exception != null)
                    message = string.Format(CultureInfo.CurrentCulture, "{0}", e.Exception.Message);
                else
                    message = "Error desconocido";


                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
            }
            catch (Exception)
            {
                
            }
        }
    }
}
