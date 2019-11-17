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
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Nieoczekiwany błąd:\n{e.Exception.Message}\n\n{e.Exception.Source}\n\n{e.Exception.TargetSite}\n\n{e.Exception.StackTrace}", "Błąd :(", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
