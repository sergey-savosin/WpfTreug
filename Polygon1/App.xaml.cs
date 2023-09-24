using System;
using System.Windows;

namespace Polygon1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Произошла ошибка."
                + Environment.NewLine + e.Exception.Message + "\r\n" + e.Exception.StackTrace, "Ошибка в приложении");
            e.Handled = true;
        }
    }
}
