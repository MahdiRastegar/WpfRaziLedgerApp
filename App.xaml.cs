using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool Suspended = true;
        protected override void OnStartup(StartupEventArgs e)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU4NUAzMjM3MkUzMTJFMzluT08wbzRnYm4zUlFDOVRzWVpYbUtuSEl0aUhTZmNMYjQxekhrV0NVRnlzPQ==");

            CultureInfo persianCulture = new CultureInfo("fa-IR");
            persianCulture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;

            Thread.CurrentThread.CurrentCulture = persianCulture;
            Thread.CurrentThread.CurrentUICulture = persianCulture;

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            var win = new winLogin();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                win.ShowDialog();
                WpfRaziLedgerApp.MainWindow.Current.Show();
            }));

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(e.Exception.Message + e.Exception.InnerException?.Message, "خطای نرم افزار", MessageBoxButton.OK, MessageBoxImage.Error);

            Environment.Exit(0);
        }
        //public static string GetKeyPassword()
        //{
        //    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp", true);
        //    if (key == null)
        //        key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp");
        //    var x = "";
        //    try
        //    {
        //        x = key.GetValue("Password").ToString();
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        key.Close();
        //    }
        //    return x;
        //}
        //public static void SetKeyPassword(string str)
        //{
        //    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp", true);
        //    if (key == null)
        //        key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp");

        //    key.SetValue("Password", str);

        //    key.Close();
        //}
    }
}
