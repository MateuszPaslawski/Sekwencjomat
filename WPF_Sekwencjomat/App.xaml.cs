using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Sekwencjomat
{
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.Source);
            Console.WriteLine(e.Exception.StackTrace);
            Console.WriteLine(e.Exception.TargetSite);
            MessageBox.Show($"Nieoczekiwany błąd:\n{e.Exception.Message}\n\n{e.Exception.Source}\n\n{e.Exception.TargetSite}\n\n{e.Exception.StackTrace}", "Błąd :(", MessageBoxButton.OK, MessageBoxImage.Error);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = true;
        }
    }
}
