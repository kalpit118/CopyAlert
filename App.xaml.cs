using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace ClipAlert
{
    public partial class App : System.Windows.Application
    {
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("crash.txt", e.ExceptionObject.ToString());
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            File.WriteAllText("crash_disp.txt", e.Exception.ToString());
            e.Handled = true;
            Environment.Exit(1);
        }
    }
}
